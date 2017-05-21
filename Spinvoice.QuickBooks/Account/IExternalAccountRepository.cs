
namespace Spinvoice.QuickBooks.Account
{
    public interface IExternalAccountRepository
    {
        ExternalAccount SalesOfProductIncome { get; }
        ExternalAccount CostOfGoodsSold { get; }
        ExternalAccount InventoryAsset { get; }
    }
}