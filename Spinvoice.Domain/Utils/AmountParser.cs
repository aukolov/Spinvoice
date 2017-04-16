using System.Globalization;

namespace Spinvoice.Domain.Utils
{
    public sealed class AmountParser
    {
        public static decimal Parse(string text)
        {
            return decimal.Parse(text, CultureInfo.InvariantCulture);
        }
    }
}