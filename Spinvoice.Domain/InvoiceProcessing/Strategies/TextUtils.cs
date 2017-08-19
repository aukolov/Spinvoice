using System.Text.RegularExpressions;

namespace Spinvoice.Domain.InvoiceProcessing.Strategies
{
    internal static class TextUtils
    {
        private static readonly Regex IsNumberRegex = new Regex(@"^[- .,0-9]+$", RegexOptions.Compiled);
        private static readonly Regex EndsWithNumberRegex = new Regex(@"\s[- .,0-9]+$", RegexOptions.Compiled);

        public static bool IsNumber(string text)
        {
            return !string.IsNullOrEmpty(text) && IsNumberRegex.IsMatch(text);
        }

        public static bool EndsWithNumber(string text)
        {
            return !string.IsNullOrEmpty(text) && EndsWithNumberRegex.IsMatch(text);
        }

        public static bool IsNonTrivialString(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            return text.Trim().Length > 0;
        }
    }
}