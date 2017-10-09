using System.Collections.ObjectModel;
using Intuit.Ipp.Data;

namespace Spinvoice.QuickBooks.Invoice
{
    public interface IExternalBillCrudService
    {
        ReadOnlyCollection<Bill> GetByExternalCompany(string externalCompanyId);
        Bill GetById(string externalInvoiceId);
        void Add(Bill bill);
        void Update(Bill bill);
        void Delete(Bill bill);
    }
}