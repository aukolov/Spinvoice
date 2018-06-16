namespace Spinvoice.QuickBooks.Domain
{
    public interface IInventoryValuationItem
    {
        string Id { get;  }
        string Name { get; }
        decimal Quantity { get; }
        decimal Amount { get; }
    }
}