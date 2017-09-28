using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using Spinvoice.Common.Presentation;
using Spinvoice.Domain.Accounting;
using Spinvoice.QuickBooks.Account;
using Spinvoice.QuickBooks.Domain;
using Spinvoice.Utils;

namespace Spinvoice.Application.ViewModels.QuickBooks
{
    public class AccountsChartViewModel : IAccountsChartViewModel
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
            var externalAccounts = externalAccountRepository.GetAll();
            AssetAccounts = CreateAccountsView(externalAccounts);
            ExpenseAccounts = CreateAccountsView(externalAccounts);
            IncomeAccounts = CreateAccountsView(externalAccounts);
            VatAccounts = CreateAccountsView(externalAccounts);
            TransportationCostsAccounts = CreateAccountsView(externalAccounts);

            var accountsChart = _accountsChartRepository.AccountsChart;
            AssetExternalAccountId = accountsChart.AssetExternalAccountId;
            ExpenseExternalAccountId = accountsChart.ExpenseExternalAccountId;
            IncomeExternalAccountId = accountsChart.IncomeExternalAccountId;
            VatAccountId = accountsChart.VatAccountId;
            TransportationCostsAccountId = accountsChart.TransportationCostsAccountId;

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(() => windowManager.CloseDialog(this, false));
        }

        public ListCollectionView AssetAccounts { get; }
        public ListCollectionView ExpenseAccounts { get; }
        public ListCollectionView IncomeAccounts { get; }
        public ListCollectionView VatAccounts { get; }
        public ListCollectionView TransportationCostsAccounts { get; }

        private static ListCollectionView CreateAccountsView(ObservableCollection<IExternalAccount> externalAccounts)
        {
            return new ListCollectionView(externalAccounts)
            {
                SortDescriptions = { new SortDescription("Name", ListSortDirection.Ascending) }
            };
        }

        public string AssetExternalAccountId { get; set; }
        public string ExpenseExternalAccountId { get; set; }
        public string IncomeExternalAccountId { get; set; }
        public string VatAccountId { get; set; }
        public string TransportationCostsAccountId { get; set; }

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
                accountsChart.VatAccountId = VatAccountId;
                accountsChart.TransportationCostsAccountId = TransportationCostsAccountId;
            }
            _windowManager.CloseDialog(this, true);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}