using System.ComponentModel;

namespace Spinvoice.ViewModels.FileSystem
{
    public interface IFileSystemViewModel : INotifyPropertyChanged
    {
        string Name { get; }
        string Path { get; }
        bool IsSelected { get; set; }
    }
}