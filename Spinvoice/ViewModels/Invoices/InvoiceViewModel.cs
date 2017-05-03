using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.Exchange;
using Spinvoice.Domain.Pdf;
using Spinvoice.Services;
using Spinvoice.Utils;

namespace Spinvoice.ViewModels.Invoices
{
    public sealed class InvoiceViewModel : INotifyPropertyChanged
    {
        private readonly AnalyzeInvoiceService _analyzeInvoiceService;
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
            AnalyzeInvoiceService analyzeInvoiceService)
        {
            _clipboardService = clipboardService;
            _companyRepository = companyRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
            _pdfModel = pdfModel;
            _analyzeInvoiceService = analyzeInvoiceService;

            CopyCommand = new RelayCommand(CopyToClipboard);
            ClearCommand = new RelayCommand(Clear);
            Invoice = new Invoice();
            Invoice.Positions.Add(new Position());

            ActionSelectorViewModel = new ActionSelectorViewModel();
            PositionListViewModel = new PositionListViewModel(Invoice.Positions, ActionSelectorViewModel);

            analyzeInvoiceService.Analyze(pdfModel, Invoice);
        }

        public ActionSelectorViewModel ActionSelectorViewModel { get; }

        public Invoice Invoice
        {
            get { return _invoice; }
            set
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

        public ICommand CopyCommand { get; }

        public ICommand ClearCommand { get; }

        public PositionListViewModel PositionListViewModel { get; }

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

        private void CopyToClipboard()
        {
            var text =
                $"{Invoice.Date:dd.MM.yyyy}\t" +
                $"{Invoice.CompanyName}\t" +
                $"{Invoice.VatNumber}\t" +
                $"{Invoice.InvoiceNumber}\t" +
                $"{Invoice.Currency}\t" +
                $"{Invoice.NetAmount}\t" +
                $"{Invoice.ExchangeRate}\t" +
                $"{Invoice.NetAmountInEuro}\t" +
                $"{Invoice.VatAmount}\t" +
                $"{Invoice.TotalAmount}\t" +
                $"{Invoice.Country}\t" +
                $"{(Invoice.IsEuropeanUnion ? "Y" : "N")}";

            _textToIgnore = text;
            _clipboardService.TrySetText(text);

            Company company;
            using (_companyRepository.GetByNameForUpdateOrCreate(Invoice.CompanyName, out company))
            {
                company.Country = Invoice.Country;
                company.Currency = Invoice.Currency;
                company.VatNumber = Invoice.VatNumber;
                company.IsEuropeanUnion = Invoice.IsEuropeanUnion;
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

        private void Clear()
        {
            Invoice.Clear();
            ActionSelectorViewModel.EditField = EditField.InvoiceCompany;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}