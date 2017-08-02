using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using Spinvoice.Domain.App;
using Spinvoice.Domain.ExternalBook;
using Spinvoice.Domain.Pdf;
using Spinvoice.Properties;
using Spinvoice.QuickBooks.ViewModels;
using Spinvoice.Services;
using Spinvoice.Utils;
using Spinvoice.ViewModels.Exchange;
using Spinvoice.ViewModels.FileSystem;
using Spinvoice.ViewModels.Invoices;
using Spinvoice.ViewModels.QuickBooks;

namespace Spinvoice.ViewModels
{
    public sealed class AppViewModel : IAppViewModel
    {
        private readonly WindowManager _windowManager;
        private readonly Dictionary<string, IInvoiceListViewModel> _invoiceListViewModels =
            new Dictionary<string, IInvoiceListViewModel>();

        private readonly IPdfParser _pdfParser;
        private IClipboardService _clipboardService;
        private IInvoiceListViewModel _invoiceListViewModel;
        private readonly IExternalConnectionWatcher _externalConnectionWatcher;

        private readonly Func<PdfModel, IInvoiceListViewModel> _invoiceListViewModelFactory;
        private readonly Func<IExchangeRatesViewModel> _exchangeRatesViewModelFactory;
        private readonly Func<IQuickBooksConnectViewModel> _quickBooksConnectViewModelFactory;
        private readonly Func<IAccountsChartViewModel> _accountsChartViewModelFactory;

        public AppViewModel(
            IAppMetadataRepository appMetadataRepository,
            IFileService fileService,
            IPdfParser pdfParser,
            WindowManager windowManager,
            IExternalConnectionWatcher externalConnectionWatcher,
            Func<IClipboardService> clipboardServiceFactory,
            Func<PdfModel, IInvoiceListViewModel> invoiceListViewModelFactory,
            Func<IExchangeRatesViewModel> exchangeRatesViewModelFactory,
            Func<IQuickBooksConnectViewModel> quickBooksConnectViewModelFactory,
            Func<IAccountsChartViewModel> accountsChartViewModelFactory)
        {
            _pdfParser = pdfParser;

            ProjectBrowserViewModel = new ProjectBrowserViewModel(fileService, appMetadataRepository);
            ProjectBrowserViewModel.SelectedFileChanged += OnCurrentFileChanged;
            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
                {
                    _clipboardService = clipboardServiceFactory();
                    OnCurrentFileChanged();
                },
                DispatcherPriority.Loaded);
            _windowManager = windowManager;
            OpenExchangeRatesCommand = new RelayCommand(OpenExchangeRates);
            OpenQuickBooksCommand = new RelayCommand(OpenQuickBooks);
            OpenChartOfAccountsCommand = new RelayCommand(OpenChartOfAccounts,
                () => _externalConnectionWatcher.IsConnected);

            _externalConnectionWatcher = externalConnectionWatcher;
            _invoiceListViewModelFactory = invoiceListViewModelFactory;
            _exchangeRatesViewModelFactory = exchangeRatesViewModelFactory;
            _quickBooksConnectViewModelFactory = quickBooksConnectViewModelFactory;
            _accountsChartViewModelFactory = accountsChartViewModelFactory;
            _externalConnectionWatcher.Connected += () => OpenChartOfAccountsCommand.RaiseCanExecuteChanged();
        }

        public ICommand OpenExchangeRatesCommand { get; }
        public ICommand OpenQuickBooksCommand { get; }
        public ProjectBrowserViewModel ProjectBrowserViewModel { get; }
        public RelayCommand OpenChartOfAccountsCommand { get; }

        public IInvoiceListViewModel InvoiceListViewModel
        {
            get { return _invoiceListViewModel; }
            set
            {
                if (_invoiceListViewModel == value) return;

                _invoiceListViewModel?.Unsubscribe();
                _invoiceListViewModel = value;
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

            var filePath = ProjectBrowserViewModel.SelectedFilePath;
            if (string.IsNullOrEmpty(filePath))
                return;

            IInvoiceListViewModel invoiceListViewModel;
            if (!_invoiceListViewModels.TryGetValue(filePath, out invoiceListViewModel))
            {
                var pdfModel = _pdfParser.IsPdf(filePath)
                    ? _pdfParser.Parse(filePath)
                    : null;
                invoiceListViewModel = _invoiceListViewModelFactory(pdfModel);
                _invoiceListViewModels[filePath] = invoiceListViewModel;
            }
            InvoiceListViewModel = invoiceListViewModel;
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


        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}