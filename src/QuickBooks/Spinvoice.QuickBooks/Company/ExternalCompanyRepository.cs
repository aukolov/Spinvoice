using System;
using System.Collections.ObjectModel;
using System.Linq;
using Intuit.Ipp.Data;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Domain;
using Spinvoice.Utils;

namespace Spinvoice.QuickBooks.Company
{
    public class ExternalCompanyRepository : IExternalCompanyRepository
    {
        private readonly ExternalConnection _externalConnection;
        private readonly ObservableCollection<IExternalCompany> _externalCompanies;

        public ExternalCompanyRepository(
            ExternalConnection externalConnection)
        {
            _externalConnection = externalConnection;
            _externalCompanies = new ObservableCollection<IExternalCompany>();
            _externalConnection.Connected += () => GetAll();
        }

        public ObservableCollection<IExternalCompany> GetAll()
        {
            if (!_externalConnection.IsConnected
                || _externalCompanies.Any())
            {
                return _externalCompanies;
            }

            var externalCompanies = _externalConnection
                .GetAll<Vendor>()
                .Select(vendor => new ExternalCompany(vendor));
            _externalCompanies.Clear();
            _externalCompanies.AddRange(externalCompanies);
            return _externalCompanies;
        }

        public IExternalCompany Create(
            string externalCompanyName,
            Side side,
            string currency)
        {

            switch (side)
            {
                case Side.Vendor:
                    return CreateVendor(externalCompanyName, currency);
                case Side.Customer:
                    return CreateCustomer(externalCompanyName, currency);
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
        }

        private IExternalCompany CreateVendor(string externalCompanyName, string currency)
        {
            var vendor = new Vendor
            {
                CurrencyRef = new ReferenceType
                {
                    Value = currency
                },
                DisplayName = externalCompanyName
            };
            var addedVendor = _externalConnection.Add(vendor);

            var externalCompany = new ExternalCompany(addedVendor);
            _externalCompanies.Add(externalCompany);
            return externalCompany;
        }

        private IExternalCompany CreateCustomer(string externalCompanyName, string currency)
        {
            var customer = new Customer
            {
                CurrencyRef = new ReferenceType
                {
                    Value = currency
                },
                DisplayName = externalCompanyName
            };
            var addedCustomer = _externalConnection.Add(customer);

            var externalCompany = new ExternalCompany(addedCustomer);
            _externalCompanies.Add(externalCompany);
            return externalCompany;
        }
    }
}