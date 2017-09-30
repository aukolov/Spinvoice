using System;
using System.Globalization;
using System.Windows.Data;

namespace Spinvoice.Common.Presentation
{
    public class InvertBooleanConverter : IValueConverter
    {
        public static readonly InvertBooleanConverter Instance = new InvertBooleanConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            return !(bool) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return true;

            return !(bool) value;
        }
    }
}