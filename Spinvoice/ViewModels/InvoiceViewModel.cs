using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Spinvoice.Domain;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.Exchange;
using Spinvoice.Domain.Pdf;
using Spinvoice.Domain.Utils;
using Spinvoice.Services;

namespace Spinvoice.ViewModels
{
    public sealed class InvoiceViewModel : INotifyPropertyChanged
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IExchangeRatesRepository _exchangeRatesRepository;
        private readonly ClipboardService _clipboardService;
        private readonly AnalyzeInvoiceService _analyzeInvoiceService;

        private readonly Action[] _commands;
        private readonly PdfModel _pdfModel;
        private volatile string _textToIgnore;
        private int _index;
        private Invoice _invoice;
        private string _clipboardText;

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

            _commands = new Action[]
            {
                () => ChangeCompanyName(),
                () => ChangeCountry(),
                () => ChangeCurrency(),
                () => ChangeVatNumber(),
                () => ChangeDate(),
                () => ChangeInvoiceNumber(),
                () => ChangeNetAmount(),
                () => ChangeVatAmount()
            };

            CopyCommand = new RelayCommand(CopyToClipboard);
            ClearCommand = new RelayCommand(Clear);
            Invoice = new Invoice();

            _analyzeInvoiceService.Analyze(pdfModel, Invoice);
        }

        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                OnPropertyChanged();
            }
        }

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

        public void Subscribe()
        {
            _clipboardService.ClipboardChanged += OnClipboardChanged;
        }

        public void Unsubscribe()
        {
            _clipboardService.ClipboardChanged += OnClipboardChanged;
        }

        private void ChangeDate()
        {
            Invoice.Date = ParseDate(_clipboardText);
            UpdateRate();
        }

        private void UpdateRate()
        {
            if (Invoice.Date != default(DateTime) && !string.IsNullOrEmpty(Invoice.Currency))
            {
                Invoice.ExchangeRate = _exchangeRatesRepository.GetRate(Invoice.Currency, Invoice.Date);
            }
        }

        private void ChangeCompanyName()
        {
            Invoice.CompanyName = _clipboardText;

            var company = _companyRepository.GetByName(Invoice.CompanyName);
            if (company != null)
            {
                Invoice.ApplyCompany(company);
                Index += 2;
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
            Invoice.NetAmount = decimal.Parse(_clipboardText, CultureInfo.InvariantCulture);
        }

        private void ChangeVatAmount()
        {
            Invoice.VatAmount = decimal.Parse(_clipboardText, CultureInfo.InvariantCulture);
        }

        private DateTime ParseDate(string text)
        {
            DateTime dateTime;
            if (DateTime.TryParseExact(text, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out dateTime))
                return dateTime;
            if (DateTime.TryParse(text, out dateTime))
                return dateTime;
            return new DateTime();
        }

        private void OnClipboardChanged()
        {
            if (!Application.Current.MainWindow.IsActive)
            {
                return;
            }
            if (Clipboard.ContainsText())
            {
                var text = Clipboard.GetText();
                if (text == _clipboardText)
                {
                    return;
                }
                if (text == _textToIgnore)
                {
                    return;
                }


                string val = TextDecoder.Decode(text);
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
                _commands[Index]();
                Index = (Index + 1) % _commands.Length;
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
                $"{(Invoice.IsEuropeanUnion ? "Y" : "N")}s";

            _textToIgnore = text;
            Clipboard.SetText(text);

            Company company;
            using (_companyRepository.GetByNameForUpdateOrCreate(Invoice.CompanyName, out company))
            {
                company.Country = Invoice.Country;
                company.Currency = Invoice.Currency;
                company.VatNumber = Invoice.VatNumber;
                company.IsEuropeanUnion = Invoice.IsEuropeanUnion;
                if (company.CompanyInvoiceStrategy == null)
                {
                    company.CompanyInvoiceStrategy = new NextTokenStrategy();
                }
                company.CompanyInvoiceStrategy.Study(_pdfModel, company.Name);
                if (company.InvoiceNumberStrategy == null)
                {
                    company.InvoiceNumberStrategy = new NextTokenStrategy();
                }
                company.InvoiceNumberStrategy.Study(_pdfModel, Invoice.InvoiceNumber);
            }
        }

        private void Clear()
        {
            Invoice.Clear();
            Index = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}