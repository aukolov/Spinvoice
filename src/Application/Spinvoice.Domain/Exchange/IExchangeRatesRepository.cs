using System;

namespace Spinvoice.Domain.Exchange
{
    public interface IExchangeRatesRepository
    {
        decimal? GetRate(string currency, DateTime date);
    }
}