using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spinvoice.Utils
{
    public sealed class AmountParser
    {
        private static readonly  Regex _amountRegex = new Regex(@"^(?<integral>[ .,0-9]+)(?<delimiter>[,.])(?<decimals>\d{2})$");

        public static decimal Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            var match = _amountRegex.Match(text);
            if (match.Success)
            {
                var delimiter = match.Groups["delimiter"].Value.Single();
                var integralPart = match.Groups["integral"].Value;
                var decimalPart = match.Groups["decimals"].Value;

                if (!integralPart.Contains(delimiter))
                {
                    var parsedIntegralPart = int.Parse(integralPart.Replace(",", "").Replace(".", "").Replace(" ", ""), CultureInfo.InvariantCulture);
                    var parsedDecimalPart = int.Parse(decimalPart, CultureInfo.InvariantCulture);

                    return parsedIntegralPart + parsedDecimalPart / 100.00m;
                }
            }

            decimal value;
            TryParse(text, out value, CultureInfo.InvariantCulture);
            return value;
        }

        private static bool TryParse(string text, out decimal value, CultureInfo culture)
        {
            return decimal.TryParse(text, NumberStyles.Any, culture, out value);
        }
    }
}