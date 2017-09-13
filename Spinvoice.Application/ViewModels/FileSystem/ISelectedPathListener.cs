namespace Spinvoice.Application.ViewModels.FileSystem
{
    public interface ISelectedPathListener
    {
        IFileViewModel SelectedFileViewModel { get; set; }
    }
}