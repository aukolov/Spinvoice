using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.Exchange;
using Spinvoice.Domain.ExternalBook;
using Spinvoice.Domain.Pdf;
using Spinvoice.Domain.UI;
using Spinvoice.QuickBooks.Account;
using Spinvoice.QuickBooks.Item;
using Spinvoice.QuickBooks.Web;
using Spinvoice.Services;
using Spinvoice.Utils;
using Spinvoice.ViewModels.QuickBooks;

namespace Spinvoice.ViewModels.Invoices
{
    public sealed class InvoiceViewModel : INotifyPropertyChanged
    {
        private readonly AnalyzeInvoiceService _analyzeInvoiceService;
        private readonly IExternalInvoiceService _externalInvoiceService;
        private readonly IExternalCompanyRepository _externalCompanyRepository;
        private readonly IExternalAccountRepository _externalAccountRepository;
        private readonly IWindowManager _windowManager;
        private readonly ClipboardService _clipboardService;

        private readonly ICompanyRepository _companyRepository;
        private readonly IExchangeRatesRepository _exchangeRatesRepository;
        private readonly IExternalItemRepository _externalItemRepository;
        private readonly IAccountsChartRepository _accountsChartRepository;
        private readonly PdfModel _pdfModel;
        private string _clipboardText;
        private Invoice _invoice;
        private string _stringDate;
        private string _stringNetAmount;
        private volatile string _textToIgnore;

