using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Spinvoice.Services;

namespace Spinvoice.Views.Converters
{
    public class DateToStringConverter : IValueConverter
    {
        public static readonly DateToStringConverter Instance = new DateToStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = value as DateTime?;
            return dateTime?.ToString(Format.Date);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateString = value as string;
            if (dateString == null) return null;
            DateTime date;
            return DateTime.TryParseExact(dateString, Format.Date, CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces, out date)
                ? date
                : DependencyProperty.UnsetValue;
        }
    }
}