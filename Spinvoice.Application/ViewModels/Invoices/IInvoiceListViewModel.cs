using System.Collections.ObjectModel;
using System.ComponentModel;
using Spinvoice.Utils;

namespace Spinvoice.Application.ViewModels.Invoices
{
    public interface IInvoiceListViewModel : INotifyPropertyChanged
    {
        PdfXrayViewModel PdfXrayViewModel { get; }
        RelayCommand AddInvoiceViewModelCommand { get; }
        ObservableCollection<InvoiceViewModel> InvoiceViewModels { get; }
        bool IsLoaded { get; set; }
        FileProcessStatus FileProcessStatus { get; set; }
        void Subscribe();
        void Unsubscribe();
        void Init();
    }
}