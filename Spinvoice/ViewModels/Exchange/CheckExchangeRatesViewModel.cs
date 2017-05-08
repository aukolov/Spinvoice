﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Spinvoice.Annotations;
using Spinvoice.Domain.Exchange;
using Spinvoice.Services;
using Spinvoice.Utils;

namespace Spinvoice.ViewModels.Exchange
{
    public class CheckExchangeRatesViewModel : INotifyPropertyChanged
    {
        private readonly IExchangeRatesRepository _exchangeRatesRepository;
        private readonly ClipboardService _clipboardService;

        private string _currency;
        private DateTime _date;
        public event PropertyChangedEventHandler PropertyChanged;

        public CheckExchangeRatesViewModel(
            IExchangeRatesRepository exchangeRatesRepository,
            ClipboardService clipboardService)
        {
            _currency = "USD";
            _date = new DateTime(2017, 1, 17);
            _exchangeRatesRepository = exchangeRatesRepository;
            _clipboardService = clipboardService;
            CopyToEuroRateCommand = new RelayCommand(() => _clipboardService.TrySetText(
                ToEuroRate.ToString(CultureInfo.InvariantCulture)));
            CopyToCurrnecyRateCommand = new RelayCommand(() => _clipboardService.TrySetText(
                ToCurrencyRate.ToString(CultureInfo.InvariantCulture)));
        }

        public string Currency
        {
            get { return _currency; }
            set
            {
                _currency = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ToCurrencyRate));
                OnPropertyChanged(nameof(ToEuroRate));
            }
        }

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged(nameof(ToCurrencyRate));
                OnPropertyChanged(nameof(ToEuroRate));
            }
        }

        public decimal ToCurrencyRate
        {
            get
            {
                var toEuroRate = ToEuroRate;
                if (toEuroRate == 0) return 0;
                return Math.Round(1 / toEuroRate, 4);
            }
        }

        public decimal ToEuroRate
        {
            get
            {
                if (string.IsNullOrEmpty(Currency)) return 0;
                return _exchangeRatesRepository.GetRate(Currency, Date) ?? 0; 
            }
        }

        public ICommand CopyToEuroRateCommand { get; }
        public ICommand CopyToCurrnecyRateCommand { get; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}