using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using Spinvoice.Domain.Annotations;

namespace Spinvoice.Domain.Accounting
{
    public class Position : INotifyPropertyChanged
    {
        private decimal _amount;
        private string _name;
        private int _quantity;
        private string _externalId;
        private readonly Subject<Position> _amountChanged;

        public Position()
        {
            _amountChanged = new Subject<Position>();
            AmountChanged = _amountChanged.AsObservable();
        }

        public Position(string name, int quantity, decimal amount) : this()
        {
            _name = name;
            _quantity = quantity;
            _amount = amount;
        }

        public IObservable<Position> AmountChanged { get; }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity == value) return;
                _quantity = value;
                OnPropertyChanged();
            }
        }

        public decimal Amount
        {
            get { return _amount; }
            set
            {
                if (_amount == value) return;
                _amount = value;
                _amountChanged.OnNext(this);
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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}