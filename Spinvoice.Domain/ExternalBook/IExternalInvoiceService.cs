using Spinvoice.Domain.Accounting;

namespace Spinvoice.Domain.ExternalBook
{
    public interface IExternalInvoiceService
    {
        void Save(Invoice invoice);
    }
}