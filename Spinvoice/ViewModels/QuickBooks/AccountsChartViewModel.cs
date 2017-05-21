using System.Collections.ObjectModel;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.ExternalBook;
using Spinvoice.Domain.UI;
using Spinvoice.QuickBooks.Account;
using Spinvoice.Utils;

namespace Spinvoice.ViewModels.QuickBooks
{
    public class AccountsChartViewModel
    {
        private readonly IAccountsChartRepository _accountsChartRepository;
        private readonly IWindowManager _windowManager;

        public AccountsChartViewModel(
            IExternalAccountRepository externalAccountRepository,
            IAccountsChartRepository accountsChartRepository,
            IWindowManager windowManager)
        {
            _accountsChartRepository = accountsChartRepository;
            _windowManager = windowManager;
            ExternalAccounts = externalAccountRepository.GetAll();

            var accountsChart = _accountsChartRepository.AccountsChart;
            AssetExternalAccountId = accountsChart.AssetExternalAccountId;
            ExpenseExternalAccountId = accountsChart.ExpenseExternalAccountId;
            IncomeExternalAccountId = accountsChart.IncomeExternalAccountId;

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(() => windowManager.CloseDialog(this, false));
        }

        public ObservableCollection<IExternalAccount> ExternalAccounts { get; }

        public string AssetExternalAccountId { get; set; }
        public string ExpenseExternalAccountId { get; set; }
        public string IncomeExternalAccountId { get; set; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }

        private void Save()
        {
            using (_accountsChartRepository.Update())
            {
                var accountsChart = _accountsChartRepository.AccountsChart;
                accountsChart.AssetExternalAccountId = AssetExternalAccountId;
                accountsChart.ExpenseExternalAccountId = ExpenseExternalAccountId;
                accountsChart.IncomeExternalAccountId = IncomeExternalAccountId;
            }
            _windowManager.CloseDialog(this, true);
        }

    }
}