using System;
using System.Globalization;
using System.Windows.Data;
using Spinvoice.Domain.Accounting;

namespace Spinvoice.Application.Views.Converters
{
    public class TrueToInventoryFalseToServiceConverter : IValueConverter
    {
        public static readonly TrueToInventoryFalseToServiceConverter Instance = new TrueToInventoryFalseToServiceConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var side = value as PositionType?;
            return side == null || side == PositionType.Inventory;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = value as bool?;
            return b == null || b.Value ? PositionType.Inventory : PositionType.Service;
        }
    }
}