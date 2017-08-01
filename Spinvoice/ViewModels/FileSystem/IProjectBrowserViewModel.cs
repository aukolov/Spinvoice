using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Spinvoice.ViewModels.FileSystem
{
    public interface IProjectBrowserViewModel : INotifyPropertyChanged, ISelectedPathListener, IDisposable
    {
        event Action SelectedFileChanged;
        string ProjectDirectoryPath { get; }
        string SelectedFilePath { get; }
        ICommand OpenCommand { get; }
        DirectoryViewModel[] DirectoryViewModels { get; }
    }
}