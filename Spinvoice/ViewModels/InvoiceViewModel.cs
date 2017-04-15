using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Spinvoice.Domain;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.Exchange;
using Spinvoice.Domain.Utils;
using Spinvoice.Services;

namespace Spinvoice.ViewModels
{
    public sealed class InvoiceViewModel : IDisposable, INotifyPropertyChanged
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IExchangeRatesRepository _exchangeRatesRepository;
        private readonly Action[] _commands;
        private ClipboardService _clipboardService;
        private string _textToIgnore;
        private int _index;
        private Invoice _invoice;
        private string _clipboardText;

        public InvoiceViewModel(
            ICompanyRepository companyRepository,
            IExchangeRatesRepository exchangeRatesRepository)
        {
            _companyRepository = companyRepository;
            _exchangeRatesRepository = exchangeRatesRepository;

            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                _clipboardService = new ClipboardService();
                _clipboardService.ClipboardChanged += OnClipboardChanged;
            }, DispatcherPriority.Loaded);

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

        public string ClipboardText
        {
            get { return _clipboardText; }
            set
            {
                if (_clipboardText == value) return;
                _clipboardText = value;
                OnPropertyChanged();
            }
        }

        public ICommand CopyCommand { get; }

        public ICommand ClearCommand { get; }

        private void ChangeDate()
        {
            Invoice.Date = ParseDate(ClipboardText);
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
            Invoice.CompanyName = ClipboardText;

            var company = _companyRepository.GetByName(Invoice.CompanyName);
            if (company != null)
            {
                Invoice.Country = company.Country;
                Invoice.Currency = company.Currency;
                Invoice.VatNumber = company.VatNumber;
                Invoice.IsEuropeanUnion = company.IsEuropeanUnion;
                Index += 2;
            }
        }

        private void ChangeInvoiceNumber()
        {
            Invoice.InvoiceNumber = ClipboardText;
        }

        private void ChangeCurrency()
        {
            Invoice.Currency = ClipboardText;
            UpdateRate();
        }

        private void ChangeCountry()
        {
            Invoice.Country = ClipboardText;
        }

        private void ChangeVatNumber()
        {
            Invoice.VatNumber = ClipboardText;
        }

        private void ChangeNetAmount()
        {
            Invoice.NetAmount = decimal.Parse(ClipboardText, CultureInfo.InvariantCulture);
        }

        private void ChangeVatAmount()
        {
            Invoice.VatAmount = decimal.Parse(ClipboardText, CultureInfo.InvariantCulture);
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
                if (text == ClipboardText)
                {
                    return;
                }
                if (text == _textToIgnore)
                {
                    return;
                }


                ClipboardText = TextDecoder.Decode(text);
            }
            else
            {
                ClipboardText = null;
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

            Company company;
            using (_companyRepository.GetByNameForUpdateOrCreate(Invoice.CompanyName, out company))
            {
                company.Country = Invoice.Country;
                company.Currency = Invoice.Currency;
                company.VatNumber = Invoice.VatNumber;
                company.IsEuropeanUnion = Invoice.IsEuropeanUnion;
            }

            _textToIgnore = text;
            Clipboard.SetText(text);
        }

        private void Clear()
        {
            Invoice = new Invoice();
            Index = 0;
        }

        public void Dispose()
        {
            _clipboardService?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}