using System.ComponentModel;
using System.Runtime.CompilerServices;
using Spinvoice.Annotations;
using Spinvoice.Domain.Accounting;

namespace Spinvoice.ViewModels.Invoices
{
    public class PositionViewModel : INotifyPropertyChanged
    {
        public PositionViewModel(
            Position position, 
            ActionSelectorViewModel actionSelectorViewModel)
        {
            Position = position;
            ActionSelectorViewModel = actionSelectorViewModel;
        }

        public Position Position { get; }
        public ActionSelectorViewModel ActionSelectorViewModel { get; }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}