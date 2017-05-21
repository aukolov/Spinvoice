using Spinvoice.Domain.ExternalBook;

namespace Spinvoice.QuickBooks.Item
{
    public interface IExternalItemRepository
    {
        IExternalItem Get(string name);
        IExternalItem Add(string name);
    }
}