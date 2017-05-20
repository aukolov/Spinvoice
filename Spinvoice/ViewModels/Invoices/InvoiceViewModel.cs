using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.Exchange;
using Spinvoice.Domain.ExternalBook;
using Spinvoice.Domain.Pdf;
using Spinvoice.QuickBooks.Company;
using Spinvoice.QuickBooks.Invoice;
using Spinvoice.Services;
using Spinvoice.Utils;

namespace Spinvoice.ViewModels.Invoices
{
    public sealed class InvoiceViewModel : INotifyPropertyChanged
    {
        private readonly AnalyzeInvoiceService _analyzeInvoiceService;
        private readonly ExternalInvoiceService _externalInvoiceService;
        private readonly IExternalCompanyService _externalCompanyService;
        private readonly ClipboardService _clipboardService;

        private readonly ICompanyRepository _companyRepository;
        private readonly IExchangeRatesRepository _exchangeRatesRepository;
        private readonly PdfModel _pdfModel;
        private string _clipboardText;
        private Invoice _invoice;
        private string _stringDate;
        private string _stringNetAmount;
        private volatile string _textToIgnore;

        public InvoiceViewModel(
            ICompanyRepository companyRepository,
            IExchangeRatesRepository exchangeRatesRepository,
            ClipboardService clipboardService,
            PdfModel pdfModel,
            AnalyzeInvoiceService analyzeInvoiceService,
            ExternalInvoiceService externalInvoiceService,
            IExternalCompanyService externalCompanyService)
        {
            _clipboardService = clipboardService;
            _companyRepository = companyRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
            _pdfModel = pdfModel;
            _analyzeInvoiceService = analyzeInvoiceService;
            _externalInvoiceService = externalInvoiceService;
            _externalCompanyService = externalCompanyService;

            Invoice = new Invoice();
            Invoice.Positions.Add(new Position());

            CopyInvoiceCommand = new RelayCommand(CopyInvoice);
            CopyPositionsCommand = new RelayCommand(CopyPositions);
            ClearCommand = new RelayCommand(Reset);
            SaveToQuickBooksCommand = new RelayCommand(SaveToQuickBooks);

            ActionSelectorViewModel = new ActionSelectorViewModel();
            PositionListViewModel = new PositionListViewModel(Invoice.Positions, ActionSelectorViewModel);

            ExternalCompanies = externalCompanyService.GetAll();

            if (_pdfModel != null)
            {
                analyzeInvoiceService.Analyze(_pdfModel, Invoice);
            }

            CreateExternalCompanyCommand = new RelayCommand(CreateExternalCompany);
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

            var externalCompany = new ExternalCompany
            {
                Name = Invoice.CompanyName
            };
            _externalCompanyService.Save(externalCompany);
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

        public PositionListViewModel PositionListViewModel { get; }

        public ObservableCollection<IExternalCompany> ExternalCompanies { get; }

        public ICommand CreateExternalCompanyCommand { get; }

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
                PositionListViewModel.SelectedPositionViewModel.Position.Description = _clipboardText;
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
                case EditField.PositionDescription:
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
            _externalInvoiceService.Save(Invoice);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}