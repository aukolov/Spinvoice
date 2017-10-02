using Spinvoice.Application.ViewModels.Invoices;

namespace Spinvoice.Application.ViewModels.FileSystem
{
    public interface IFileViewModel : IFileSystemViewModel
    {
        IInvoiceListViewModel InvoiceListViewModel { get; }
    }
}