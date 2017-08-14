using System;
using System.Globalization;
using System.Windows.Data;

namespace Spinvoice.Application.Views.Converters
{
    public class BottomToTopConverter : IMultiValueConverter
    {
        public static readonly BottomToTopConverter Instance = new BottomToTopConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var bottom = (double)values[0];
            var height = (double)values[1];

            return bottom - height;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}