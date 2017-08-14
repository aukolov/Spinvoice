using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Windows;
using NLog;
using Spinvoice.Application.Services;
using Spinvoice.Application.ViewModels.QuickBooks;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.Exchange;
using Spinvoice.Domain.ExternalBook;
using Spinvoice.Domain.InvoiceProcessing;
using Spinvoice.Domain.Pdf;
using Spinvoice.Domain.UI;
using Spinvoice.QuickBooks.Account;
using Spinvoice.QuickBooks.Item;
using Spinvoice.QuickBooks.Web;
using Spinvoice.Utils;

namespace Spinvoice.Application.ViewModels.Invoices
{
    public sealed class InvoiceViewModel : INotifyPropertyChanged
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly AnalyzeInvoiceService _analyzeInvoiceService;
        private readonly TrainStrategyService _trainStrategyService;
        private readonly IExternalInvoiceService _externalInvoiceService;
        private readonly IExternalCompanyRepository _externalCompanyRepository;
        private readonly IExternalAccountRepository _externalAccountRepository;
        private readonly IWindowManager _windowManager;
        private readonly ClipboardService _clipboardService;

        private readonly ICompanyRepository _companyRepository;
        private readonly IExternalItemRepository _externalItemRepository;
        private readonly IAccountsChartRepository _accountsChartRepository;
        private readonly PdfModel _pdfModel;
        private string _clipboardText;
        private volatile string _textToIgnore;
        private readonly RawInvoice _rawInvoice;
        private readonly bool _positionsAnalyzed;
        private bool _isActive;
        private readonly ISubject<InvoiceViewModel> _activated;
        private PdfXrayViewModel _pdfXrayViewModel;

        public InvoiceViewModel(
            ICompanyRepository companyRepository,
            IExchangeRatesRepository exchangeRatesRepository,
            IExternalItemRepository externalItemRepository,
            IAccountsChartRepository accountsChartRepository,
            ClipboardService clipboardService,
            PdfModel pdfModel,
            PdfXrayViewModel pdfXrayViewModel,
            AnalyzeInvoiceService analyzeInvoiceService,
            TrainStrategyService trainStrategyService,
            IExternalInvoiceService externalInvoiceService,
            IExternalCompanyRepository externalCompanyRepository,
            IExternalAccountRepository externalAccountRepository,
            IExternalConnectionWatcher externalConnectionWatcher,
            IWindowManager windowManager)
        {
            _clipboardService = clipboardService;
            _companyRepository = companyRepository;
            _externalItemRepository = externalItemRepository;
            _accountsChartRepository = accountsChartRepository;
            _pdfModel = pdfModel;
            _analyzeInvoiceService = analyzeInvoiceService;
            _trainStrategyService = trainStrategyService;

            _externalInvoiceService = externalInvoiceService;
            _externalCompanyRepository = externalCompanyRepository;
            _externalAccountRepository = externalAccountRepository;
            _windowManager = windowManager;

            Invoice = new Invoice();
            _rawInvoice = new RawInvoice();

            CopyInvoiceCommand = new RelayCommand(CopyInvoice);
            CopyPositionsCommand = new RelayCommand(CopyPositions);
            ClearCommand = new RelayCommand(Reset);

            ActionSelectorViewModel = new ActionSelectorViewModel();
            PositionListViewModel = new PositionListViewModel(Invoice.Positions, ActionSelectorViewModel);
            InvoiceEditViewModel = new InvoiceEditViewModel(
                Invoice,
                ActionSelectorViewModel,
                _rawInvoice,
                companyRepository,
                exchangeRatesRepository);

            _pdfXrayViewModel = pdfXrayViewModel;

            ExternalCompanies = externalCompanyRepository.GetAll();

            if (_pdfModel != null)
            {
                analyzeInvoiceService.Analyze(_pdfModel, Invoice);
            }
            _positionsAnalyzed = Invoice.Positions.Any();
            if (!Invoice.Positions.Any())
            {
                Invoice.Positions.Add(new Position());
            }

            SaveToQuickBooksCommand = new RelayCommand(SaveToQuickBooks,
                () => externalConnectionWatcher.IsConnected);
            OpenInQuickBooksCommand = new RelayCommand(OpenInQuickBooks);
            CreateExternalCompanyCommand = new RelayCommand(
                CreateExternalCompany,
                () => externalConnectionWatcher.IsConnected);

            externalConnectionWatcher.Connected += () =>
            {
                CreateExternalCompanyCommand.RaiseCanExecuteChanged();
                SaveToQuickBooksCommand.RaiseCanExecuteChanged();
            };

            _activated = new Subject<InvoiceViewModel>();
        }

