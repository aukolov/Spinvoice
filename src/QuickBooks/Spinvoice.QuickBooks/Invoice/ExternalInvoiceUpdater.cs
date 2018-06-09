using System;
using System.Collections.Generic;
using System.Linq;
using Intuit.Ipp.Data;
using Spinvoice.Domain.Accounting;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.Invoice
{
    public class ExternalInvoiceUpdater : IExternalInvoiceUpdater
    {
        private readonly IAccountsChartRepository _accountsChartRepository;

        public ExternalInvoiceUpdater(IAccountsChartRepository accountsChartRepository)
        {
            _accountsChartRepository = accountsChartRepository;
        }

        public void Update(Spinvoice.Domain.Accounting.Invoice invoice, Bill bill)
        {
            bill.TotalAmt = invoice.TotalAmount;
            bill.VendorRef = new ReferenceType
            {
                Value = invoice.ExternalCompanyId
            };
            bill.Line = AccountLines(invoice).Concat(ItemLines(invoice)).ToArray();
            UpdateCommon(invoice, bill);
        }

        public void Update(Spinvoice.Domain.Accounting.Invoice invoice, Intuit.Ipp.Data.Invoice externalInvoice)
        {
            externalInvoice.TotalAmt = invoice.TotalAmount;
            externalInvoice.CustomerRef = new ReferenceType
            {
                Value = invoice.ExternalCompanyId
            };
            externalInvoice.GlobalTaxCalculation = GlobalTaxCalculationEnum.NotApplicable;
            externalInvoice.GlobalTaxCalculationSpecified = true;
            externalInvoice.Line = AccountLines(invoice).Concat(ItemLines(invoice)).ToArray();
            UpdateCommon(invoice, externalInvoice);
        }

        private static void UpdateCommon(
            Spinvoice.Domain.Accounting.Invoice invoice,
            Transaction transaction)
        {
            transaction.CurrencyRef = new ReferenceType
            {
                Value = invoice.Currency
            };
            transaction.ExchangeRate = invoice.ExchangeRate;
            transaction.ExchangeRateSpecified = invoice.ExchangeRate != 0;
            transaction.DocNumber = invoice.InvoiceNumber;
            transaction.TxnDate = invoice.Date;
            transaction.TxnDateSpecified = true;
        }

        private IEnumerable<Line> AccountLines(Spinvoice.Domain.Accounting.Invoice invoice)
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
                DetailTypeSpecified = true,
                DetailType = LineDetailTypeEnum.AccountBasedExpenseLineDetail,
                AnyIntuitObject = new AccountBasedExpenseLineDetail
                {
                    AccountRef = new ReferenceType
                    {
                        Value = accountId
                    }
                }
            };
        }

        private static IEnumerable<Line> ItemLines(Spinvoice.Domain.Accounting.Invoice invoice)
        {
            return invoice.Positions
                .Where(position => !string.IsNullOrEmpty(position.Name))
                .Select(position => TranslatePosition(invoice.Side, position));
        }

        private static Line TranslatePosition(Side side, Position position)
        {
            ItemLineDetail lineDetail;
            LineDetailTypeEnum lineDetailType;
            switch (side)
            {
                case Side.Vendor:
                    lineDetail = new ItemBasedExpenseLineDetail();
                    lineDetailType = LineDetailTypeEnum.ItemBasedExpenseLineDetail;
                    break;
                case Side.Customer:
                    lineDetail = new SalesItemLineDetail();
                    lineDetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
            lineDetail.ItemRef = new ReferenceType
            {
                Value = position.ExternalId
            };
            lineDetail.Qty = position.Quantity;
            lineDetail.QtySpecified = true;
            lineDetail.ItemElementName = ItemChoiceType.UnitPrice;
            lineDetail.AnyIntuitObject = position.Quantity == 0
                ? 0m
                : position.Amount / position.Quantity;

            var line = new Line
            {
                Amount = position.Amount,
                AmountSpecified = true,
                DetailType = lineDetailType,
                DetailTypeSpecified = true,
                AnyIntuitObject = lineDetail
            };
            return line;
        }
    }
}