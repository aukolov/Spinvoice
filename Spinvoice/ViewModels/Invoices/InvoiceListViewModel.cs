using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Spinvoice.Annotations;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.Exchange;
using Spinvoice.Domain.ExternalBook;
using Spinvoice.Domain.InvoiceProcessing;
using Spinvoice.Domain.Pdf;
using Spinvoice.Domain.UI;
using Spinvoice.QuickBooks.Account;
using Spinvoice.QuickBooks.Item;
using Spinvoice.Services;
using Spinvoice.Utils;

namespace Spinvoice.ViewModels.Invoices
{
    public class InvoiceListViewModel : INotifyPropertyChanged
    {
        private readonly IAccountsChartRepository _accountsChartRepository;
        private readonly AnalyzeInvoiceService _analyzeInvoiceService;
        private readonly ClipboardService _clipboardService;
        private readonly ICompanyRepository _companyRepository;
        private readonly IExchangeRatesRepository _exchangeRatesRepository;
        private readonly IExternalAccountRepository _externalAccountRepository;
        private readonly IExternalCompanyRepository _externalCompanyRepository;
        private readonly IExternalConnectionWatcher _externalConnectionWatcher;
        private readonly IExternalInvoiceService _externalInvoiceService;
        private readonly IExternalItemRepository _externalItemRepository;
        private readonly TrainStrategyService _trainStrategyService;
        private readonly IWindowManager _windowManager;
        private InvoiceViewModel _lastActive;

        public InvoiceListViewModel(
            ICompanyRepository companyRepository,
            IExchangeRatesRepository exchangeRatesRepository,
            IExternalItemRepository externalItemRepository,
            IAccountsChartRepository accountsChartRepository,
            ClipboardService clipboardService,
            PdfModel pdfModel,
            AnalyzeInvoiceService analyzeInvoiceService,
            TrainStrategyService trainStrategyService,
            IExternalInvoiceService externalInvoiceService,
            IExternalCompanyRepository externalCompanyRepository,
            IExternalAccountRepository externalAccountRepository,
            IExternalConnectionWatcher externalConnectionWatcher,
            IWindowManager windowManager)
        {
            _companyRepository = companyRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
            _externalItemRepository = externalItemRepository;
            _accountsChartRepository = accountsChartRepository;
            _clipboardService = clipboardService;
            _analyzeInvoiceService = analyzeInvoiceService;
            _trainStrategyService = trainStrategyService;
            _externalInvoiceService = externalInvoiceService;
            _externalCompanyRepository = externalCompanyRepository;
            _externalAccountRepository = externalAccountRepository;
            _externalConnectionWatcher = externalConnectionWatcher;
            _windowManager = windowManager;
            InvoiceViewModels = new ObservableCollection<InvoiceViewModel>();
            PdfXrayViewModel = pdfModel != null ? new PdfXrayViewModel(pdfModel) : null;
            AddInvoiceViewModel(pdfModel, PdfXrayViewModel);
            AddInvoiceViewModelCommand = new RelayCommand(() => AddInvoiceViewModel(null, PdfXrayViewModel));
        }

        public PdfXrayViewModel PdfXrayViewModel { get; }
        public RelayCommand AddInvoiceViewModelCommand { get; }
        public ObservableCollection<InvoiceViewModel> InvoiceViewModels { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void AddInvoiceViewModel(PdfModel pdfModel, PdfXrayViewModel pdfXrayViewModel)
        {
            var invoiceViewModel = new InvoiceViewModel(
                _companyRepository,
                _exchangeRatesRepository,
                _externalItemRepository,
                _accountsChartRepository,
                _clipboardService,
                pdfModel,
                pdfXrayViewModel,
                _analyzeInvoiceService,
                _trainStrategyService,
                _externalInvoiceService,
                _externalCompanyRepository,
                _externalAccountRepository,
                _externalConnectionWatcher,
                _windowManager);
            invoiceViewModel.Activated.Subscribe(OnActivated);
            InvoiceViewModels.Add(invoiceViewModel);
            invoiceViewModel.IsActive = true;
        }

        private void OnActivated(InvoiceViewModel invoiceViewModel)
        {
            InvoiceViewModels.Where(vm => vm != invoiceViewModel).ForEach(vm => vm.IsActive = false);
        }

        public void Subscribe()
        {
            if (_lastActive != null && InvoiceViewModels.Contains(_lastActive))
            {
                _lastActive.IsActive = true;
            }
            else if (InvoiceViewModels.Count == 1)
            {
                InvoiceViewModels.Single().IsActive = true;
            }
        }

        public void Unsubscribe()
        {
            _lastActive = InvoiceViewModels.FirstOrDefault(vm => vm.IsActive);
            InvoiceViewModels.ForEach(vm => vm.IsActive = false);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}