using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Spinvoice.Annotations;
using Spinvoice.Services;
using Spinvoice.Utils;

namespace Spinvoice.ViewModels
{
    public class DirectoryViewModel : IFileSystemViewModel
    {
        private bool _isExpanded;
        private bool _isSelected;

        public DirectoryViewModel(
            string path,
            IFileService fileService,
            ISelectedPathListener selectedPathListener)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(path);

            Items = new ObservableCollection<IFileSystemViewModel>();
            Items.AddRange(fileService.GetSubDirectories(path)
                .Select(s => new DirectoryViewModel(s, fileService, selectedPathListener)));
            Items.AddRange(fileService.GetFiles(path)
                .Where(s => System.IO.Path.GetExtension(s) == ".pdf")
                .Select(s => new FileViewModel(s, selectedPathListener)));
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
