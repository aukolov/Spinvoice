using System;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.Reporting
{
    public interface IInventoryValuationReportService
    {
        IInventoryValuationItem[] Execute(DateTime date);
    }
}