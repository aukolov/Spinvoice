using System;
using System.Globalization;
using System.Windows.Data;
using Spinvoice.Domain.Accounting;

namespace Spinvoice.Application.Views.Converters
{
    public class TrueToServiceFalseToInventoryConverter : IValueConverter
    {
        public static readonly TrueToServiceFalseToInventoryConverter Instance = new TrueToServiceFalseToInventoryConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var side = value as PositionType?;
            return side == null || side == PositionType.Service;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = value as bool?;
            return b == null || b.Value ? PositionType.Service : PositionType.Inventory;
        }
    }
}