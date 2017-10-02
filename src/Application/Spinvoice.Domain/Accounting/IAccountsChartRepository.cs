using System;

namespace Spinvoice.Domain.Accounting
{
    public interface IAccountsChartRepository
    {
        AccountsChart AccountsChart { get; }
        IDisposable Update();
    }
}
