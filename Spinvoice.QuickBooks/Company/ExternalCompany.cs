using Intuit.Ipp.Data;
using Spinvoice.Domain.ExternalBook;

namespace Spinvoice.QuickBooks.Company
{
    public class ExternalCompany : IExternalCompany
    {
        private readonly Vendor _vendor;

        public ExternalCompany(Vendor vendor)
        {
            _vendor = vendor;
        }

        public ExternalCompany()
        {
            _vendor = new Vendor();
        }

        public string Id
        {
            get { return _vendor.Id; }
            set { _vendor.Id = value; }
        }

        public string Name
        {
            get { return _vendor.DisplayName; }
            set { _vendor.DisplayName = value; }
        }
    }
}