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
        private RelayCommand[] _commands;
        private int _index = 0;

        private ClipboardService _clipboardService;
        private string _clipboardText;
        public Invoice Invoice { get; private set; }

        public AppViewModel()
        {
            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                _clipboardService = new ClipboardService();
                _clipboardService.ClipboardChanged += OnClipboardChanged;
            }, DispatcherPriority.Loaded);

            Invoice = new Invoice();
            Invoices = new[] { Invoice };
            ChangeDateCommand = new RelayCommand(ChangeDate);
            ChangeCompanyNameCommand = new RelayCommand(ChangeCompanyName);
            ChangeNetAmountCommand = new RelayCommand(ChangeNetAmount);

            _commands = new[] { ChangeDateCommand, ChangeCompanyNameCommand, ChangeNetAmountCommand };
        }

        private void ChangeNetAmount()
        {
            Invoice.NetAmount = decimal.Parse(ClipboardText);
        }

        private void ChangeCompanyName()
        {
            Invoice.CompanyName = ClipboardText;
        }

        private void ChangeDate()
        {
            Invoice.Date = ParseDate(ClipboardText);
        }

        private DateTime ParseDate(string text)
        {
            DateTime dateTime;
            if (DateTime.TryParseExact(text, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                return dateTime;
            }
            if (DateTime.TryParse(text, out dateTime))
            {
                return dateTime;
            }
            return new DateTime();
        }

        public RelayCommand ChangeCompanyNameCommand { get; private set; }
        public RelayCommand ChangeDateCommand { get; private set; }
        public RelayCommand ChangeNetAmountCommand { get; private set; }

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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnClipboardChanged()
        {
            if (Clipboard.ContainsText())
            {
                var text = Clipboard.GetText();
                if (text == ClipboardText)
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
                _commands[_index].Execute(null);
            }
            catch
            {
            }
            _index = (_index + 1) % _commands.Length;
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            _clipboardService?.Dispose();
        }
    }
}