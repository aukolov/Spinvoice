using System;
using System.Linq;
using Spinvoice.Domain.Accounting;
using Spinvoice.Utils;

namespace Spinvoice.Infrastructure.DataAccess
{
    public class AccountsChartRepository : IAccountsChartRepository
    {
        private readonly IAccountsChartDataAccess _accountsChartDataAccess;

        public AccountsChartRepository(IAccountsChartDataAccess accountsChartDataAccess)
        {
            _accountsChartDataAccess = accountsChartDataAccess;
            AccountsChart = accountsChartDataAccess.GetAll().SingleOrDefault()
                ?? new AccountsChart();
        }

        public AccountsChart AccountsChart { get; }
        public IDisposable Update()
        {
            return new RelayDisposable(() => _accountsChartDataAccess.AddOrUpdate(AccountsChart));
        }
    }
}