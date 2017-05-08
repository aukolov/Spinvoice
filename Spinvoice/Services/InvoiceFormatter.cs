﻿using System.Linq;
using Spinvoice.Domain.Accounting;

namespace Spinvoice.Services
{
    public static class InvoiceFormatter
    {
        public static string GetInvoiceText(Invoice invoice)
        {
            var text =
                $"{invoice.Date:dd.MM.yyyy}\t" +
                $"{invoice.CompanyName}\t" +
                $"{invoice.VatNumber}\t" +
                $"{invoice.InvoiceNumber}\t" +
                $"{invoice.Currency}\t" +
                $"{invoice.NetAmount}\t" +
                $"{invoice.ExchangeRate}\t" +
                $"{invoice.NetAmountInEuro}\t" +
                $"{invoice.VatAmount}\t" +
                $"{invoice.TotalAmount}\t" +
                $"{invoice.Country}\t" +
                $"{(invoice.IsEuropeanUnion ? "Y" : "N")}";
            return text;
        }

        public static string GetPositionsText(Invoice invoice)
        {
            var text = string.Join("\r\n",
                invoice
                    .Positions
                    .Where(position => !string.IsNullOrEmpty(position.Description)
                        || position.Quantity != 0
                        || position.Amount != 0)
                    .Select(position =>
                    {
                        var line =
                            $"{invoice.Date:dd.MM.yyyy}\t" +
                            $"{invoice.CompanyName}\t" +
                            $"{invoice.InvoiceNumber}\t" +
                            $"{invoice.Currency}\t" +
                            $"{position.Description}\t" +
                            $"{position.Quantity}\t" +
                            $"{position.Amount}";
                        return line;
                    }));
            return text;
        }
    }
}