using Intuit.Ipp.Data;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.Company
{
    public class ExternalCompany : IExternalCompany
    {
        private readonly NameBase _nameBase;

        public ExternalCompany(Vendor vendor)
        {
            _nameBase = vendor;
            Side = Side.Vendor;
        }

        public ExternalCompany(Customer customer)
        {
            _nameBase = customer;
            Side = Side.Customer;
        }

        public string Id => _nameBase.Id;
        public string Name => _nameBase.DisplayName;
        public Side Side { get; }
    }
}