using System;
using System.Globalization;

namespace Spinvoice.Domain.Utils
{
    public sealed class DateParser
    {
        public static DateTime? TryParseDate(string text)
        {
            DateTime dateTime;
            if (DateTime.TryParseExact(text, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out dateTime))
                return dateTime;
            if (DateTime.TryParse(text, out dateTime))
                return dateTime;
            return null;
        }
    }
}