using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spinvoice.Utils
{
    public static class AmountParser
    {
        private static readonly Regex AmountRegex = new Regex(@"^(?<integral>[ .,0-9]+)(?<delimiter>[,.])(?<decimals>\d{2})$");
        private static readonly string[] Currencies = { "USD", "US$", "$", "EUR", "RUB", "RUR", "GBP", "JPY", "CHF", "CHN" };

        public static decimal Parse(string text)
        {
            decimal result;
            TryParse(text, out result);
            return result;
        }

        public static bool TryParse(string text, out decimal amount)
        {
            if (string.IsNullOrEmpty(text))
            {
                {
                    amount = 0;
                    return false;
                }
            }

            var currency = Currencies.FirstOrDefault(text.Contains);
            if (currency != null)
            {
                text = text.Replace(currency, "").Trim();
            }

            if (text.EndsWith(" -"))
            {
                text = text.Remove(text.Length - 2);
            }
            var match = AmountRegex.Match(text);
            if (match.Success)
            {
                var delimiter = match.Groups["delimiter"].Value.Single();
                var integralPart = match.Groups["integral"].Value;
                var decimalPart = match.Groups["decimals"].Value;

                if (!integralPart.Contains(delimiter))
                {
                    var simplifiedIntegralPart = integralPart.Replace(",", "").Replace(".", "").Replace(" ", "");
                    var parsedIntegralPart = simplifiedIntegralPart.Length > 0
                        ? int.Parse(simplifiedIntegralPart, CultureInfo.InvariantCulture)
                        : 0;
                    var parsedDecimalPart = int.Parse(decimalPart, CultureInfo.InvariantCulture);

                    {
                        amount = parsedIntegralPart + parsedDecimalPart / 100.00m;
                        return true;
                    }
                }
            }

            return decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out amount);
        }
    }
}