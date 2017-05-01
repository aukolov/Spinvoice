using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Spinvoice.Annotations;
using Spinvoice.Domain.Accounting;

namespace Spinvoice.ViewModels
{
    public class PositionListViewModel : INotifyPropertyChanged
    {
        private Position _selectedPosition;

        public event PropertyChangedEventHandler PropertyChanged;

        public PositionListViewModel(ObservableCollection<Position> positions)
        {
            Positions = positions;
            AddCommand = new RelayCommand(AddPosition);
            RemoveCommand = new RelayCommand(RemovePosition);
        }

        private void AddPosition()
        {
            var position = new Position();
            Positions.Add(position);
            SelectedPosition = position;
        }

        private void RemovePosition()
        {
            if (SelectedPosition != null)
            {
                Positions.Remove(SelectedPosition);
            }
        }

        public ObservableCollection<Position> Positions { get; }

        public Position SelectedPosition
        {
            get { return _selectedPosition; }
            set
            {
                if (_selectedPosition == value) return;
                _selectedPosition = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}