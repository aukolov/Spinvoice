using System.Collections.ObjectModel;

namespace Spinvoice.QuickBooks.Invoice
{
    public interface IExternalInvoiceCrudService
    {
        ReadOnlyCollection<Intuit.Ipp.Data.Invoice> GetByExternalCompany(string externalCompanyId);
        Intuit.Ipp.Data.Invoice GetById(string externalInvoiceId);
        void Add(Intuit.Ipp.Data.Invoice bill);
        void Update(Intuit.Ipp.Data.Invoice bill);
        void Delete(Intuit.Ipp.Data.Invoice bill);
    }
}