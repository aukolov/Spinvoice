using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Win32;
using Spinvoice.Application.Services;
using Spinvoice.Utils;

namespace Spinvoice.Application.ViewModels.Exchange
{
    public class LoadExchangeRatesViewModel : INotifyPropertyChanged
    {
        private readonly ExchangeRatesLoader _exchangeRatesLoader;

        public LoadExchangeRatesViewModel(ExchangeRatesLoader exchangeRatesLoader)
        {
            _exchangeRatesLoader = exchangeRatesLoader;
            LoadExchangeRatesCommand = new RelayCommand(LoadExchangeRates);
        }

        public ICommand LoadExchangeRatesCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void LoadExchangeRates()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "XML|*.xml"
            };
            if (dialog.ShowDialog() ?? false)
                _exchangeRatesLoader.Load(dialog.FileName);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}