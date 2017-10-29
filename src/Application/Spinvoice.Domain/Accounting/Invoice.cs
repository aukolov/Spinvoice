using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Spinvoice.QuickBooks.Domain;
using Spinvoice.Utils;

namespace Spinvoice.Domain.Accounting
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
        private bool _isEuropeanUnion;
        private string _vatNumber;
        private string _externalCompanyId;
        private string _externalId;
        private decimal _transportationCosts;
        private Side _side;

        public event Action CurrencyChanged;
        public event Action DateChanged;
        public event Action SideChanged;

        public Invoice()
        {
            Positions = new ObservableCollection<Position>();
            Side = Side.Vendor;
        }

        public Side Side
        {
            get { return _side; }
            set
            {
                if (_side == value) return;
                _side = value;
                OnPropertyChanged();
                SideChanged.Raise();
            }
        }

        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (_date == value) return;
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
                OnOtherPropertyChanged(nameof(TotalAmount));
                OnOtherPropertyChanged(nameof(TotalAmountInHomeCurrency));
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
                OnOtherPropertyChanged(nameof(TotalAmount));
                OnOtherPropertyChanged(nameof(TotalAmountInHomeCurrency));
            }
        }

        public decimal VatAmount
        {
            get { return _vatAmount; }
            set
            {
                _vatAmount = value;
                OnPropertyChanged();
                OnOtherPropertyChanged(nameof(TotalAmount));
                OnOtherPropertyChanged(nameof(TotalAmountInHomeCurrency));
            }
        }

        public decimal TransportationCosts
        {
            get { return _transportationCosts; }
            set
            {
                _transportationCosts = value;
                OnPropertyChanged();
                OnOtherPropertyChanged(nameof(TotalAmount));
                OnOtherPropertyChanged(nameof(TotalAmountInHomeCurrency));
            }
        }

        public decimal TotalAmount => NetAmount + VatAmount + TransportationCosts;
        public decimal TotalAmountInHomeCurrency => Round(TotalAmount * _exchangeRate);

        private static decimal Round(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        public bool IsEuropeanUnion
        {
            get { return _isEuropeanUnion; }
            set
            {
                _isEuropeanUnion = value;
                OnPropertyChanged();
            }
        }

        public string VatNumber
        {
            get { return _vatNumber; }
            set
            {
                _vatNumber = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Position> Positions { get; set; }

        public string ExternalCompanyId
        {
            get { return _externalCompanyId; }
            set
            {
                _externalCompanyId = value;
                OnPropertyChanged();
            }
        }

        public string ExternalId
        {
            get { return _externalId; }
            set
            {
                if (_externalId == value) return;
                _externalId = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnOtherPropertyChanged(propertyName);
        }

        private void OnOtherPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ApplyCompany(Company.Company company)
        {
            CompanyName = company.Name;
            Country = company.Country;
            Currency = company.Currency;
            VatNumber = company.VatNumber;
            IsEuropeanUnion = company.IsEuropeanUnion;
            Side = company.Side;
            ExternalCompanyId = company.ExternalId;
        }

        public void Clear()
        {
            _companyName = null;
            _exchangeRate = 0;
            _country = null;
            _date = new DateTime();
            _currency = null;
            _invoiceNumber = null;
            _netAmount = 0;
            _vatAmount = 0;
            _vatNumber = null;
            _isEuropeanUnion = false;
            _externalCompanyId = null;
            _side = Side.Vendor;

            Positions.Clear();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }
    }
}