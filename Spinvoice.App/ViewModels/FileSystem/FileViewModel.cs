using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Spinvoice.Application.ViewModels.Invoices;

namespace Spinvoice.Application.ViewModels.FileSystem
{
    public class FileViewModel : IFileViewModel
    {
        private readonly ISelectedPathListener _selectedPathListener;
        private bool _isSelected;

        public FileViewModel(
            string path,
            ISelectedPathListener selectedPathListener,
            Func<string, IInvoiceListViewModel> invoiceListViewModelFactory)
        {
            _selectedPathListener = selectedPathListener;
            Path = path;
            Name = System.IO.Path.GetFileName(path);

            InvoiceListViewModel = invoiceListViewModelFactory(path);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; }
        public string Path { get; }
        public IInvoiceListViewModel InvoiceListViewModel { get; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                if (_isSelected)
                    _selectedPathListener.SelectedFileViewModel = this;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}