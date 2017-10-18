using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Spinvoice.Domain.Accounting;

namespace Spinvoice.Application.Views.Converters
{
    public class ServiceToVisibleOtherviseCollapsedConverter : IValueConverter
    {
        public static readonly ServiceToVisibleOtherviseCollapsedConverter Instance = new ServiceToVisibleOtherviseCollapsedConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var side = value as PositionType?;
            return side == null || side == PositionType.Service ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}