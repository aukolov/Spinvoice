using System.ComponentModel;
using System.Runtime.CompilerServices;
using Spinvoice.Domain.Annotations;

namespace Spinvoice.Domain.Accounting
{
    public class Position : INotifyPropertyChanged
    {
        private string _description;
        private int _quantity;
        private decimal _amount;

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description == value) return;
                _description = value;
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}