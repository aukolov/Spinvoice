using System;
using System.ComponentModel;
using System.Windows.Input;
using Spinvoice.Utils;
using Spinvoice.ViewModels.FileSystem;
using Spinvoice.ViewModels.Invoices;

namespace Spinvoice.ViewModels
{
    public interface IAppViewModel : INotifyPropertyChanged, IDisposable
    {
        ICommand OpenExchangeRatesCommand { get; }
        ICommand OpenQuickBooksCommand { get; }
        ProjectBrowserViewModel ProjectBrowserViewModel { get; }
        RelayCommand OpenChartOfAccountsCommand { get; }
        IInvoiceListViewModel InvoiceListViewModel { get; set; }
    }
}