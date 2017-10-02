using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Spinvoice.Application.ViewModels.Invoices
{
    public class ActionSelectorViewModel : INotifyPropertyChanged
    {
        private EditField _editField;
        private readonly int _commandsCount;

        public ActionSelectorViewModel()
        {
            GroupName = Guid.NewGuid().ToString();
            _commandsCount = Enum.GetValues(typeof(EditField)).Length;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string GroupName { get; }

        public EditField EditField
        {
            get { return _editField; }
            set
            {
                if (_editField == value) return;
                _editField = value;
                OnPropertyChanged();
            }
        }

        public void Advance()
        {
            var newField = (int)EditField + 1;
            EditField = newField >= _commandsCount
                ? EditField.PositionName
                : (EditField)newField;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}