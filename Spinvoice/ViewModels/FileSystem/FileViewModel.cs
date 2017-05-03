using System.ComponentModel;
using System.Runtime.CompilerServices;
using Spinvoice.Annotations;

namespace Spinvoice.ViewModels.FileSystem
{
    public class FileViewModel : IFileSystemViewModel
    {
        private readonly ISelectedPathListener _selectedPathListener;
        private bool _isSelected;

        public FileViewModel(string path, ISelectedPathListener selectedPathListener)
        {
            _selectedPathListener = selectedPathListener;
            Path = path;
            Name = System.IO.Path.GetFileName(path);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; }
        public string Path { get; }
        
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                if (_isSelected)
                    _selectedPathListener.SelectedPath = Path;
                OnPropertyChanged();
            }
        }

        public bool IsExpanded { get; set; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}