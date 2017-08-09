using System.Collections.ObjectModel;

namespace Spinvoice.ViewModels.FileSystem
{
    public interface IDirectoryViewModel : IFileSystemViewModel
    {
        ObservableCollection<IFileSystemViewModel> Items { get; }
        bool IsExpanded { get; set; }
    }
}