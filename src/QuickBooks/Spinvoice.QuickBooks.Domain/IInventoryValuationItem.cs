namespace Spinvoice.QuickBooks.Domain
{
    public interface IInventoryValuationItem
    {
        string Id { get;  }
        decimal Quantity { get; }
        decimal Amount { get; }
    }
}