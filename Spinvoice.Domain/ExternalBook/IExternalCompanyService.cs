using System.Collections.ObjectModel;

namespace Spinvoice.Domain.ExternalBook
{
    public interface IExternalCompanyService
    {
        ObservableCollection<IExternalCompany> GetAll();
        void Save(IExternalCompany externalCompany);
    }
}