﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Spinvoice.Application.Services;
using Spinvoice.Common.Domain.Pdf;
using Spinvoice.Utils;

namespace Spinvoice.Application.ViewModels.Invoices
{
    public class InvoiceListViewModel : IInvoiceListViewModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IFileParseServiceProxy _fileParseServiceProxy;
        private readonly IFileService _fileService;
        private readonly ITaskSchedulerProvider _taskSchedulerProvider;

        private readonly string _filePath;
        private readonly Func<PdfModel, PdfXrayViewModel, InvoiceViewModel> _invoiceViewModelFactory;
        private InvoiceViewModel _lastActive;
        private PdfXrayViewModel _pdfXrayViewModel;
        private bool _isLoaded;
        private FileProcessStatus _fileProcessStatus;
        private bool _isSubscribed;

        public InvoiceListViewModel(
            string filePath,
            IFileParseServiceProxy fileParseServiceProxy,
            IFileService fileService,
            ITaskSchedulerProvider taskSchedulerProvider,
            Func<PdfModel, PdfXrayViewModel, InvoiceViewModel> invoiceViewModelFactory)
        {
            _filePath = filePath;
            _invoiceViewModelFactory = invoiceViewModelFactory;
            _fileParseServiceProxy = fileParseServiceProxy;
            _fileService = fileService;
            _taskSchedulerProvider = taskSchedulerProvider;

            InvoiceViewModels = new ObservableCollection<InvoiceViewModel>();
            AddInvoiceViewModelCommand = new RelayCommand(
                () => AddInvoiceViewModel(null, PdfXrayViewModel));
            FileProcessStatus = FileProcessStatus.NotScheduled;
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

        public FileProcessStatus FileProcessStatus
        {
            get { return _fileProcessStatus; }
            set
            {
                if (_fileProcessStatus == value) return;
                _fileProcessStatus = value;
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

        public async void Init()
        {
            if (FileProcessStatus != FileProcessStatus.NotScheduled)
            {
                return;
            }
            FileProcessStatus = FileProcessStatus.Scheduled;

            PdfModel pdfModel;
            if (_fileService.HasExtension(_filePath, ".pdf"))
            {
                try
                {
                    pdfModel = await ParsePdfModel();
                    BackgroundExecutor.Execute(() => FileProcessStatus = FileProcessStatus.Done);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Error while parsing file. ");
                    BackgroundExecutor.Execute(() => FileProcessStatus = FileProcessStatus.Error);
                    pdfModel = null;
                }
            }
            else
            {
                pdfModel = null;
                BackgroundExecutor.Execute(() => FileProcessStatus = FileProcessStatus.Done);
            }

            PdfXrayViewModel = pdfModel != null ? new PdfXrayViewModel(pdfModel) : null;
            var invoiceViewModel = _invoiceViewModelFactory(pdfModel, PdfXrayViewModel);
            InvoiceViewModels.Add(invoiceViewModel);
            IsLoaded = true;
            if (_isSubscribed)
            {
                Subscribe();
            }
        }

        private async Task<PdfModel> ParsePdfModel()
        {
            return await Task.Factory.StartNew(() =>
                {
                    BackgroundExecutor.Execute(() => FileProcessStatus = FileProcessStatus.InProgress);
                    var pdfModel = _fileParseServiceProxy.Parse(_filePath);
                    return pdfModel;
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                _taskSchedulerProvider.PdfParseTaskScheduler);
        }

        private void AddInvoiceViewModel(PdfModel pdfModel, PdfXrayViewModel pdfXrayViewModel)
        {
            var invoiceViewModel = _invoiceViewModelFactory(pdfModel, pdfXrayViewModel);
            InvoiceViewModels.Add(invoiceViewModel);
            invoiceViewModel.Activated.Subscribe(OnActivated);
            invoiceViewModel.IsActive = true;
        }

        private void OnActivated(InvoiceViewModel invoiceViewModel)
        {
            InvoiceViewModels.Where(vm => vm != invoiceViewModel).ForEach(vm => vm.IsActive = false);
        }

        public void Subscribe()
        {
            _isSubscribed = true;
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
            _isSubscribed = false;
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