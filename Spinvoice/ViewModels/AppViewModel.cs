﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.Exchange;
using Spinvoice.Domain.ExternalBook;
using Spinvoice.Domain.Pdf;
using Spinvoice.Infrastructure.DataAccess;
using Spinvoice.Properties;
using Spinvoice.QuickBooks.Company;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Invoice;
using Spinvoice.QuickBooks.ViewModels;
using Spinvoice.Services;
using Spinvoice.Utils;
using Spinvoice.ViewModels.Exchange;
using Spinvoice.ViewModels.FileSystem;
using Spinvoice.ViewModels.Invoices;

namespace Spinvoice.ViewModels
{
    public sealed class AppViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly AnalyzeInvoiceService _analyzeInvoiceService;
        private readonly WindowManager _windowManager;
        private readonly IOAuthRepository _oauthRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ExchangeRatesLoader _exchangeRatesLoader;
        private readonly IExchangeRatesRepository _exchangeRatesRepository;

        private readonly Dictionary<string, InvoiceViewModel> _invoiceViewModels =
            new Dictionary<string, InvoiceViewModel>();

        private readonly IPdfParser _pdfParser;
        private ClipboardService _clipboardService;
        private InvoiceViewModel _invoiceViewModel;
        private readonly ExternalInvoiceService _externalInvoiceService;
        private readonly ExternalCompanyService _externalCompanyService;

        public AppViewModel(
            ICompanyRepository companyRepository,
            IExchangeRatesRepository exchangeRatesRepository,
            AppMetadataRepository appMetadataRepository,
            ExchangeRatesLoader exchangeRatesLoader,
            IFileService fileService,
            IPdfParser pdfParser,
            AnalyzeInvoiceService analyzeInvoiceService,
            WindowManager windowManager, 
            IOAuthRepository oauthRepository,
            ExternalInvoiceService externalInvoiceService, 
            ExternalCompanyService externalCompanyService)
        {
            _exchangeRatesRepository = exchangeRatesRepository;
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
            _windowManager = windowManager;
            _oauthRepository = oauthRepository;
            OpenExchangeRatesCommand = new RelayCommand(OpenExchangeRates);
            OpenQuickBooksCommand = new RelayCommand(OpenQuickBooks);
            _externalInvoiceService = externalInvoiceService;
            _externalCompanyService = externalCompanyService;
        }

        public ICommand OpenExchangeRatesCommand { get; }
        public ICommand OpenQuickBooksCommand { get; }
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
                    _clipboardService,
                    pdfModel,
                    _analyzeInvoiceService,
                    _externalInvoiceService,
                    _externalCompanyService);
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

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}