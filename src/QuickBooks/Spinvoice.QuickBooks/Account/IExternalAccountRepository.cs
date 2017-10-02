
using System.Collections.ObjectModel;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.Account
{
    public interface IExternalAccountRepository
    {
        ObservableCollection<IExternalAccount> GetAll();
    }
}