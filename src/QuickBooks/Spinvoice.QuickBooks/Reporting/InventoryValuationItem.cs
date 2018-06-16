using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.Reporting
{
    public class InventoryValuationItem : IInventoryValuationItem
    {
        public InventoryValuationItem(string id, decimal quantity, decimal amount)
        {
            Id = id;
            Quantity = quantity;
            Amount = amount;
        }

        public string Id { get; }
        public decimal Quantity { get; }
        public decimal Amount { get; }
    }
}