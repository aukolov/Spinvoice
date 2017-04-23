using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using Spinvoice.Services;
using Spinvoice.Utils;

namespace Spinvoice.ViewModels
{
    public sealed class ProjectBrowserViewModel : INotifyPropertyChanged, ISelectedPathListener
    {
        private string _projectDirectoryPath;
        private DirectoryViewModel[] _directoryViewModels;
        private readonly IFileService _fileService;
        private string _pdfPath;
        private string _selectedPath;

        public event Action PdfChanged;

        public ProjectBrowserViewModel(IFileService fileService)
        {
            _fileService = fileService;
            OpenCommand = new RelayCommand(OpenDirectoryCommand);
        }

        public string ProjectDirectoryPath
        {
            get { return _projectDirectoryPath; }
            private set
            {
                _projectDirectoryPath = value;
                OnPropertyChanged();
                DirectoryViewModels = new[]
                {
                    new DirectoryViewModel(_projectDirectoryPath, _fileService, this)
                };
            }
        }

        public string PdfPath
        {
            get { return _pdfPath; }
            private set
            {
                if (_pdfPath == value) return;
                _pdfPath = value;
                OnPropertyChanged();
                PdfChanged.Raise();
            }
        }

        public string SelectedPath
        {
            get { return _selectedPath; }
            set
            {
                if (_selectedPath == value) return;
                _selectedPath = value;

                if (_fileService.FileExists(_selectedPath))
                {
                    PdfPath = _selectedPath;
                }
                OnPropertyChanged();
            }
        }

        private void OpenDirectoryCommand()
        {
            var dialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = false,
                Description = "Select root folder with PDF documents."
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ProjectDirectoryPath = dialog.SelectedPath;
            }
        }

        public ICommand OpenCommand { get; }

        public DirectoryViewModel[] DirectoryViewModels
        {
            get { return _directoryViewModels; }
            private set
            {
                _directoryViewModels = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
