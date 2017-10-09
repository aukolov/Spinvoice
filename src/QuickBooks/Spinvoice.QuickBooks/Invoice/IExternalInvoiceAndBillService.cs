namespace Spinvoice.QuickBooks.Invoice
{
    public interface IExternalInvoiceAndBillService
    {
        string Save(Spinvoice.Domain.Accounting.Invoice invoice);
    }
}