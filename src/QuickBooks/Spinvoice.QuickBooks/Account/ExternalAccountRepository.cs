using System.Collections.ObjectModel;
using System.Linq;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Domain;
using Spinvoice.Utils;

namespace Spinvoice.QuickBooks.Account
{
    public class ExternalAccountRepository : IExternalAccountRepository
    {
        private readonly ExternalConnection _externalConnection;
        private readonly ObservableCollection<IExternalAccount> _externalAccounts =
            new ObservableCollection<IExternalAccount>();

        public ExternalAccountRepository(ExternalConnection externalConnection)
        {
            _externalConnection = externalConnection;
            _externalConnection.Connected += TryLoadData;
            TryLoadData();
        }

        private void TryLoadData()
        {
            if (!_externalConnection.IsConnected)
            {
                return;
            }

            var accounts = _externalConnection
                .GetAll<Intuit.Ipp.Data.Account>()
                .ToArray();

            _externalAccounts.Clear();
            _externalAccounts.AddRange(accounts.Select(account => new ExternalAccount(account)).OrderBy(account => account.Name));
        }

        public ObservableCollection<IExternalAccount> GetAll()
        {
            return _externalAccounts;
        }
    }
}