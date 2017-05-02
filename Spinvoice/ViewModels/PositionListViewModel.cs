using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Spinvoice.Annotations;
using Spinvoice.Domain.Accounting;
using Spinvoice.Utils;

namespace Spinvoice.ViewModels
{
    public class PositionListViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<Position> _positions;
        private PositionViewModel _selectedPositionViewModel;

        public event PropertyChangedEventHandler PropertyChanged;

        public PositionListViewModel(
            ObservableCollection<Position> positions, 
            ActionSelectorViewModel actionSelectorViewModel)
        {
            _positions = positions;
            Positions = new ListCollectionProxyView<Position, PositionViewModel, ObservableCollection<Position>>(
                positions,
                p => new PositionViewModel(p, actionSelectorViewModel),
                (p, vm) => vm.Position == p);
            AddCommand = new RelayCommand(AddPosition);
            RemoveCommand = new RelayCommand(RemovePosition);
        }

        private void AddPosition()
        {
            var position = new Position();
            _positions.Add(position);
            SelectedPositionViewModel = Positions.ViewModels.FirstOrDefault(vm => vm.Position == position);
        }

        private void RemovePosition()
        {
            if (SelectedPositionViewModel != null)
            {
                _positions.Remove(SelectedPositionViewModel.Position);
            }
        }

        public ListCollectionProxyView<Position, PositionViewModel, ObservableCollection<Position>> Positions { get; }

        public PositionViewModel SelectedPositionViewModel
        {
            get { return _selectedPositionViewModel; }
            set
            {
                if (_selectedPositionViewModel == value) return;
                _selectedPositionViewModel = value;
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