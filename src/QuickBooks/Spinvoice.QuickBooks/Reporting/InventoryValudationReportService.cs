using System;
using System.Globalization;
using System.Linq;
using Intuit.Ipp.Data;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.Reporting
{
    public class InventoryValudationReportService : IInventoryValudationReportService
    {
        private readonly IExternalConnection _externalConnection;

        public InventoryValudationReportService(IExternalConnection externalConnection)
        {
            _externalConnection = externalConnection;
        }

        public IInventoryValuationItem[] Execute(DateTime date)
        {
            var report = _externalConnection.GetInventoryValuation(date);
            var items = report.Rows.Select(row => row.AnyIntuitObjects[0])
                .Cast<ColData[]>()
                .Select(x => new InventoryValuationItem(
                    x[0].value,
                    ParseDecimal(x[2]),
                    ParseDecimal(x[3])))
                .Cast<IInventoryValuationItem>()
                .ToArray();
            return items;
        }

        private static decimal ParseDecimal(ColData colData)
        {
            return !string.IsNullOrEmpty(colData.value) 
                ? decimal.Parse(colData.value, CultureInfo.InvariantCulture) 
                : 0;
        }
    }
}