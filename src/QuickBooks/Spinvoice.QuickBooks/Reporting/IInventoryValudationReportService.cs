using System;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.Reporting
{
    public interface IInventoryValudationReportService
    {
        IInventoryValuationItem[] Execute(DateTime date);
    }
}