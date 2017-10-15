using System;
using System.Globalization;
using System.Windows.Data;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.Application.Views.Converters
{
    public class TrueToCustomerFalseToVendorConverter : IValueConverter
    {
        public static readonly TrueToCustomerFalseToVendorConverter Instance = new TrueToCustomerFalseToVendorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var side = value as Side?;
            return side == null || side == Side.Customer;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = value as bool?;
            return b == null || b.Value ? Side.Customer : Side.Vendor;
        }
    }
}