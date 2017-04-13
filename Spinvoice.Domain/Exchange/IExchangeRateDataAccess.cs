using System;
using Spinvoice.Domain.Company;

namespace Spinvoice.Domain.Exchange
{
    public interface IExchangeRateDataAccess : IBaseDataAccess<Rate>
    {
        Rate GetRate(string currency, DateTime date);
    }
}