        public InvoiceViewModel(
            ICompanyRepository companyRepository,
            IExchangeRatesRepository exchangeRatesRepository,
            IExternalItemRepository externalItemRepository,
            IAccountsChartRepository accountsChartRepository,
            ClipboardService clipboardService,
            PdfModel pdfModel,
            AnalyzeInvoiceService analyzeInvoiceService,
            IExternalInvoiceService externalInvoiceService,
            IExternalCompanyRepository externalCompanyRepository,
            IExternalAccountRepository externalAccountRepository,
            IExternalConnectionWatcher externalConnectionWatcher,
            IWindowManager windowManager)
        {
            _clipboardService = clipboardService;
            _companyRepository = companyRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
            _externalItemRepository = externalItemRepository;
            _accountsChartRepository = accountsChartRepository;
            _pdfModel = pdfModel;
            _analyzeInvoiceService = analyzeInvoiceService;
            _externalInvoiceService = externalInvoiceService;
            _externalCompanyRepository = externalCompanyRepository;
            _externalAccountRepository = externalAccountRepository;
            _windowManager = windowManager;

            Invoice = new Invoice();
            Invoice.Positions.Add(new Position());

            CopyInvoiceCommand = new RelayCommand(CopyInvoice);
            CopyPositionsCommand = new RelayCommand(CopyPositions);
            ClearCommand = new RelayCommand(Reset);
            
            ActionSelectorViewModel = new ActionSelectorViewModel();
            PositionListViewModel = new PositionListViewModel(Invoice.Positions, ActionSelectorViewModel);

            ExternalCompanies = externalCompanyRepository.GetAll();

            if (_pdfModel != null)
            {
                analyzeInvoiceService.Analyze(_pdfModel, Invoice);
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
        }

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

        public ActionSelectorViewModel ActionSelectorViewModel { get; }

        public Invoice Invoice
        {
            get { return _invoice; }
            private set
            {
                if (_invoice != null)
                {
                    _invoice.CurrencyChanged -= UpdateRate;
                    _invoice.DateChanged -= UpdateRate;
                }
                _invoice = value;
                _invoice.CurrencyChanged += UpdateRate;
                _invoice.DateChanged += UpdateRate;

                OnPropertyChanged();
            }
        }

        public ICommand CopyInvoiceCommand { get; }
        public RelayCommand CopyPositionsCommand { get; }
        public ICommand ClearCommand { get; }
        public RelayCommand SaveToQuickBooksCommand { get; set; }
        public RelayCommand OpenInQuickBooksCommand { get; }
        public PositionListViewModel PositionListViewModel { get; }
        public ObservableCollection<IExternalCompany> ExternalCompanies { get; }
        public RelayCommand CreateExternalCompanyCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Subscribe()
        {
            _clipboardService.ClipboardChanged += OnClipboardChanged;
        }

        public void Unsubscribe()
        {
            _clipboardService.ClipboardChanged -= OnClipboardChanged;
        }

        private void ChangeDate()
        {
            var parsedDate = DateParser.TryParseDate(_clipboardText);
            if (parsedDate.HasValue)
            {
                Invoice.Date = parsedDate.Value;
                _stringDate = _clipboardText;
                UpdateRate();
            }
        }

        private void UpdateRate()
        {
            if (Invoice.Date != default(DateTime) && !string.IsNullOrEmpty(Invoice.Currency))
                for (var i = 0; i < 5; i++)
                {
                    var rate = _exchangeRatesRepository.GetRate(Invoice.Currency, Invoice.Date.AddDays(-i));
                    if (!rate.HasValue) continue;

                    Invoice.ExchangeRate = rate.Value;
                    break;
                }
        }

        private void ChangeCompanyName()
        {
            Invoice.CompanyName = _clipboardText;

            var company = _companyRepository.GetByName(Invoice.CompanyName);
            if (company != null)
            {
                Invoice.ApplyCompany(company);
                ActionSelectorViewModel.MoveEditFieldToNext();
                ActionSelectorViewModel.MoveEditFieldToNext();
            }
        }

        private void ChangeInvoiceNumber()
        {
            Invoice.InvoiceNumber = _clipboardText;
        }

        private void ChangeCurrency()
        {
            Invoice.Currency = _clipboardText;
            UpdateRate();
        }

        private void ChangeCountry()
        {
            Invoice.Country = _clipboardText;
        }

        private void ChangeVatNumber()
        {
            Invoice.VatNumber = _clipboardText;
        }

        private void ChangeNetAmount()
        {
            decimal netAmount;
            if (!AmountParser.TryParse(_clipboardText, out netAmount)) return;

            Invoice.NetAmount = netAmount;
            _stringNetAmount = _clipboardText;
        }

        private void ChangeVatAmount()
        {
            Invoice.VatAmount = AmountParser.Parse(_clipboardText);
        }

        private void ChangePositionDescription()
        {
            if (PositionListViewModel.SelectedPositionViewModel != null)
                PositionListViewModel.SelectedPositionViewModel.Position.Name = _clipboardText;
        }

        private void ChangePositionQuantity()
        {
            if (PositionListViewModel.SelectedPositionViewModel == null) return;

            int quantity;
            if (int.TryParse(_clipboardText, out quantity))
            {
                PositionListViewModel.SelectedPositionViewModel.Position.Quantity = quantity;
            }
        }

        private void ChangePositionAmount()
        {
            if (PositionListViewModel.SelectedPositionViewModel != null)
                PositionListViewModel.SelectedPositionViewModel.Position.Amount = AmountParser.Parse(_clipboardText);
        }

        private void OnClipboardChanged()
        {
            if (!Application.Current.MainWindow.IsActive)
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
                ExecuteCurrentCommand();
                if (ActionSelectorViewModel.EditField == EditField.InvoiceVatAmount
                    && PositionListViewModel.SelectedPositionViewModel == null)
                {
                    ActionSelectorViewModel.EditField = EditField.InvoiceCompany;
                }
                else
                {
                    ActionSelectorViewModel.MoveEditFieldToNext();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    Application.Current.MainWindow,
                    "Something went wrong... " + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ExecuteCurrentCommand()
        {
            switch (ActionSelectorViewModel.EditField)
            {
                case EditField.InvoiceCompany:
                    ChangeCompanyName();
                    break;
                case EditField.InvoiceCountry:
                    ChangeCountry();
                    break;
                case EditField.InvoiceCurrency:
                    ChangeCurrency();
                    break;
                case EditField.InvoiceVatNumber:
                    ChangeVatNumber();
                    break;
                case EditField.InvoiceDate:
                    ChangeDate();
                    break;
                case EditField.InvoiceNumber:
                    ChangeInvoiceNumber();
                    break;
                case EditField.InvoiceNetAmount:
                    ChangeNetAmount();
                    break;
                case EditField.InvoiceVatAmount:
                    ChangeVatAmount();
                    break;
                case EditField.PositionName:
                    ChangePositionDescription();
                    break;
                case EditField.PositionQuantity:
                    ChangePositionQuantity();
                    break;
                case EditField.PositionAmount:
                    ChangePositionAmount();
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

        private void TrainAboutCompany()
        {
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
                    var rawInvoice = new RawInvoice
                    {
                        CompanyName = company.Name,
                        Date = _stringDate,
                        InvoiceNumber = Invoice.InvoiceNumber,
                        NetAmount = _stringNetAmount
                    };
                    _analyzeInvoiceService.Learn(company, rawInvoice, _pdfModel);
                }
            }
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
                while (!_accountsChartRepository.AccountsChart.IsComplete )
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