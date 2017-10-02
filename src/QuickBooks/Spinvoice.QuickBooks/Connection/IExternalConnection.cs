using System;
using System.Collections.ObjectModel;
using Intuit.Ipp.Data;

namespace Spinvoice.QuickBooks.Connection
{
    public interface IExternalConnection
    {
        T Add<T>(T entity) where T : IEntity;
        T[] GetAll<T>() where T : IEntity, new();
        Intuit.Ipp.Data.ExchangeRate GetExchangeRate(DateTime date, string sourceCurrency);
        Bill GetBill(string externalInvoiceId);
        ReadOnlyCollection<Bill> GetBillsByCompany(string externalCompanyId);
        T Update<T>(T entity) where T : IEntity;
        void Delete<T>(T entity) where T : IEntity;
    }
}