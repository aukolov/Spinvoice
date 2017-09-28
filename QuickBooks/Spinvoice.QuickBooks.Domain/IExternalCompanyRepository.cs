using System.Collections.ObjectModel;

namespace Spinvoice.QuickBooks.Domain
{
    public interface IExternalCompanyRepository
    {
        ObservableCollection<IExternalCompany> GetAll();
        IExternalCompany Create(string externalCompanyName, string currency);
    }
}