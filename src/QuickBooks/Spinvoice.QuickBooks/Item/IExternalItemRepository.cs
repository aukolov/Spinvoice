using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.Item
{
    public interface IExternalItemRepository
    {
        IExternalItem GetByName(string name);
        IExternalItem AddInventory(string name);
        IExternalItem AddService(string name, string externalAccountId, Side side);
        IExternalItem GetById(string id);
    }
}