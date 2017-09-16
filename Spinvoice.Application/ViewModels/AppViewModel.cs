using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using Spinvoice.Application.Services;
using Spinvoice.Application.ViewModels.Exchange;
using Spinvoice.Application.ViewModels.FileSystem;
using Spinvoice.Application.ViewModels.Invoices;
using Spinvoice.Application.ViewModels.QuickBooks;
using Spinvoice.Domain.ExternalBook;
using Spinvoice.QuickBooks.ViewModels;
using Spinvoice.Utils;

namespace Spinvoice.Application.ViewModels
{
    public sealed class AppViewModel : IAppViewModel
    {
        private readonly WindowManager _windowManager;

        private IClipboardService _clipboardService;
        private IInvoiceListViewModel _invoiceListViewModel;
        private readonly IExternalConnectionWatcher _externalConnectionWatcher;

        private readonly Func<IExchangeRatesViewModel> _exchangeRatesViewModelFactory;
        private readonly Func<IQuickBooksConnectViewModel> _quickBooksConnectViewModelFactory;
        private readonly Func<IAccountsChartViewModel> _accountsChartViewModelFactory;

        public AppViewModel(
            IProjectBrowserViewModel projectBrowserViewModel,
            WindowManager windowManager,
            IExternalConnectionWatcher externalConnectionWatcher,
            Func<IClipboardService> clipboardServiceFactory,
            Func<IExchangeRatesViewModel> exchangeRatesViewModelFactory,
            Func<IQuickBooksConnectViewModel> quickBooksConnectViewModelFactory,
            Func<IAccountsChartViewModel> accountsChartViewModelFactory)
        {
            ProjectBrowserViewModel = projectBrowserViewModel;
            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
                {
                    _clipboardService = clipboardServiceFactory();
                    OnCurrentFileChanged();
                    ProjectBrowserViewModel.SelectedFileChanged += OnCurrentFileChanged;
                },
                DispatcherPriority.Loaded);
            _windowManager = windowManager;
            OpenExchangeRatesCommand = new RelayCommand(OpenExchangeRates);
            OpenQuickBooksCommand = new RelayCommand(OpenQuickBooks);
            OpenChartOfAccountsCommand = new RelayCommand(OpenChartOfAccounts,
                () => _externalConnectionWatcher.IsConnected);

            _externalConnectionWatcher = externalConnectionWatcher;
            _exchangeRatesViewModelFactory = exchangeRatesViewModelFactory;
            _quickBooksConnectViewModelFactory = quickBooksConnectViewModelFactory;
            _accountsChartViewModelFactory = accountsChartViewModelFactory;
            _externalConnectionWatcher.Connected += () => OpenChartOfAccountsCommand.RaiseCanExecuteChanged();
        }

        public ICommand OpenExchangeRatesCommand { get; }
        public ICommand OpenQuickBooksCommand { get; }
        public IProjectBrowserViewModel ProjectBrowserViewModel { get; }
        public RelayCommand OpenChartOfAccountsCommand { get; }

        public IInvoiceListViewModel InvoiceListViewModel
        {
            get { return _invoiceListViewModel; }
            set
            {
                if (_invoiceListViewModel == value) return;

                _invoiceListViewModel?.Unsubscribe();
                _invoiceListViewModel = value;
                _invoiceListViewModel?.Init();
                _invoiceListViewModel?.Subscribe();
                OnPropertyChanged();
            }
        }
        public void Dispose()
        {
            _clipboardService?.Dispose();
            ProjectBrowserViewModel?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnCurrentFileChanged()
        {
            if (_clipboardService == null)
                return;

            InvoiceListViewModel = ProjectBrowserViewModel.SelectedFileViewModel?.InvoiceListViewModel;
        }

        private void OpenExchangeRates()
        {
            var exchangeRatesViewModel = _exchangeRatesViewModelFactory();
            _windowManager.ShowWindow(exchangeRatesViewModel);
        }

        private void OpenQuickBooks()
        {
            var quickBooksConnectViewModel = _quickBooksConnectViewModelFactory();
            _windowManager.ShowWindow(quickBooksConnectViewModel);
        }

        private void OpenChartOfAccounts()
        {
            var accountsChartViewModel = _accountsChartViewModelFactory();
            _windowManager.ShowDialog(accountsChartViewModel);
        }


        [Domain.Annotations.NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}