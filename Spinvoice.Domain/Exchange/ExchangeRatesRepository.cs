using System;

namespace Spinvoice.Domain.Exchange
{
    public class ExchangeRatesRepository : IExchangeRatesRepository
    {
        private readonly IExchangeRateDataAccess _exchangeRateDataAccess;

        public ExchangeRatesRepository(IExchangeRateDataAccess exchangeRateDataAccess)
        {
            _exchangeRateDataAccess = exchangeRateDataAccess;
        }

        public decimal GetRate(string currency, DateTime date)
        {
            if (currency == "EUR")
            {
                return 1;
            }
            var rate = _exchangeRateDataAccess.GetRate(currency, date);
            return rate?.Value ?? 0;
        }
    }
}