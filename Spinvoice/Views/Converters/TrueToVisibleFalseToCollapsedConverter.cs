using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Spinvoice.Views.Converters
{
    public class TrueToVisibleFalseToCollapsedConverter : IValueConverter
    {
        public static readonly TrueToVisibleFalseToCollapsedConverter Instance = new TrueToVisibleFalseToCollapsedConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = value as bool?;
            return boolValue ?? false ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}