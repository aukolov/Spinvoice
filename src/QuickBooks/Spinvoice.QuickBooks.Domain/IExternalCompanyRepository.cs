using System.Collections.ObjectModel;

namespace Spinvoice.QuickBooks.Domain
{
    public interface IExternalCompanyRepository
    {
        ObservableCollection<IExternalCompany> GetAllVendors();
        ObservableCollection<IExternalCompany> GetAllCustomers();

        IExternalCompany Create(
            string externalCompanyName,
            Side side,
            string currency);
    }
}