using System;
using System.ComponentModel;
using System.Windows.Input;
using Spinvoice.Application.ViewModels.FileSystem;
using Spinvoice.Application.ViewModels.Invoices;
using Spinvoice.Utils;

namespace Spinvoice.Application.ViewModels
{
    public interface IAppViewModel : INotifyPropertyChanged, IDisposable
    {
        ICommand OpenExchangeRatesCommand { get; }
        ICommand OpenQuickBooksCommand { get; }
        IProjectBrowserViewModel ProjectBrowserViewModel { get; }
        RelayCommand OpenChartOfAccountsCommand { get; }
        IInvoiceListViewModel InvoiceListViewModel { get; set; }
    }
}