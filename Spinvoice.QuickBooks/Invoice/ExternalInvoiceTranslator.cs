using System.Collections.Generic;
using System.Linq;
using Intuit.Ipp.Data;
using Spinvoice.Domain.Accounting;

namespace Spinvoice.QuickBooks.Invoice
{
    public class ExternalInvoiceTranslator
    {
        private readonly IAccountsChartRepository _accountsChartRepository;

        public ExternalInvoiceTranslator(IAccountsChartRepository accountsChartRepository)
        {
            _accountsChartRepository = accountsChartRepository;
        }

        public Bill Translate(Domain.Accounting.Invoice invoice)
        {
            return new Bill
            {
                TotalAmt = invoice.TotalAmount,
                Line = AccountLines(invoice).Concat(ItemLines(invoice)).ToArray(),
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

        private IEnumerable<Line> AccountLines(Domain.Accounting.Invoice invoice)
        {
            if (invoice.VatAmount != 0)
            {
                var line = TranslateAccountBasedItem(
                    _accountsChartRepository.AccountsChart.VatAccountId,
                    invoice.VatAmount);
                yield return line;
            }

            if (invoice.TransportationCosts != 0)
            {
                var line = TranslateAccountBasedItem(
                    _accountsChartRepository.AccountsChart.TransportationCostsAccountId,
                    invoice.TransportationCosts);
                yield return line;
            }
        }

        private Line TranslateAccountBasedItem(string accountId, decimal amount)
        {
            return new Line
            {
                Amount = amount,
                AmountSpecified = true,
                DetailType = LineDetailTypeEnum.AccountBasedExpenseLineDetail,
                DetailTypeSpecified = true,
                AnyIntuitObject = new AccountBasedExpenseLineDetail
                {
                    AccountRef = new ReferenceType
                    {
                        Value = accountId
                    }
                }
            };
        }

        private static IEnumerable<Line> ItemLines(Domain.Accounting.Invoice invoice)
        {
            return invoice.Positions
                .Where(position => !string.IsNullOrEmpty(position.Name))
                .Select(TranslatePosition);
        }

        private static Line TranslatePosition(Position position)
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