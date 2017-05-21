using System.Linq;
using Spinvoice.QuickBooks.Connection;

namespace Spinvoice.QuickBooks.Account
{
    public class ExternalAccountRepository : IExternalAccountRepository
    {
        private readonly ExternalConnection _externalConnection;

        public ExternalAccountRepository(ExternalConnection externalConnection)
        {
            _externalConnection = externalConnection;
            _externalConnection.Connected += TryLoadData;
            TryLoadData();
        }

        private void TryLoadData()
        {
            var accounts = _externalConnection
                .GetAll<Intuit.Ipp.Data.Account>()
                .ToArray();

            InventoryAsset = new ExternalAccount(
                accounts.Single(a => a.Name == "Inventory Asset"));
            CostOfGoodsSold = new ExternalAccount(
                accounts.Single(a => a.Name == "Cost of Goods Sold"));
            SalesOfProductIncome = new ExternalAccount(
                accounts.Single(a => a.Name == "Sales of Product Income"));
        }

        public ExternalAccount SalesOfProductIncome { get; private set; }
        public ExternalAccount CostOfGoodsSold { get; private set; }
        public ExternalAccount InventoryAsset { get; private set; }
    }
}