using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Spinvoice.Annotations;
using Spinvoice.Domain.Pdf;
using Spinvoice.Utils;

namespace Spinvoice.ViewModels.Invoices
{
    public class InvoiceListViewModel : IInvoiceListViewModel
    {
        private readonly Func<PdfModel, PdfXrayViewModel, InvoiceViewModel> _invoiceViewModelFactory;
        private InvoiceViewModel _lastActive;

        public InvoiceListViewModel(
            PdfModel pdfModel,
            Func<PdfModel, PdfXrayViewModel, InvoiceViewModel> invoiceViewModelFactory)
        {
            _invoiceViewModelFactory = invoiceViewModelFactory;
            InvoiceViewModels = new ObservableCollection<InvoiceViewModel>();
            PdfXrayViewModel = pdfModel != null ? new PdfXrayViewModel(pdfModel) : null;
            AddInvoiceViewModel(pdfModel, PdfXrayViewModel);
            AddInvoiceViewModelCommand = new RelayCommand(() => AddInvoiceViewModel(null, PdfXrayViewModel));
        }

        public PdfXrayViewModel PdfXrayViewModel { get; }
        public RelayCommand AddInvoiceViewModelCommand { get; }
        public ObservableCollection<InvoiceViewModel> InvoiceViewModels { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void AddInvoiceViewModel(PdfModel pdfModel, PdfXrayViewModel pdfXrayViewModel)
        {
            var invoiceViewModel = _invoiceViewModelFactory(
                pdfModel,
                pdfXrayViewModel);
            invoiceViewModel.Activated.Subscribe(OnActivated);
            InvoiceViewModels.Add(invoiceViewModel);
            invoiceViewModel.IsActive = true;
        }

        private void OnActivated(InvoiceViewModel invoiceViewModel)
        {
            InvoiceViewModels.Where(vm => vm != invoiceViewModel).ForEach(vm => vm.IsActive = false);
        }

        public void Subscribe()
        {
            if (_lastActive != null && InvoiceViewModels.Contains(_lastActive))
            {
                _lastActive.IsActive = true;
            }
            else if (InvoiceViewModels.Count == 1)
            {
                InvoiceViewModels.Single().IsActive = true;
            }
        }

        public void Unsubscribe()
        {
            _lastActive = InvoiceViewModels.FirstOrDefault(vm => vm.IsActive);
            InvoiceViewModels.ForEach(vm => vm.IsActive = false);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}