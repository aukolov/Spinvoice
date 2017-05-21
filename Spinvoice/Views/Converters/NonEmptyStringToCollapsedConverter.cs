using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Spinvoice.Views.Converters
{
    public class NonEmptyStringToCollapsedConverter : IValueConverter
    {
        public static readonly NonEmptyStringToCollapsedConverter Instance = new NonEmptyStringToCollapsedConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value as string;
            return string.IsNullOrEmpty(stringValue) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}