using System.Collections.ObjectModel;
using System.Linq;
using Intuit.Ipp.Data;
using Spinvoice.Domain.ExternalBook;
using Spinvoice.QuickBooks.Connection;
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
            string currency)
        {
            var vendor = new Vendor
            {
                DisplayName = externalCompanyName,
                CurrencyRef = new ReferenceType
                {
                    Value = currency
                }
            };
            var addedVendor = _externalConnection.Add(vendor);
            var externalCompany = new ExternalCompany(addedVendor);
            _externalCompanies.Add(externalCompany);
            return externalCompany;
        }
    }
}