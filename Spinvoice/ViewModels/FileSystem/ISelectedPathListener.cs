using Spinvoice.ViewModels.Invoices;

namespace Spinvoice.ViewModels.FileSystem
{
    public interface ISelectedPathListener
    {
        IFileViewModel SelectedFileViewModel { get; set; }
    }
}