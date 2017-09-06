using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Spinvoice.Application.ViewModels.FileSystem
{
    public interface IProjectBrowserViewModel : INotifyPropertyChanged, ISelectedPathListener, IDisposable
    {
        event Action SelectedFileChanged;
        string ProjectDirectoryPath { get; }
        ICommand OpenCommand { get; }
        IDirectoryViewModel[] DirectoryViewModels { get; }
        string SelectedFilePath { get; set; }
    }
}