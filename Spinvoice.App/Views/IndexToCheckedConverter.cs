using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Spinvoice.App.Views
{
    public class IndexToCheckedConverter : IValueConverter
    {
        public static readonly IndexToCheckedConverter Instance = new IndexToCheckedConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            if (parameter == null) return false;

            return (int)value == ParseParameter(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            if (parameter == null) return false;

            return (bool)value ? ParseParameter(parameter) : DependencyProperty.UnsetValue;
        }

        private static int ParseParameter(object parameter)
        {
            return int.Parse((string)parameter, CultureInfo.InvariantCulture);
        }
    }
}
