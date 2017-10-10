using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.QueryFilter;
using Spinvoice.QuickBooks.Domain;
using Spinvoice.Utils;

namespace Spinvoice.QuickBooks.Connection
{
    public class ExternalConnection : IExternalConnectionWatcher, IExternalConnection
    {
        private readonly IOAuthRepository _oauthRepository;
        private readonly IExternalAuthService _externalAuthService;
        private DataService _dataService;
        private QueryService<Intuit.Ipp.Data.ExchangeRate> _exchangeRateQueryService;
        private QueryService<Bill> _billQueryService;
        private QueryService<Intuit.Ipp.Data.Invoice> _invoiceQueryService;

        public ExternalConnection(
            IOAuthRepository oauthRepository,
            IExternalAuthService externalAuthService)
        {
            _oauthRepository = oauthRepository;
            _externalAuthService = externalAuthService;

            _oauthRepository.Profile.Updated += TryConnect;
            TryConnect();
        }

        public event Action Connected;

        public bool IsConnected => _dataService != null;

        public T Add<T>(T entity) where T : IEntity
        {
            VerifyConnected();
            return _dataService.Add(entity);
        }

        public T Update<T>(T entity) where T : IEntity
        {
            VerifyConnected();
            return _dataService.Update(entity);
        }

        public void Delete<T>(T entity) where T : IEntity
        {
            VerifyConnected();
            _dataService.Delete(entity);
        }

        public T[] GetAll<T>() where T : IEntity, new()
        {
            VerifyConnected();
            var allItems = new List<T>();
            var i = 1;
            var maxResults = 1000;
            while (true)
            {
                var loadedItems = _dataService.FindAll(
                    new T(),
                    startPosition: i,
                    maxResults: maxResults).ToArray();
                allItems.AddRange(loadedItems);
                if (loadedItems.Length < maxResults)
                {
                    break;
                }
                i += maxResults;
            }
            return allItems.ToArray();
        }

        private void TryConnect()
        {
            ServiceContext serviceContext;
            if (!_externalAuthService.TryConnect(out serviceContext, _oauthRepository.Profile, _oauthRepository.Params))
            {
                return;
            }

            _dataService = new DataService(serviceContext);
            _exchangeRateQueryService = new QueryService<Intuit.Ipp.Data.ExchangeRate>(serviceContext);
            _billQueryService = new QueryService<Bill>(serviceContext);
            _invoiceQueryService = new QueryService<Intuit.Ipp.Data.Invoice>(serviceContext);
            Connected.Raise();
        }

        public Intuit.Ipp.Data.ExchangeRate GetExchangeRate(DateTime date, string sourceCurrency)
        {
            VerifyConnected();

            var items = _exchangeRateQueryService.ExecuteIdsQuery(
                "select * from exchangerate " +
                $"where sourcecurrencycode='{sourceCurrency}' " +
                $"and asofdate='{date:yyyy-MM-dd}'");
            return items.FirstOrDefault();
        }

        public Bill GetBill(string externalInvoiceId)
        {
            VerifyConnected();

            var bills = _billQueryService.ExecuteIdsQuery(
                $"select * from bill where Id = '{externalInvoiceId}'");
            return bills.SingleOrDefault();
        }

        public ReadOnlyCollection<Bill> GetBillsByCompany(string externalCompanyId)
        {
            VerifyConnected();

            var bills = _billQueryService.ExecuteIdsQuery(
                $"select * from bill where VendorRef = '{externalCompanyId}'");
            return bills;
        }

        public Intuit.Ipp.Data.Invoice GetInvoice(string externalInvoiceId)
        {
            var invoices = _invoiceQueryService.ExecuteIdsQuery(
                $"select * from invoice where Id = '{externalInvoiceId}'");
            return invoices.SingleOrDefault();
        }

        public ReadOnlyCollection<Intuit.Ipp.Data.Invoice> GetInvoicesByCompany(string externalCompanyId)
        {
            VerifyConnected();

            var invoices = _invoiceQueryService.ExecuteIdsQuery(
                $"select * from invoice where CustomerRef = '{externalCompanyId}'");
            return invoices;
        }

        private void VerifyConnected()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Not connected.");
            }
        }
    }
}