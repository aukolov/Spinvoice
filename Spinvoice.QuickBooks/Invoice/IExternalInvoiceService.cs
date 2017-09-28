namespace Spinvoice.QuickBooks.Invoice
{
    public interface IExternalInvoiceService
    {
        string Save(Spinvoice.Domain.Accounting.Invoice invoice);
    }
}