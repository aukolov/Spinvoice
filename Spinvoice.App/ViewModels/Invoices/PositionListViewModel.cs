using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Spinvoice.Domain.Accounting;
using Spinvoice.Utils;

namespace Spinvoice.Application.ViewModels.Invoices
{
    public class PositionListViewModel : INotifyPropertyChanged
    {
        private readonly Dictionary<Position, IDisposable> _positionSubscriptions = new Dictionary<Position, IDisposable>();
        private readonly ObservableCollection<Position> _positions;
        private PositionViewModel _selectedPositionViewModel;

        public event PropertyChangedEventHandler PropertyChanged;

        public PositionListViewModel(
            ObservableCollection<Position> positions,
            ActionSelectorViewModel actionSelectorViewModel)
        {
            _positions = positions;
            _positions.CollectionChanged += OnPositionCollectionChanged;
            _positions.ForEach(Subscribe);
            Positions = new ListCollectionProxyView<Position, PositionViewModel, ObservableCollection<Position>>(
                positions,
                p => new PositionViewModel(p, actionSelectorViewModel),
                (p, vm) => vm.Position == p);
            AddCommand = new RelayCommand(AddPosition);
            RemoveCommand = new RelayCommand(RemovePosition);
        }

        private void Subscribe(Position position)
        {
            var subscription = position.AmountChanged.Subscribe(OnAmountChanged);
            _positionSubscriptions.Add(position, subscription);
        }

        public decimal TotalSum => _positions.Sum(position => position.Amount);

        private void OnAmountChanged(Position position)
        {
            OnPropertyChanged(nameof(TotalSum));
        }

        private void OnPositionCollectionChanged(
            object sender,
            NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    args.NewItems.Cast<Position>().ForEach(Subscribe);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    args.OldItems.Cast<Position>().ForEach(position =>
                    {
                        IDisposable subscription;
                        if (_positionSubscriptions.TryGetValue(position, out subscription))
                        {
                            subscription.Dispose();
                        }
                    });
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _positionSubscriptions.Values.ForEach(disposable => disposable.Dispose());
                    _positions.ForEach(Subscribe);
                    break;
            }
            OnPropertyChanged(nameof(TotalSum));
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

        public void ChangePositionName(string text)
        {
            SelectedPositionViewModel?.ChangePositionName(text);
        }

        public void ChangePositionQuantity(string text)
        {
            SelectedPositionViewModel?.ChangePositionQuantity(text);
        }

        public void ChangePositionAmount(string text)
        {
            SelectedPositionViewModel?.ChangePositionAmount(text);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}