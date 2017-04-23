using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spinvoice.Utils
{
    public static class AmountParser
    {
        private static readonly  Regex AmountRegex = new Regex(@"^(?<integral>[ .,0-9]+)(?<delimiter>[,.])(?<decimals>\d{2})$");

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

            var match = AmountRegex.Match(text);
            if (match.Success)
            {
                var delimiter = match.Groups["delimiter"].Value.Single();
                var integralPart = match.Groups["integral"].Value;
                var decimalPart = match.Groups["decimals"].Value;

                if (!integralPart.Contains(delimiter))
                {
                    var parsedIntegralPart = int.Parse(integralPart.Replace(",", "").Replace(".", "").Replace(" ", ""),
                        CultureInfo.InvariantCulture);
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