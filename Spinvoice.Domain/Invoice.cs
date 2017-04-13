using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Spinvoice.Domain.Utils;

namespace Spinvoice.Domain
{
    public class Invoice : INotifyPropertyChanged
    {
        private DateTime _date;
        private string _companyName;
        private string _invoiceNumber;
        private decimal _netAmount;
        private string _currency;
        private decimal _vatAmount;
        private string _country;
        private decimal _exchangeRate;

        public event Action CurrencyChanged;
        public event Action DateChanged;

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged();
                DateChanged.Raise();
            }
        }

        public string CompanyName
        {
            get { return _companyName; }
            set
            {
                _companyName = value;
                OnPropertyChanged();
            }
        }

        public string InvoiceNumber
        {
            get { return _invoiceNumber; }

            set
            {
                _invoiceNumber = value;
                OnPropertyChanged();
            }
        }

        public string Currency
        {
            get { return _currency; }
            set
            {
                _currency = value;
                OnPropertyChanged();
                CurrencyChanged.Raise();
            }
        }

        public decimal ExchangeRate
        {
            get { return _exchangeRate; }
            set
            {
                _exchangeRate = value;
                OnPropertyChanged();
                OnOtherPropertyChanged(nameof(NetAmountInEuro));
                OnOtherPropertyChanged(nameof(TotalAmount));
            }
        }

        public string Country
        {
            get { return _country; }
            set
            {
                _country = value;
                OnPropertyChanged();
            }
        }

        public decimal NetAmount
        {
            get { return _netAmount; }
            set
            {
                _netAmount = value;
                OnPropertyChanged();
                OnOtherPropertyChanged(nameof(NetAmountInEuro));
                OnOtherPropertyChanged(nameof(TotalAmount));
            }
        }

        public decimal NetAmountInEuro => _netAmount * _exchangeRate;

        public decimal VatAmount
        {
            get { return _vatAmount; }
            set
            {
                _vatAmount = value;
                OnPropertyChanged();
                OnOtherPropertyChanged(nameof(TotalAmount));
            }
        }

        public decimal TotalAmount => NetAmountInEuro + VatAmount;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnOtherPropertyChanged(propertyName);
        }

        private void OnOtherPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}