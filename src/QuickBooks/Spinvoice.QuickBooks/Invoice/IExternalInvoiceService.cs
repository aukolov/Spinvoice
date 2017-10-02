﻿using System.Collections.ObjectModel;
using Intuit.Ipp.Data;

namespace Spinvoice.QuickBooks.Invoice
{
    public interface IExternalInvoiceService
    {
        string Save(Spinvoice.Domain.Accounting.Invoice invoice);
        Bill GetById(string externalInvoiceId);
        void Add(Bill bill);
        void Update(Bill bill);
        ReadOnlyCollection<Bill> GetByExternalCompany(string externalCompanyId);
        void Delete(Bill bill);
    }
}