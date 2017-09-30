using System.Collections.ObjectModel;
using Intuit.Ipp.Data;
using Spinvoice.QuickBooks.Connection;

namespace Spinvoice.QuickBooks.Invoice
{
    public class ExternalInvoiceService : IExternalInvoiceService
    {
        private readonly ExternalInvoiceTranslator _externalInvoiceTranslator;
        private readonly IExternalConnection _externalConnection;

        public ExternalInvoiceService(
            ExternalInvoiceTranslator externalInvoiceTranslator, 
            IExternalConnection externalConnection)
        {
            _externalInvoiceTranslator = externalInvoiceTranslator;
            _externalConnection = externalConnection;
        }

        public string Save(Spinvoice.Domain.Accounting.Invoice invoice)
        {
            var bill = _externalInvoiceTranslator.Translate(invoice);
            var savedBill = _externalConnection.Add(bill);
            return savedBill.Id;
        }

        public ReadOnlyCollection<Bill> GetByExternalCompany(string externalCompanyId)
        {
            return _externalConnection.GetBillsByCompany(externalCompanyId);
        }

        public Bill GetById(string externalInvoiceId)
        {
            return _externalConnection.GetBill(externalInvoiceId);
        }

        public void Update(Bill bill)
        {
            _externalConnection.Update(bill);
        }
    }
}