using Intuit.Ipp.Data;
using Spinvoice.Domain.ExternalBook;

namespace Spinvoice.QuickBooks.Company
{
    public class ExternalCompanyTranslator
    {
        public Vendor Translate(IExternalCompany externalCompany)
        {
            var vendor = new Vendor
            {
                DisplayName = externalCompany.Name
            };
            return vendor;
        }

        public IExternalCompany Translate(Vendor vendor)
        {
            var externalCompany = new ExternalCompany(vendor);
            return externalCompany;
        }
    }
}