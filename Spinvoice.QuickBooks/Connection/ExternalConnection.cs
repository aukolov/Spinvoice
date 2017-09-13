using System;
using System.Collections.Generic;
using System.Linq;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using NLog;
using Spinvoice.Domain.ExternalBook;
using Spinvoice.Utils;

namespace Spinvoice.QuickBooks.Connection
{
    public class ExternalConnection : IExternalConnectionWatcher, IExternalConnection
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IOAuthRepository _oauthRepository;
        private DataService _dataService;
        private QueryService<Intuit.Ipp.Data.ExchangeRate> _queryService;

        public ExternalConnection(
            IOAuthRepository oauthRepository)
        {
            _oauthRepository = oauthRepository;

            _oauthRepository.Profile.Updated += TryConnect;
            TryConnect();
        }

        public event Action Connected;

        public bool IsConnected => _dataService != null;

        public T Add<T>(T entity) where T : IEntity
        {
            if (!IsConnected) throw new InvalidOperationException();
            return _dataService.Add(entity);
        }

        public T[] GetAll<T>() where T : IEntity, new()
        {
            if (!IsConnected) throw new InvalidOperationException();
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
            if (!_oauthRepository.Profile.IsReady)
            {
                return;
            }
            var oauthRequestValidator = new OAuthRequestValidator(
                _oauthRepository.Profile.AccessToken,
                _oauthRepository.Profile.AccessSecret,
                _oauthRepository.Params.ConsumerKey,
                _oauthRepository.Params.ConsumerSecret);
            var serviceContext = new ServiceContext(
                _oauthRepository.Profile.RealmId,
                IntuitServicesType.QBO,
                oauthRequestValidator);
            var dataService = new DataService(serviceContext);

            try
            {
                dataService.FindAll(new Intuit.Ipp.Data.Account());
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error when testing connection with QuickBooks.");
                return;
            }

            _dataService = dataService;
            _queryService = new QueryService<Intuit.Ipp.Data.ExchangeRate>(serviceContext);
            Connected.Raise();
        }

        public Intuit.Ipp.Data.ExchangeRate GetExchangeRate(DateTime date, string sourceCurrency)
        {
            var items = _queryService.ExecuteIdsQuery(
                "select * from exchangerate " +
                $"where sourcecurrencycode='{sourceCurrency}' " +
                $"and asofdate='{date:yyyy-MM-dd}'");
            return items.FirstOrDefault();
        }
    }
}