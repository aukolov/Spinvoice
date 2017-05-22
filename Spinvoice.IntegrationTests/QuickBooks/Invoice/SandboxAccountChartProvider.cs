using Spinvoice.Domain.Accounting;

namespace Spinvoice.IntegrationTests.QuickBooks.Invoice
{
    public static class SandboxAccountChartProvider
    {
        public static AccountsChart Get()
        {
            return new AccountsChart
            {
                AssetExternalAccountId = "81",
                ExpenseExternalAccountId = "80",
                IncomeExternalAccountId = "79"
            };
        }
    }
}