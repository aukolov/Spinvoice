using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Spinvoice.Application.Views.Converters
{
    public class NullOrEmptyStringToCollapsedConverter : IValueConverter
    {
        public static readonly NullOrEmptyStringToCollapsedConverter Instance = new NullOrEmptyStringToCollapsedConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value as string;
            return string.IsNullOrEmpty(stringValue) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}