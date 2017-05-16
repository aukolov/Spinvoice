using Spinvoice.Domain.Accounting;

namespace Spinvoice.Domain.QuickBooks
{
    public interface IInvoiceService
    {
        void Save(Invoice invoice);
    }
}