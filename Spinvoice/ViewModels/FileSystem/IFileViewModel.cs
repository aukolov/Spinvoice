using Spinvoice.ViewModels.Invoices;

namespace Spinvoice.ViewModels.FileSystem
{
    public interface IFileViewModel : IFileSystemViewModel
    {
        IInvoiceListViewModel InvoiceListViewModel { get; }
    }
}