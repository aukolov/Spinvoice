using System.Collections.ObjectModel;
using System.Linq;
using Intuit.Ipp.Data;
using Spinvoice.Domain.ExternalBook;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.Utils;

namespace Spinvoice.QuickBooks.Company
{
    public class ExternalCompanyService : IExternalCompanyService
    {
        private readonly ExternalConnection _externalConnection;
        private readonly ExternalCompanyTranslator _externalCompanyTranslator;
        private ObservableCollection<IExternalCompany> _externalCompanies;

        public ExternalCompanyService(
            ExternalCompanyTranslator externalCompanyTranslator,
            ExternalConnection externalConnection)
        {
            _externalConnection = externalConnection;
            _externalCompanyTranslator = externalCompanyTranslator;
            _externalCompanies = new ObservableCollection<IExternalCompany>();
        }

        public ObservableCollection<IExternalCompany> GetAll()
        {
            if (_externalConnection.IsReady
                && !_externalCompanies.Any())
            {
                var externalCompanies = _externalConnection
                    .GetAll<Vendor>()
                    .Select(vendor => _externalCompanyTranslator.Translate(vendor));
                _externalCompanies.Clear();
                _externalCompanies.AddRange(externalCompanies);
            }
            return _externalCompanies;
        }

        public void Save(IExternalCompany externalCompany)
        {
            var vendor = _externalCompanyTranslator.Translate(externalCompany);
            var addedVendor = _externalConnection.Add(vendor);
            externalCompany.Id = addedVendor.Id;
        }
    }
}