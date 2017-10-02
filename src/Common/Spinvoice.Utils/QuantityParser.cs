namespace Spinvoice.Utils
{
    public static class QuantityParser
    {
        public static bool TryParse(string text, out int quantity)
        {
            return int.TryParse(
                text.Replace(",", "").Replace(".", ""),
                out quantity);
        }
    }
}