using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Spinvoice.Application.Services;
using Spinvoice.Common.Presentation;
using Spinvoice.Domain.Exchange;
using Spinvoice.QuickBooks.Domain;
using Spinvoice.Utils;

namespace Spinvoice.Application.ViewModels.Exchange
{
    public class ExchangeRatesViewModel : IExchangeRatesViewModel
    {
        public ExchangeRatesViewModel(
            ExchangeRatesLoader exchangeRatesLoader, 
            WindowManager windowManager, 
            IExchangeRatesRepository exchangeRatesRepository,
            IExternalCompanyPreferencesRepository externalCompanyPreferencesRepository,
            IClipboardService clipboardService)
        {
            LoadExchangeRatesViewModel = new LoadExchangeRatesViewModel(exchangeRatesLoader);
            CheckExchangeRatesViewModel = new CheckExchangeRatesViewModel(exchangeRatesRepository, externalCompanyPreferencesRepository, clipboardService);
            CloseCommand = new RelayCommand(() => windowManager.Close(this));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public LoadExchangeRatesViewModel LoadExchangeRatesViewModel { get; }
        public ICommand CloseCommand { get; }

        public CheckExchangeRatesViewModel CheckExchangeRatesViewModel { get; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}