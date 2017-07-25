using System.ComponentModel;
using System.Runtime.CompilerServices;
using Spinvoice.Annotations;
using Spinvoice.Domain.Accounting;
using Spinvoice.Utils;

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
            RawPosition = new RawPosition();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Position Position { get; }
        public ActionSelectorViewModel ActionSelectorViewModel { get; }
        public RawPosition RawPosition { get; }

        public void ChangePositionName(string text)
        {
            Position.Name = text;
            RawPosition.Name = text;
        }

        public void ChangePositionQuantity(string text)
        {
            int quantity;
            if (QuantityParser.TryParse(text, out quantity))
            {
                Position.Quantity = quantity;
                RawPosition.Quantity = text;
            }
        }

        public void ChangePositionAmount(string text)
        {
            decimal amount;
            if (AmountParser.TryParse(text, out amount))
            {
                Position.Amount = amount;
                RawPosition.Amount = text;
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}