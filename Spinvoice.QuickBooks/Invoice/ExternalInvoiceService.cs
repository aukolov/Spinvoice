using Spinvoice.Domain.ExternalBook;
using Spinvoice.QuickBooks.Connection;

namespace Spinvoice.QuickBooks.Invoice
{
    public class ExternalInvoiceService : IExternalInvoiceService
    {
        private readonly ExternalInvoiceTranslator _externalInvoiceTranslator;
        private readonly ExternalConnection _externalConnection;

        public ExternalInvoiceService(
            ExternalInvoiceTranslator externalInvoiceTranslator, 
            ExternalConnection externalConnection)
        {
            _externalInvoiceTranslator = externalInvoiceTranslator;
            _externalConnection = externalConnection;
        }

        public string Save(Domain.Accounting.Invoice invoice)
        {
            var bill = _externalInvoiceTranslator.Translate(invoice);
            var savedBill = _externalConnection.Add(bill);
            return savedBill.Id;
        }
    }
}