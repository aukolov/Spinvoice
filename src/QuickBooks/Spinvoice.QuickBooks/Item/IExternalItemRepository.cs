using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.Item
{
    public interface IExternalItemRepository
    {
        IExternalItem Get(string name);
        IExternalItem AddInventory(string name);
        IExternalItem AddService(string name, string externalAccountId, Side side);
    }
}