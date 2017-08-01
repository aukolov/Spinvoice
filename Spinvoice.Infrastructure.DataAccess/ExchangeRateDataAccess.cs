using System;
using System.Linq;
using Spinvoice.Domain.Exchange;

namespace Spinvoice.Infrastructure.DataAccess
{
    public class ExchangeRateDataAccess : BaseDataAccess<Rate>, IExchangeRateDataAccess
    {
        public ExchangeRateDataAccess(IDocumentStoreContainer documentStoreContainer)
            : base(documentStoreContainer)
        {
        }

        public Rate GetRate(string currency, DateTime date)
        {
            using (var session = DocumentStore.OpenSession())
            {
               return session.Query<Rate>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfNow(TimeSpan.FromMinutes(2)))
                    .FirstOrDefault(r => r.Currency == currency && r.Date == date);
            }
        }
    }
}