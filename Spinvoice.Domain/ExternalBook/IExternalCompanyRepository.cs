using System.Collections.ObjectModel;

namespace Spinvoice.Domain.ExternalBook
{
    public interface IExternalCompanyRepository
    {
        ObservableCollection<IExternalCompany> GetAll();
        IExternalCompany Create(string externalCompanyName, string currency);
    }
}