namespace Spinvoice.ViewModels
{
    public class FileViewModel : IFileSystemViewModel
    {
        private readonly ISelectedPathListener _selectedPathListener;
        private string _selectedElement;
        private bool _isSelected;

        public FileViewModel(string path, ISelectedPathListener selectedPathListener)
        {
            _selectedPathListener = selectedPathListener;
            Path = path;
            Name = System.IO.Path.GetFileName(path);
        }

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
                {
                    _selectedPathListener.SelectedPath = Path;
                }
            }
        }
    }
}