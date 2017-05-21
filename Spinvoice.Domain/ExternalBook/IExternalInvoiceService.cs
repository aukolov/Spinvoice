using Spinvoice.Domain.Accounting;

namespace Spinvoice.Domain.ExternalBook
{
    public interface IExternalInvoiceService
    {
        string Save(Invoice invoice);
    }
}