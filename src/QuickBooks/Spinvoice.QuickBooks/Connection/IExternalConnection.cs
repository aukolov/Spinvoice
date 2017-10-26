using System;
using System.Collections.ObjectModel;
using Intuit.Ipp.Data;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.Connection
{
    public interface IExternalConnection : IExternalConnectionWatcher
    {
        T Add<T>(T entity) where T : IEntity;
        T[] GetAll<T>() where T : IEntity, new();
        T Update<T>(T entity) where T : IEntity;
        void Delete<T>(T entity) where T : IEntity;

        Intuit.Ipp.Data.ExchangeRate GetExchangeRate(DateTime date, string sourceCurrency);

        Bill GetBill(string externalInvoiceId);
        ReadOnlyCollection<Bill> GetBillsByCompany(string externalCompanyId);

        Intuit.Ipp.Data.Invoice GetInvoice(string externalInvoiceId);
        ReadOnlyCollection<Intuit.Ipp.Data.Invoice> GetInvoicesByCompany(string externalCompanyId);
    }
}