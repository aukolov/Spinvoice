using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Win32;
using Spinvoice.Domain;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.Exchange;
using Spinvoice.Properties;
using Spinvoice.Services;

namespace Spinvoice.ViewModels
{
    public sealed class AppViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly ExchangeRatesLoader _exchangeRatesLoader;

        public AppViewModel(
            ICompanyRepository companyRepository,
            IExchangeRatesRepository exchangeRatesRepository,
            ExchangeRatesLoader exchangeRatesLoader,
            IFileService fileService)
        {
            InvoiceViewModel = new InvoiceViewModel(companyRepository, exchangeRatesRepository);
            _exchangeRatesLoader = exchangeRatesLoader;

            LoadExchangeRatesCommand = new RelayCommand(LoadExchangeRates);

            ProjectBrowserViewModel = new ProjectBrowserViewModel(fileService);
        }

        private void LoadExchangeRates()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "XML|*.xml"
            };
            if (dialog.ShowDialog() ?? false)
            {
                _exchangeRatesLoader.Load(dialog.FileName);
            }
        }

        public ICommand LoadExchangeRatesCommand { get; }
        public ProjectBrowserViewModel ProjectBrowserViewModel { get; }
        public InvoiceViewModel InvoiceViewModel { get; }

        public void Dispose()
        {
            InvoiceViewModel?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}