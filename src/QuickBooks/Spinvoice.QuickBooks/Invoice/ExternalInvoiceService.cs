using System.Collections.ObjectModel;
using Intuit.Ipp.Data;
using Spinvoice.QuickBooks.Connection;

namespace Spinvoice.QuickBooks.Invoice
{
    public class ExternalInvoiceService : IExternalInvoiceService
    {
        private readonly ExternalInvoiceUpdater _externalInvoiceUpdater;
        private readonly IExternalConnection _externalConnection;

        public ExternalInvoiceService(
            ExternalInvoiceUpdater externalInvoiceUpdater,
            IExternalConnection externalConnection)
        {
            _externalInvoiceUpdater = externalInvoiceUpdater;
            _externalConnection = externalConnection;
        }

        public string Save(Spinvoice.Domain.Accounting.Invoice invoice)
        {
            return invoice.ExternalId == null ? Add(invoice) : Update(invoice);
        }

        private string Add(Spinvoice.Domain.Accounting.Invoice invoice)
        {
            var bill = new Bill();
            _externalInvoiceUpdater.Update(invoice, bill);
            return _externalConnection.Add(bill).Id;
        }

        private string Update(Spinvoice.Domain.Accounting.Invoice invoice)
        {
            var bill = _externalConnection.GetBill(invoice.ExternalId);
            _externalInvoiceUpdater.Update(invoice, bill);
            _externalConnection.Update(bill);
            return invoice.ExternalId;
        }

        public ReadOnlyCollection<Bill> GetByExternalCompany(string externalCompanyId)
        {
            return _externalConnection.GetBillsByCompany(externalCompanyId);
        }

        public Bill GetById(string externalInvoiceId)
        {
            return _externalConnection.GetBill(externalInvoiceId);
        }

        public void Add(Bill bill)
        {
            _externalConnection.Add(bill);
        }

        public void Update(Bill bill)
        {
            _externalConnection.Update(bill);
        }

        public void Delete(Bill bill)
        {
            _externalConnection.Delete(bill);
        }
    }
}