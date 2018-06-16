using System;
using System.Globalization;
using System.Linq;
using Intuit.Ipp.Data;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.Reporting
{
    public class InventoryValuationReportService : IInventoryValuationReportService
    {
        private readonly IExternalConnection _externalConnection;

        public InventoryValuationReportService(IExternalConnection externalConnection)
        {
            _externalConnection = externalConnection;
        }

        public IInventoryValuationItem[] Execute(DateTime date)
        {
            var report = _externalConnection.GetInventoryValuation(date);
            var items = report.Rows.Select(row => row.AnyIntuitObjects[0])
                .Cast<ColData[]>()
                .Where(x => x[0].id != null)
                .Select(x => new InventoryValuationItem(
                    x[0].id,
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