using System;
using NLog;
using Spinvoice.Domain.Exchange;
using Spinvoice.QuickBooks.Connection;

namespace Spinvoice.QuickBooks.ExchangeRate
{
    public class ExternalExchangeRatesRepository : IExchangeRatesRepository
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ExternalConnection _externalConnection;

        public ExternalExchangeRatesRepository(ExternalConnection externalConnection)
        {
            _externalConnection = externalConnection;
        }

        public decimal? GetRate(string currency, DateTime date)
        {
            if (currency == "EUR")
            {
                return 1;
            }
            if (!_externalConnection.IsConnected)
            {
                return null;
            }
            Intuit.Ipp.Data.ExchangeRate exchangeRate;
            try
            {
                exchangeRate = _externalConnection.GetExchangeRate(date, currency);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while loading exchange rates");
                return null;
            }
            if (exchangeRate == null)
            {
                return null;
            }
            if (!exchangeRate.RateSpecified)
            {
                return null;
            }
            return exchangeRate.Rate;
        }
    }
}