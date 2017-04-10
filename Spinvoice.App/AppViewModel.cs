using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using Spinvoice.App.Annotations;

namespace Spinvoice.App
{
    public sealed class AppViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly Action[] _commands;

        private ClipboardService _clipboardService;
        private string _clipboardText;
        private int _index;

        public AppViewModel()
        {
            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                _clipboardService = new ClipboardService();
                _clipboardService.ClipboardChanged += OnClipboardChanged;
            }, DispatcherPriority.Loaded);

            Invoice = new Invoice();
            Invoices = new[] {Invoice};

            _commands = new Action[]
            {
                () => ChangeDate(),
                () => ChangeCompanyName(),
                () => ChangeInvoiceNumber(),
                () => ChangeCurrency(),
                () => ChangeNetAmount(),
                () => ChangeVatAmount()
            };
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

        public Invoice Invoice { get; }

        public Invoice[] Invoices { get; private set; }

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
        }

        private void ChangeInvoiceNumber()
        {
            Invoice.InvoiceNumber = ClipboardText;
        }

        private void ChangeCurrency()
        {
            Invoice.Currency = ClipboardText;
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
                    return;
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

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}