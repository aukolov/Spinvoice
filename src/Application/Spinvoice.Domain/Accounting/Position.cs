using System;
using System.ComponentModel;
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
        private string _externalAccountId;
        private PositionType _positionType;

        public Position()
        {
            _amountChanged = new Subject<Position>();
            AmountChanged = _amountChanged.AsObservable();
        }

        public Position(string name, int quantity, decimal amount) : this()
        {
            Name = name;
            _quantity = quantity;
            _amount = amount;
            _positionType = PositionType.Inventory;
        }

        public IObservable<Position> AmountChanged { get; }

        public string Name
        {
            get { return _name; }
            set
            {
                var name = value;
                if (_name == name) return;
                var prefix = "PART: ";
                if (name != null && name.StartsWith(prefix))
                    name = name.Substring(prefix.Length);
                _name = name;
                OnPropertyChanged();
            }
        }

        public PositionType PositionType
        {
            get { return _positionType; }
            set
            {
                if (_positionType == value) return;
                _positionType = value;
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

        public string ExternalAccountId
        {
            get { return _externalAccountId; }
            set
            {
                if (_externalAccountId == value) return;
                _externalAccountId = value;
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