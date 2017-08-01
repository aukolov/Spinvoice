using System.Collections.ObjectModel;
using System.ComponentModel;
using Spinvoice.Utils;

namespace Spinvoice.ViewModels.Invoices
{
    public interface IInvoiceListViewModel : INotifyPropertyChanged
    {
        PdfXrayViewModel PdfXrayViewModel { get; }
        RelayCommand AddInvoiceViewModelCommand { get; }
        ObservableCollection<InvoiceViewModel> InvoiceViewModels { get; }
        void Subscribe();
        void Unsubscribe();
    }
}