using System;
using Intuit.Ipp.Data;

namespace Spinvoice.QuickBooks.Connection
{
    public interface IExternalConnection
    {
        T Add<T>(T entity) where T : IEntity;
        T[] GetAll<T>() where T : IEntity, new();
        Intuit.Ipp.Data.ExchangeRate GetExchangeRate(DateTime date, string sourceCurrency);
    }
}