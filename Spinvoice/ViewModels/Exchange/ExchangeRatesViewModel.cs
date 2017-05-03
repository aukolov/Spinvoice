﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Spinvoice.Annotations;
using Spinvoice.Domain.Exchange;
using Spinvoice.Services;
using Spinvoice.Utils;

namespace Spinvoice.ViewModels.Exchange
{
    public class ExchangeRatesViewModel : INotifyPropertyChanged
    {
        public ExchangeRatesViewModel(
            ExchangeRatesLoader exchangeRatesLoader, 
            WindowManager windowManager, 
            IExchangeRatesRepository exchangeRatesRepository,
            ClipboardService clipboardService)
        {
            LoadExchangeRatesViewModel = new LoadExchangeRatesViewModel(exchangeRatesLoader);
            CheckExchangeRatesViewModel = new CheckExchangeRatesViewModel(exchangeRatesRepository, clipboardService);
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