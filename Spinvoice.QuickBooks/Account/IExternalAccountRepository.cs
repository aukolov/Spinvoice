
using System.Collections.ObjectModel;
using Spinvoice.Domain.ExternalBook;

namespace Spinvoice.QuickBooks.Account
{
    public interface IExternalAccountRepository
    {
        ObservableCollection<IExternalAccount> GetAll();
    }
}