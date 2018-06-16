using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.Reporting
{
    public class InventoryValuationItem : IInventoryValuationItem
    {
        public InventoryValuationItem(
            string id,
            string name, 
            decimal quantity, 
            decimal amount)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Amount = amount;
        }

        public string Id { get; }
        public string Name { get; }
        public decimal Quantity { get; }
        public decimal Amount { get; }
    }
}