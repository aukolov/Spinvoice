using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Spinvoice.Application.Services;
using Spinvoice.Utils;

namespace Spinvoice.Application.ViewModels.FileSystem
{
    public class DirectoryViewModel : IDirectoryViewModel
    {
        private static readonly HashSet<string> SupportedExtensions = new HashSet<string> { ".pdf", ".jpg", ".jpeg" };

        private bool _isExpanded;
        private bool _isSelected;

        public DirectoryViewModel(
            string path,
            IFileService fileService,
            ISelectedPathListener selectedPathListener,
            Func<string, ISelectedPathListener, IDirectoryViewModel> directoryViewModelFactory,
            Func<string, ISelectedPathListener, IFileViewModel> fileViewModelFactory)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(path);

            Items = new ObservableCollection<IFileSystemViewModel>();
            Items.AddRange(fileService.GetSubDirectories(path)
                .Select(s => directoryViewModelFactory(s, selectedPathListener)));
            Items.AddRange(fileService.GetFiles(path)
                .Where(s =>
                {
                    var extension = System.IO.Path.GetExtension(s);
                    return extension != null && SupportedExtensions.Contains(extension.ToLower());
                })
                .Select(s => fileViewModelFactory(s, selectedPathListener)));

            AnalyzeCommand = new RelayCommand(Analyze);
        }

        public string Name { get; }

        public string Path { get; }

        public ObservableCollection<IFileSystemViewModel> Items { get; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded == value) return;
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand AnalyzeCommand { get; }

        private void Analyze()
        {
            AnalyzeDirectory(this);
        }

        private static void AnalyzeDirectory(IDirectoryViewModel directoryViewModel)
        {
            foreach (var fileSystemViewModel in directoryViewModel.Items)
            {
                var fileViewModel = fileSystemViewModel as IFileViewModel;
                fileViewModel?.InvoiceListViewModel.Init();

                var subdirectoryViewModel = fileSystemViewModel as IDirectoryViewModel;
                if (subdirectoryViewModel != null)
                {
                    AnalyzeDirectory(subdirectoryViewModel);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
