using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Spinvoice.Application.Views.Converters
{
    public class EqualsToCheckedConverter : IValueConverter
    {
        public static readonly EqualsToCheckedConverter Instance = new EqualsToCheckedConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Equals(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;

            return (bool)value ? parameter : DependencyProperty.UnsetValue;
        }
    }
}
