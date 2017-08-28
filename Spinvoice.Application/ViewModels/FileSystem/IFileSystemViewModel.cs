using System.ComponentModel;

namespace Spinvoice.Application.ViewModels.FileSystem
{
    public interface IFileSystemViewModel : INotifyPropertyChanged
    {
        string Name { get; }
        string Path { get; }
        bool IsSelected { get; set; }
        bool IsExpanded { get; set; }
    }
}