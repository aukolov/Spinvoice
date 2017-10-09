using Intuit.Ipp.Data;

namespace Spinvoice.QuickBooks.Invoice
{
    public interface IExternalInvoiceUpdater
    {
        void Update(Spinvoice.Domain.Accounting.Invoice invoice, Bill bill);
        void Update(Spinvoice.Domain.Accounting.Invoice invoice, Intuit.Ipp.Data.Invoice externalInvoice);
    }
}