        public Invoice Invoice { get; }

        public InvoiceEditViewModel InvoiceEditViewModel { get; }
        public ActionSelectorViewModel ActionSelectorViewModel { get; }
        public PositionListViewModel PositionListViewModel { get; }
        public ObservableCollection<IExternalCompany> ExternalCompanies { get; }

        public IObservable<InvoiceViewModel> Activated => _activated;

        public RelayCommand CopyInvoiceCommand { get; }
        public RelayCommand CopyPositionsCommand { get; }
        public RelayCommand ClearCommand { get; }
        public RelayCommand SaveToQuickBooksCommand { get; set; }
        public RelayCommand OpenInQuickBooksCommand { get; }
        public RelayCommand CreateExternalCompanyCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void CreateExternalCompany()
        {
            if (string.IsNullOrEmpty(Invoice.CompanyName))
            {
                return;
            }
            if (ExternalCompanies.Any(company => company.Name == Invoice.CompanyName))
            {
                return;
            }

            var externalCompany = _externalCompanyRepository.Create(
                Invoice.CompanyName,
                Invoice.Currency);
            Invoice.ExternalCompanyId = externalCompany.Id;
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive == value) return;
                _isActive = value;
                if (_isActive)
                {
                    _activated.OnNext(this);
                    _clipboardService.ClipboardChanged += OnClipboardChanged;
                    if (_pdfXrayViewModel != null)
                    {
                        _pdfXrayViewModel.TextClicked += OnXrayTextClicked;
                    }
                }
                else
                {
                    _clipboardService.ClipboardChanged -= OnClipboardChanged;
                    if (_pdfXrayViewModel != null)
                    {
                        _pdfXrayViewModel.TextClicked -= OnXrayTextClicked;
                    }
                }
                OnPropertyChanged();
            }
        }

        private void OnXrayTextClicked(SentenceModel sentence)
        {
            try
            {
                ExecuteCurrentCommand(sentence.Text);
                AdvanceCommand();
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        private void OnClipboardChanged()
        {
            if (!System.Windows.Application.Current.MainWindow.IsActive)
                return;

            if (_clipboardService.CheckContainsText())
            {
                var text = _clipboardService.GetText();
                if (text == _clipboardText)
                    return;
                if (text == _textToIgnore)
                    return;


                var val = TextDecoder.Decode(text);
                if (_clipboardText == val) return;
                _clipboardText = val;
                OnPropertyChanged();
            }
            else
            {
                if (_clipboardText == null) return;
                _clipboardText = null;
                OnPropertyChanged();
                return;
            }

            try
            {
                ExecuteCurrentCommand(_clipboardText);
                AdvanceCommand();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private static void ShowError(Exception ex)
        {
            MessageBox.Show(
                System.Windows.Application.Current.MainWindow,
                "Something went wrong... " + ex.Message,
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private void AdvanceCommand()
        {
            if (ActionSelectorViewModel.EditField == EditField.InvoiceTransportationCosts)
            {
                if (Invoice.Positions.Any())
                {
                    if (PositionListViewModel.SelectedPositionViewModel == null)
                    {
                        PositionListViewModel.Positions.MoveCurrentToFirst();
                    }
                    ActionSelectorViewModel.Advance();
                }
                else
                {
                    ActionSelectorViewModel.EditField = EditField.InvoiceCompany;
                }
            }
            else
            {
                if (ActionSelectorViewModel.EditField == EditField.PositionAmount)
                {
                    var firstPositionViewModel = PositionListViewModel.Positions.ViewModels.FirstOrDefault();
                    if (Invoice.Positions.Count == 1
                        && !_positionsAnalyzed
                        && firstPositionViewModel != null
                        && firstPositionViewModel.RawPosition.IsFullyInitialized
                        && _pdfModel != null)
                    {
                        var company = TrainAboutPositions();
                        _analyzeInvoiceService.AnalyzePositions(_pdfModel, Invoice, company);
                    }
                    else
                    {
                        Invoice.Positions.Add(new Position());
                        PositionListViewModel.Positions.MoveCurrentToLast();
                    }
                }
                ActionSelectorViewModel.Advance();
            }
        }

        private void ExecuteCurrentCommand(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            text = text.Trim();

            switch (ActionSelectorViewModel.EditField)
            {
                case EditField.InvoiceCompany:
                    InvoiceEditViewModel.ChangeCompanyName(text);
                    break;
                case EditField.InvoiceCountry:
                    InvoiceEditViewModel.ChangeCountry(text);
                    break;
                case EditField.InvoiceCurrency:
                    InvoiceEditViewModel.ChangeCurrency(text);
                    break;
                case EditField.InvoiceVatNumber:
                    InvoiceEditViewModel.ChangeVatNumber(text);
                    break;
                case EditField.InvoiceDate:
                    InvoiceEditViewModel.ChangeDate(text);
                    break;
                case EditField.InvoiceNumber:
                    InvoiceEditViewModel.ChangeInvoiceNumber(text);
                    break;
                case EditField.InvoiceNetAmount:
                    InvoiceEditViewModel.ChangeNetAmount(text);
                    break;
                case EditField.InvoiceVatAmount:
                    InvoiceEditViewModel.ChangeVatAmount(text);
                    break;
                case EditField.InvoiceTransportationCosts:
                    InvoiceEditViewModel.ChangeTransportationCosts(text);
                    break;
                case EditField.PositionName:
                    PositionListViewModel.ChangePositionName(text);
                    break;
                case EditField.PositionQuantity:
                    PositionListViewModel.ChangePositionQuantity(text);
                    break;
                case EditField.PositionAmount:
                    PositionListViewModel.ChangePositionAmount(text);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CopyInvoice()
        {
            var text = InvoiceFormatter.GetInvoiceText(Invoice);
            CopyToClipboard(text);
            TrainAboutCompany();
        }

        private void CopyPositions()
        {
            var text = InvoiceFormatter.GetPositionsText(Invoice);
            CopyToClipboard(text);
            TrainAboutCompany();
        }

        private void CopyToClipboard(string text)
        {
            _textToIgnore = text;
            _clipboardService.TrySetText(text);
        }

        private RawPosition GetFirstRawPosition()
        {
            var firstPositionViewModel = PositionListViewModel.Positions.ViewModels.FirstOrDefault();
            RawPosition rawPosition = null;
            if (firstPositionViewModel != null)
            {
                rawPosition = firstPositionViewModel.RawPosition;
            }
            return rawPosition;
        }

        private void TrainAboutCompany()
        {
            Logger.Info($"Start training company '{Invoice.CompanyName}'.");
            Company company;
            using (_companyRepository.GetByNameForUpdateOrCreate(Invoice.CompanyName, out company))
            {
                company.Country = Invoice.Country;
                company.Currency = Invoice.Currency;
                company.VatNumber = Invoice.VatNumber;
                company.IsEuropeanUnion = Invoice.IsEuropeanUnion;
                company.ExternalId = Invoice.ExternalCompanyId;

                if (_pdfModel != null)
                {
                    _rawInvoice.InvoiceNumber = _rawInvoice.InvoiceNumber ?? Invoice.InvoiceNumber;
                    _rawInvoice.CompanyName = _rawInvoice.CompanyName ?? Invoice.CompanyName;
                    _rawInvoice.FirstPosition = GetFirstRawPosition();

                    _trainStrategyService.Train(company, _rawInvoice, _pdfModel);
                }
            }
        }

        private Company TrainAboutPositions()
        {
            Company company;
            using (_companyRepository.GetByNameForUpdateOrCreate(Invoice.CompanyName, out company))
            {
                var rawPosition = GetFirstRawPosition();
                if (_pdfModel != null)
                {
                    _trainStrategyService.Train(company, rawPosition, _pdfModel);
                }
            }
            return company;
        }

        private void Reset()
        {
            Invoice.Clear();
            Invoice.Positions.Add(new Position());
            ActionSelectorViewModel.EditField = EditField.InvoiceCompany;
        }

        private void SaveToQuickBooks()
        {
            var invoicePositions = Invoice.Positions
                .Where(position => !string.IsNullOrEmpty(position.Name))
                .ToArray();
            if (invoicePositions.Any())
            {
                while (!_accountsChartRepository.AccountsChart.IsComplete)
                {
                    var dialogResult = _windowManager.ShowDialog(
                                           new AccountsChartViewModel(
                                               _externalAccountRepository,
                                               _accountsChartRepository,
                                               _windowManager)) ?? false;
                    if (!dialogResult)
                    {
                        return;
                    }
                }
            }
            foreach (var invoicePosition in invoicePositions)
            {
                var externalItem = _externalItemRepository.Get(invoicePosition.Name)
                    ?? _externalItemRepository.Add(invoicePosition.Name);
                invoicePosition.ExternalId = externalItem.Id;
            }
            var externalInvoiceId = _externalInvoiceService.Save(Invoice);
            Invoice.ExternalId = externalInvoiceId;

            TrainAboutCompany();
        }

        private void OpenInQuickBooks()
        {
            if (string.IsNullOrEmpty(Invoice.ExternalId))
            {
                return;
            }

            Process.Start(QuickBooksUrlService.GetExternalInviceUrl(Invoice.ExternalId));
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}