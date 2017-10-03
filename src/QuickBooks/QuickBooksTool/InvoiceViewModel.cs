using System.ComponentModel;
using System.Runtime.CompilerServices;
using Intuit.Ipp.Data;
using QuickBooksTool.Annotations;

namespace QuickBooksTool
{
    public class InvoiceViewModel : INotifyPropertyChanged
    {
        private bool _isSelected;

        public InvoiceViewModel(Bill bill)
        {
            Invoice = bill;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Bill Invoice { get; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public string Description => $"{Invoice.DocNumber} - {Invoice.TxnDate} - {Invoice.TotalAmt}";

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}