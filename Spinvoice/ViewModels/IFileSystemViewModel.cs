using System.ComponentModel;

namespace Spinvoice.ViewModels
{
    public interface IFileSystemViewModel : INotifyPropertyChanged
    {
        string Name { get; }
        string Path { get; }
        bool IsSelected { get; set; }
        bool IsExpanded { get; set; }
    }
}