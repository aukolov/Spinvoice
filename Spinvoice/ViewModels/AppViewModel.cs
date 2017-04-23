using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.Exchange;
using Spinvoice.Domain.Pdf;
using Spinvoice.Infrastructure.DataAccess;
using Spinvoice.Properties;
using Spinvoice.Services;

namespace Spinvoice.ViewModels
{
    public sealed class AppViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly AnalyzeInvoiceService _analyzeInvoiceService;
        private readonly ICompanyRepository _companyRepository;
        private readonly ExchangeRatesLoader _exchangeRatesLoader;
        private readonly IExchangeRatesRepository _exchangeRatesRepository;

        private readonly Dictionary<string, InvoiceViewModel> _invoiceViewModels =
            new Dictionary<string, InvoiceViewModel>();

        private readonly IPdfParser _pdfParser;
        private ClipboardService _clipboardService;
        private InvoiceViewModel _invoiceViewModel;

        public AppViewModel(
            ICompanyRepository companyRepository,
            IExchangeRatesRepository exchangeRatesRepository,
            AppMetadataRepository appMetadataRepository,
            ExchangeRatesLoader exchangeRatesLoader, 
            IFileService fileService, 
            IPdfParser pdfParser,
            AnalyzeInvoiceService analyzeInvoiceService)
        {
            _exchangeRatesRepository = exchangeRatesRepository;
            _companyRepository = companyRepository;
            _exchangeRatesLoader = exchangeRatesLoader;
            _pdfParser = pdfParser;

            LoadExchangeRatesCommand = new RelayCommand(LoadExchangeRates);

            ProjectBrowserViewModel = new ProjectBrowserViewModel(fileService, appMetadataRepository);
            ProjectBrowserViewModel.PdfChanged += OnPdfChanged;
            Dispatcher.CurrentDispatcher.InvokeAsync(() => { _clipboardService = new ClipboardService(); },
                DispatcherPriority.Loaded);
            _analyzeInvoiceService = analyzeInvoiceService;
        }

        public ICommand LoadExchangeRatesCommand { get; }
        public ProjectBrowserViewModel ProjectBrowserViewModel { get; }

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

        private void OnPdfChanged()
        {
            if (_clipboardService == null)
                return;

            var pdfPath = ProjectBrowserViewModel.PdfPath;
            if (string.IsNullOrEmpty(pdfPath))
                return;

            InvoiceViewModel invoiceViewModel;
            if (!_invoiceViewModels.TryGetValue(pdfPath, out invoiceViewModel))
            {
                invoiceViewModel = new InvoiceViewModel(
                    _companyRepository,
                    _exchangeRatesRepository,
                    _clipboardService,
                    _pdfParser.Parse(pdfPath),
                    _analyzeInvoiceService);
                _invoiceViewModels[pdfPath] = invoiceViewModel;
            }
            InvoiceViewModel = invoiceViewModel;
        }

        private void LoadExchangeRates()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "XML|*.xml"
            };
            if (dialog.ShowDialog() ?? false)
                _exchangeRatesLoader.Load(dialog.FileName);
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}