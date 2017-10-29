using System.Linq;
using Spinvoice.Domain.Accounting;

namespace Spinvoice.Application.Services
{
    public static class InvoiceFormatter
    {
        public static string GetInvoiceText(Invoice invoice)
        {
            var stringDate = invoice.Date.ToString(Format.Date);
            var text =
                stringDate + "\t" +
                $"{invoice.CompanyName}\t" +
                $"{invoice.VatNumber}\t" +
                $"{invoice.InvoiceNumber}\t" +
                $"{invoice.Currency}\t" +
                $"{invoice.NetAmount}\t" +
                $"{invoice.ExchangeRate}\t" +
                $"{invoice.VatAmount}\t" +
                $"{invoice.TransportationCosts}\t" +
                $"{invoice.TotalAmount}\t" +
                $"{invoice.TotalAmountInHomeCurrency}\t" +
                $"{invoice.Country}\t" +
                $"{(invoice.IsEuropeanUnion ? "Y" : "N")}";
            return text;
        }

        public static string GetPositionsText(Invoice invoice)
        {
            var stringDate = invoice.Date.ToString(Format.Date);

            var text = string.Join("\r\n",
                invoice
                    .Positions
                    .Where(position => !string.IsNullOrEmpty(position.Name)
                                       || position.Quantity != 0
                                       || position.Amount != 0)
                    .Select(position =>
                    {
                        var line =
                            stringDate + "\t" +
                            $"{invoice.CompanyName}\t" +
                            $"{invoice.InvoiceNumber}\t" +
                            $"{invoice.Currency}\t" +
                            $"{position.Name}\t" +
                            $"{position.Quantity}\t" +
                            $"{position.Amount}";
                        return line;
                    }));
            return text;
        }
    }
}