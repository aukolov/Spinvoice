namespace Spinvoice.Utils
{
    public class QuantityParser
    {
        public static bool TryParse(string text, out int quantity)
        {
            return int.TryParse(text.Replace(",", ""), out quantity);
        }
    }
}