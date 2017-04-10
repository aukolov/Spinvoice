using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Spinvoice.App.Annotations;

namespace Spinvoice.App
{
    public class Invoice : INotifyPropertyChanged
    {
        private DateTime _date;
        private string _companyName;
        private string _invoiceNumber;
        private decimal _netAmount;
        private string _currency;
        private decimal _vatAmount;

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged();
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
            }
        }

        public decimal NetAmount
        {
            get { return _netAmount; }
            set
            {
                _netAmount = value;
                OnPropertyChanged();
            }
        }

        public decimal VatAmount
        {
            get { return _vatAmount; }
            set
            {
                _vatAmount = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}