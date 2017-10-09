using System.Collections.ObjectModel;
using Intuit.Ipp.Data;
using Spinvoice.QuickBooks.Connection;

namespace Spinvoice.QuickBooks.Invoice
{
    public class ExternalInvoiceCrudService : IExternalInvoiceCrudService
    {
        private readonly IExternalConnection _externalConnection;

        public ExternalInvoiceCrudService(IExternalConnection externalConnection)
        {
            _externalConnection = externalConnection;
        }

        public ReadOnlyCollection<Intuit.Ipp.Data.Invoice> GetByExternalCompany(string externalCompanyId)
        {
            return _externalConnection.GetInvoicesByCompany(externalCompanyId);
        }

        public Intuit.Ipp.Data.Invoice GetById(string externalInvoiceId)
        {
            return _externalConnection.GetInvoice(externalInvoiceId);
        }

        public void Add(Intuit.Ipp.Data.Invoice bill)
        {
            _externalConnection.Add(bill);
        }

        public void Update(Intuit.Ipp.Data.Invoice bill)
        {
            _externalConnection.Update(bill);
        }

        public void Delete(Intuit.Ipp.Data.Invoice bill)
        {
            _externalConnection.Delete(bill);
        }
    }
}