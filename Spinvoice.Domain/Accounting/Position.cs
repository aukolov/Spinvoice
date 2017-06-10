using System.ComponentModel;
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

        public Position()
        {
            
        }

        public Position(string name, int quantity, decimal amount)
        {
            _name = name;
            _quantity = quantity;
            _amount = amount;
        }

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