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
        private readonly ObservableCollection<IExternalCompany> _externalVendors;
        private readonly ObservableCollection<IExternalCompany> _externalCustomers;

        public ExternalCompanyRepository(
            ExternalConnection externalConnection)
        {
            _externalConnection = externalConnection;
            _externalVendors = new ObservableCollection<IExternalCompany>();
            _externalCustomers = new ObservableCollection<IExternalCompany>();
            _externalConnection.Connected += () =>
            {
                GetAllVendors();
                GetAllCustomers();
            };
        }

        public ObservableCollection<IExternalCompany> GetAllVendors()
        {
            if (!_externalConnection.IsConnected
                || _externalVendors.Any())
            {
                return _externalVendors;
            }

            var externalVendors = _externalConnection
                .GetAll<Vendor>()
                .Select(x => new ExternalCompany(x));

            _externalVendors.Clear();
            _externalVendors.AddRange(externalVendors);

            return _externalVendors;
        }

        public ObservableCollection<IExternalCompany> GetAllCustomers()
        {
            if (!_externalConnection.IsConnected
                || _externalCustomers.Any())
            {
                return _externalCustomers;
            }

            var externalCustomers = _externalConnection
                .GetAll<Customer>()
                .Select(x => new ExternalCompany(x));

            _externalCustomers.Clear();
            _externalCustomers.AddRange(externalCustomers);

            return _externalCustomers;
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
            _externalVendors.Add(externalCompany);
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
            _externalCustomers.Add(externalCompany);
            return externalCompany;
        }
    }
}