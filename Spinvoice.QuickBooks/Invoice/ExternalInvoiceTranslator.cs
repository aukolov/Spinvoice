using System.Linq;
using Intuit.Ipp.Data;
using Spinvoice.Domain.Accounting;

namespace Spinvoice.QuickBooks.Invoice
{
    public class ExternalInvoiceTranslator
    {
        public Bill Translate(Spinvoice.Domain.Accounting.Invoice invoice)
        {
            return new Bill
            {
                TotalAmt = invoice.TotalAmount,
                Line = invoice.Positions.Select(Translate).ToArray(),
                CurrencyRef = new ReferenceType
                {
                    Value = invoice.Currency,
                    name = "United States Dollar"
                },
                VendorRef = new ReferenceType
                {
                    Value = "56",
                    name = "Bob's Burger Joint"
                },
                ExchangeRate = invoice.ExchangeRate,
                ExchangeRateSpecified = invoice.ExchangeRate != 0
            };
        }

        private static Line Translate(Position position)
        {
            var line = new Line
            {
                Amount = position.Amount,
                AmountSpecified = true,
                Description = position.Description,
                DetailType = LineDetailTypeEnum.AccountBasedExpenseLineDetail,
                DetailTypeSpecified = true,
                AnyIntuitObject = new AccountBasedExpenseLineDetail
                {
                    AccountRef = new ReferenceType
                    {
                        Value = "1"
                    }
                }
            };
            return line;
        }
    }
}