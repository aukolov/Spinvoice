using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using NLog;
using Spinvoice.Application.Services;
using Spinvoice.Domain.App;
using Spinvoice.Utils;

namespace Spinvoice.Application.ViewModels.FileSystem
{
    public sealed class ProjectBrowserViewModel : IProjectBrowserViewModel
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private string _projectDirectoryPath;
        private IDirectoryViewModel[] _directoryViewModels;
        private readonly IAppMetadataRepository _appMetadataRepository;
        private readonly Func<string, ISelectedPathListener, IDirectoryViewModel> _directoryViewModelFactory;

        private IFileViewModel _selectedFileViewModel;
        private string _selectedFilePath;

        public event Action SelectedFileChanged;

        public ProjectBrowserViewModel(
            IFileService fileService,
            IAppMetadataRepository appMetadataRepository,
            Func<string, ISelectedPathListener, IDirectoryViewModel> directoryViewModelFactory)
        {
            _appMetadataRepository = appMetadataRepository;
            _directoryViewModelFactory = directoryViewModelFactory;
            OpenCommand = new RelayCommand(OpenDirectoryCommand);

            RestoreState(fileService);
        }

        private void RestoreState(IFileService fileService)
        {
            try
            {
                var appMetadata = _appMetadataRepository.Get();

                if (appMetadata.LastProjectPath == null || !fileService.DirectoryExists(appMetadata.LastProjectPath))
                {
                    return;
                }
                ProjectDirectoryPath = appMetadata.LastProjectPath;
                if (appMetadata.LastFilePath == null || !fileService.FileExists(appMetadata.LastFilePath))
                {
                    return;
                }

                var projectPathParts = appMetadata.LastProjectPath.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                var filePathParts = appMetadata.LastFilePath.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                var i = 0;
                while (i < projectPathParts.Length
                       && i < filePathParts.Length
                       && StringEquals(projectPathParts[i], filePathParts[i]))
                {
                    i++;
                }
                if (i != projectPathParts.Length)
                {
                    return;
                }

                var directoryViewModel = DirectoryViewModels.SingleOrDefault();
                if (directoryViewModel == null)
                {
                    return;
                }
                directoryViewModel.IsExpanded = true;
                while (i < filePathParts.Length - 1)
                {
                    directoryViewModel = directoryViewModel.Items.FirstOrDefault(
                        model => StringEquals(filePathParts[i], model.Name)) as DirectoryViewModel;
                    if (directoryViewModel == null)
                    {
                        return;
                    }
                    directoryViewModel.IsExpanded = true;
                    i++;
                }
                if (i != filePathParts.Length - 1)
                {
                    return;
                }
                var fileViewModel = directoryViewModel.Items.FirstOrDefault(
                    model => StringEquals(filePathParts[i], model.Name)) as FileViewModel;
                if (fileViewModel == null)
                {
                    return;
                }
                fileViewModel.IsSelected = true;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to restore state from app metadata.");
            }
        }

        private static bool StringEquals(string str1, string str2)
        {
            return string.Compare(str1, str2, StringComparison.InvariantCultureIgnoreCase) == 0;
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
                    _directoryViewModelFactory(_projectDirectoryPath, this)
                };
            }
        }

        public string SelectedFilePath
        {
            get { return _selectedFilePath; }
            set
            {
                if (_selectedFilePath == value) return;
                _selectedFilePath = value;
                OnPropertyChanged();
            }
        }

        public IFileViewModel SelectedFileViewModel
        {
            get { return _selectedFileViewModel; }
            set
            {
                if (_selectedFileViewModel == value) return;
                _selectedFileViewModel = value;
                if (_selectedFileViewModel != null)
                {
                    SelectedFilePath = _selectedFileViewModel.Path;
                }
                OnPropertyChanged();
                SelectedFileChanged.Raise();
            }
        }

        private void OpenDirectoryCommand()
        {
            var dialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = false,
                Description = "Select root folder with invoices."
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ProjectDirectoryPath = dialog.SelectedPath;
            }
        }

        public ICommand OpenCommand { get; }

        public IDirectoryViewModel[] DirectoryViewModels
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

        public void Dispose()
        {
            AppMetadata appMetadata;
            using (_appMetadataRepository.GetForUpdate(out appMetadata))
            {
                appMetadata.LastProjectPath = ProjectDirectoryPath;
                if (SelectedFileViewModel != null)
                {
                    appMetadata.LastFilePath = SelectedFileViewModel.Path;
                }
                else
                {
                    appMetadata.LastFilePath = null;
                }
            }
        }
    }
}
