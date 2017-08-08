using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using NLog;
using Spinvoice.Domain.App;
using Spinvoice.Services;
using Spinvoice.Utils;

namespace Spinvoice.ViewModels.FileSystem
{
    public sealed class ProjectBrowserViewModel : IProjectBrowserViewModel
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private string _projectDirectoryPath;
        private DirectoryViewModel[] _directoryViewModels;
        private readonly IFileService _fileService;
        private readonly IAppMetadataRepository _appMetadataRepository;
        private string _selectedFilePath;
        private string _selectedPath;

        public event Action SelectedFileChanged;

        public ProjectBrowserViewModel(
            IFileService fileService,
            IAppMetadataRepository appMetadataRepository)
        {
            _fileService = fileService;
            _appMetadataRepository = appMetadataRepository;
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
                    new DirectoryViewModel(_projectDirectoryPath, _fileService, this)
                };
            }
        }

        public string SelectedFilePath
        {
            get { return _selectedFilePath; }
            private set
            {
                if (_selectedFilePath == value) return;
                _selectedFilePath = value;
                OnPropertyChanged();
                SelectedFileChanged.Raise();
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
                    SelectedFilePath = _selectedPath;
                }
                OnPropertyChanged();
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

        public void Dispose()
        {
            AppMetadata appMetadata;
            using (_appMetadataRepository.GetForUpdate(out appMetadata))
            {
                appMetadata.LastProjectPath = ProjectDirectoryPath;
                appMetadata.LastFilePath = SelectedFilePath;
            }
        }
    }
}
