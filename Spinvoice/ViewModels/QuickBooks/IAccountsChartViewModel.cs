using System.ComponentModel;
using System.Windows.Data;
using Spinvoice.Utils;

namespace Spinvoice.ViewModels.QuickBooks
{
    public interface IAccountsChartViewModel : INotifyPropertyChanged
    {
        ListCollectionView AssetAccounts { get; }
        ListCollectionView ExpenseAccounts { get; }
        ListCollectionView IncomeAccounts { get; }
        ListCollectionView VatAccounts { get; }
        ListCollectionView TransportationCostsAccounts { get; }
        string AssetExternalAccountId { get; set; }
        string ExpenseExternalAccountId { get; set; }
        string IncomeExternalAccountId { get; set; }
        string VatAccountId { get; set; }
        string TransportationCostsAccountId { get; set; }
        RelayCommand SaveCommand { get; }
        RelayCommand CloseCommand { get; }
    }
}