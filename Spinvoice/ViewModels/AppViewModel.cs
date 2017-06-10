using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.App;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.Exchange;
using Spinvoice.Domain.ExternalBook;
using Spinvoice.Domain.InvoiceProcessing;
using Spinvoice.Domain.Pdf;
using Spinvoice.Properties;
using Spinvoice.QuickBooks.Account;
using Spinvoice.QuickBooks.Item;
using Spinvoice.QuickBooks.ViewModels;
using Spinvoice.Services;
using Spinvoice.Utils;
using Spinvoice.ViewModels.Exchange;
using Spinvoice.ViewModels.FileSystem;
using Spinvoice.ViewModels.Invoices;
using Spinvoice.ViewModels.QuickBooks;

namespace Spinvoice.ViewModels
{
    public sealed class AppViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly AnalyzeInvoiceService _analyzeInvoiceService;
        private readonly TrainStrategyService _trainStrategyService;
        private readonly WindowManager _windowManager;
        private readonly IOAuthRepository _oauthRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ExchangeRatesLoader _exchangeRatesLoader;
        private readonly IExchangeRatesRepository _exchangeRatesRepository;
        private readonly IAccountsChartRepository _accountsChartRepository;

        private readonly Dictionary<string, InvoiceViewModel> _invoiceViewModels =
            new Dictionary<string, InvoiceViewModel>();

        private readonly IPdfParser _pdfParser;
        private ClipboardService _clipboardService;
        private InvoiceViewModel _invoiceViewModel;
        private readonly IExternalInvoiceService _externalInvoiceService;
        private readonly IExternalCompanyRepository _externalCompanyRepository;
        private readonly IExternalItemRepository _externalItemRepository;
        private readonly IExternalConnectionWatcher _externalConnectionWatcher;
        private readonly IExternalAccountRepository _externalAccountRepository;

        public AppViewModel(
            ICompanyRepository companyRepository,
            IExchangeRatesRepository exchangeRatesRepository,
            IAccountsChartRepository accountsChartRepository,
            IAppMetadataRepository appMetadataRepository,
            ExchangeRatesLoader exchangeRatesLoader,
            IFileService fileService,
            IPdfParser pdfParser,
            AnalyzeInvoiceService analyzeInvoiceService,
            TrainStrategyService trainStrategyService,
            WindowManager windowManager, 
            IOAuthRepository oauthRepository,
            IExternalInvoiceService externalInvoiceService, 
            IExternalCompanyRepository externalCompanyRepository,
            IExternalItemRepository externalItemRepository,
            IExternalConnectionWatcher externalConnectionWatcher,
            IExternalAccountRepository externalAccountRepository)
        {
            _exchangeRatesRepository = exchangeRatesRepository;
            _accountsChartRepository = accountsChartRepository;
            _companyRepository = companyRepository;
            _exchangeRatesLoader = exchangeRatesLoader;
            _pdfParser = pdfParser;

            ProjectBrowserViewModel = new ProjectBrowserViewModel(fileService, appMetadataRepository);
            ProjectBrowserViewModel.SelectedFileChanged += OnCurrentFileChanged;
            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
                {
                    _clipboardService = new ClipboardService();
                    OnCurrentFileChanged();
                },
                DispatcherPriority.Loaded);
            _analyzeInvoiceService = analyzeInvoiceService;
            _trainStrategyService = trainStrategyService;
            _windowManager = windowManager;
            _oauthRepository = oauthRepository;
            OpenExchangeRatesCommand = new RelayCommand(OpenExchangeRates);
            OpenQuickBooksCommand = new RelayCommand(OpenQuickBooks);
            OpenChartOfAccountsCommand = new RelayCommand(OpenChartOfAccounts, 
                () => _externalConnectionWatcher.IsConnected);

            _externalInvoiceService = externalInvoiceService;
            _externalCompanyRepository = externalCompanyRepository;
            _externalItemRepository = externalItemRepository;
            _externalConnectionWatcher = externalConnectionWatcher;
            _externalAccountRepository = externalAccountRepository;
            _externalConnectionWatcher.Connected += () => OpenChartOfAccountsCommand.RaiseCanExecuteChanged();
        }

        public ICommand OpenExchangeRatesCommand { get; }
        public ICommand OpenQuickBooksCommand { get; }
        public ProjectBrowserViewModel ProjectBrowserViewModel { get; }
        public RelayCommand OpenChartOfAccountsCommand { get; }


        public InvoiceViewModel InvoiceViewModel
        {
            get { return _invoiceViewModel; }
            set
            {
                if (_invoiceViewModel == value) return;

                _invoiceViewModel?.Unsubscribe();
                _invoiceViewModel = value;
                _invoiceViewModel?.Subscribe();
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

            InvoiceViewModel invoiceViewModel;
            if (!_invoiceViewModels.TryGetValue(filePath, out invoiceViewModel))
            {
                var pdfModel = _pdfParser.IsPdf(filePath)
                    ? _pdfParser.Parse(filePath)
                    : null;
                invoiceViewModel = new InvoiceViewModel(
                    _companyRepository,
                    _exchangeRatesRepository,
                    _externalItemRepository,
                    _accountsChartRepository,
                    _clipboardService,
                    pdfModel,
                    _analyzeInvoiceService,
                    _trainStrategyService,
                    _externalInvoiceService,
                    _externalCompanyRepository,
                    _externalAccountRepository,
                    _externalConnectionWatcher,
                    _windowManager);
                _invoiceViewModels[filePath] = invoiceViewModel;
            }
            InvoiceViewModel = invoiceViewModel;
        }

        private void OpenExchangeRates()
        {
            var exchangeRatesViewModel = new ExchangeRatesViewModel(
                _exchangeRatesLoader,
                _windowManager,
                _exchangeRatesRepository,
                _clipboardService);
            _windowManager.ShowWindow(exchangeRatesViewModel);
        }

        private void OpenQuickBooks()
        {
            var quickBooksConnectViewModel = new QuickBooksConnectViewModel(
                _oauthRepository, 
                _windowManager);
            _windowManager.ShowWindow(quickBooksConnectViewModel);
        }

        private void OpenChartOfAccounts()
        {
            var accountsChartViewModel = new AccountsChartViewModel(
                _externalAccountRepository,
                _accountsChartRepository,
                _windowManager);
            _windowManager.ShowDialog(accountsChartViewModel);
        }


        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}