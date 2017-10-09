using System.Collections.ObjectModel;
using Intuit.Ipp.Data;
using Spinvoice.QuickBooks.Connection;

namespace Spinvoice.QuickBooks.Invoice
{
    public class ExternalBillCrudService : IExternalBillCrudService
    {
        private readonly IExternalConnection _externalConnection;

        public ExternalBillCrudService(IExternalConnection externalConnection)
        {
            _externalConnection = externalConnection;
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