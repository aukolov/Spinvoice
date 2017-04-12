using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Spinvoice.App.Annotations;
using Spinvoice.App.Services;
using Spinvoice.Domain;
using Spinvoice.Domain.Company;

namespace Spinvoice.App.ViewModels
{
    public sealed class AppViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly Action[] _commands;

        private ClipboardService _clipboardService;
        private string _clipboardText;
        private int _index;
        private string _clipboardTextToIgnore;
        private Invoice _invoice;

        public AppViewModel(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                _clipboardService = new ClipboardService();
                _clipboardService.ClipboardChanged += OnClipboardChanged;
            }, DispatcherPriority.Loaded);

            _invoice = new Invoice();

            _commands = new Action[]
            {
                () => ChangeCompanyName(),
                () => ChangeCountry(),
                () => ChangeCurrency(),
                () => ChangeDate(),
                () => {},
                () => ChangeInvoiceNumber(),
                () => ChangeNetAmount(),
                () => ChangeVatAmount()
            };

            CopyCommand = new RelayCommand(CopyToClipboard);
            ClearCommand = new RelayCommand(() => Clear());
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
                _invoice = value;
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

        public void Dispose()
        {
            _clipboardService?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ChangeDate()
        {
            Invoice.Date = ParseDate(ClipboardText);
        }

        private void ChangeCompanyName()
        {
            Invoice.CompanyName = ClipboardText;

            var company = _companyRepository.GetByName(Invoice.CompanyName);
            if (company != null)
            {
                Invoice.Country = company.Country;
                Invoice.Currency = company.Currency;
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
        }

        private void ChangeCountry()
        {
            Invoice.Country = ClipboardText;
        }

        private void ChangeNetAmount()
        {
            Invoice.NetAmount = decimal.Parse(ClipboardText);
        }

        private void ChangeVatAmount()
        {
            Invoice.VatAmount = decimal.Parse(ClipboardText);
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
            if (Clipboard.ContainsText())
            {
                var text = Clipboard.GetText();
                if (text == ClipboardText)
                {
                    return;
                }
                if (text == _clipboardTextToIgnore)
                {
                    return;
                }

                ClipboardText = text;
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
                $"{Invoice.InvoiceNumber}\t" +
                $"{Invoice.Currency}\t" +
                $"{Invoice.NetAmount}\t\t\t" +
                $"{Invoice.VatAmount}";
            _clipboardTextToIgnore = text;

            Company company;
            using (_companyRepository.GetByNameForUpdateOrCreate(Invoice.CompanyName, out company))
            {
                company.Country = Invoice.Country;
                company.Currency = Invoice.Currency;
            }

            Clipboard.SetText(text);
        }

        private void Clear()
        {
            Invoice = new Invoice();
            Index = 0;
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}