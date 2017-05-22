using System.Linq;
using Intuit.Ipp.Data;
using Spinvoice.Domain.Accounting;

namespace Spinvoice.QuickBooks.Invoice
{
    public class ExternalInvoiceTranslator
    {
        public Bill Translate(Domain.Accounting.Invoice invoice)
        {
            return new Bill
            {
                TotalAmt = invoice.TotalAmount,
                Line = invoice.Positions
                    .Where(position => !string.IsNullOrEmpty(position.Name))
                    .Select(Translate).ToArray(),
                CurrencyRef = new ReferenceType
                {
                    Value = invoice.Currency
                },
                VendorRef = new ReferenceType
                {
                    Value = invoice.ExternalCompanyId
                },
                ExchangeRate = invoice.ExchangeRate,
                ExchangeRateSpecified = invoice.ExchangeRate != 0,
                DocNumber = invoice.InvoiceNumber,
                TxnDate = invoice.Date,
                TxnDateSpecified = true
            };
        }

        private static Line Translate(Position position)
        {
            var line = new Line
            {
                Amount = position.Amount,
                AmountSpecified = true,
                DetailType = LineDetailTypeEnum.ItemBasedExpenseLineDetail,
                DetailTypeSpecified = true,
                AnyIntuitObject = new ItemBasedExpenseLineDetail
                {
                    ItemRef = new ReferenceType
                    {
                        Value = position.ExternalId
                    },
                    Qty = position.Quantity,
                    QtySpecified = true,
                    ItemElementName = ItemChoiceType.UnitPrice,
                    AnyIntuitObject = position.Quantity == 0
                        ? 0m
                        : position.Amount / position.Quantity

                }
            };
            return line;
        }
    }
}