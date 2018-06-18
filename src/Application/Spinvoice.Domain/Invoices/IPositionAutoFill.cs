using System.Collections.Generic;
using Spinvoice.Domain.Accounting;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.Domain.Invoices
{
    public interface IPositionAutoFill
    {
        Position[] FillPositions(IEnumerable<IInventoryValuationItem> items,
            decimal totalAmount,
            decimal exchangeRate,
            decimal markup);
    }
}