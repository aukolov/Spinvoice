﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Spinvoice.Annotations;
using Spinvoice.Domain.Pdf;
using Spinvoice.Utils;

namespace Spinvoice.ViewModels.Invoices
{
    public class InvoiceListViewModel : IInvoiceListViewModel
    {
        private readonly IPdfParser _pdfParser;

        private readonly string _filePath;
        private readonly Func<PdfModel, PdfXrayViewModel, InvoiceViewModel> _invoiceViewModelFactory;
        private InvoiceViewModel _lastActive;
        private PdfXrayViewModel _pdfXrayViewModel;
        private bool _isLoaded;

        public InvoiceListViewModel(
            string filePath,
            IPdfParser pdfParser,
            Func<PdfModel, PdfXrayViewModel, InvoiceViewModel> invoiceViewModelFactory)
        {
            _filePath = filePath;
            _invoiceViewModelFactory = invoiceViewModelFactory;
            _pdfParser = pdfParser;

            InvoiceViewModels = new ObservableCollection<InvoiceViewModel>();
            AddInvoiceViewModelCommand = new RelayCommand(
                () => AddInvoiceViewModel(null, PdfXrayViewModel));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PdfXrayViewModel PdfXrayViewModel
        {
            get { return _pdfXrayViewModel; }
            private set
            {
                if (value == _pdfXrayViewModel) return;
                _pdfXrayViewModel = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand AddInvoiceViewModelCommand { get; }
        public ObservableCollection<InvoiceViewModel> InvoiceViewModels { get; }

        public bool IsLoaded
        {
            get { return _isLoaded; }
            set
            {
                if (_isLoaded == value) return;
                _isLoaded = value;
                OnPropertyChanged();
            }
        }

        public void Init()
        {
            CreatePdfModelAsync();
        }

        private async void CreatePdfModelAsync()
        {
            PdfModel pdfModel;
            if (_pdfParser.IsPdf(_filePath))
            {
                pdfModel = await Task.Factory.StartNew(() => _pdfParser.Parse(_filePath));
            }
            else
            {
                pdfModel = null;
            }

            PdfXrayViewModel = pdfModel != null ? new PdfXrayViewModel(pdfModel) : null;
            AddInvoiceViewModel(pdfModel, PdfXrayViewModel);
            IsLoaded = true;
        }

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