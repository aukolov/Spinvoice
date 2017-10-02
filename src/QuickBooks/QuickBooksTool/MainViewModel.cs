using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Intuit.Ipp.Data;
using QuickBooksTool.Annotations;
using Spinvoice.Common.Presentation;
using Spinvoice.QuickBooks.Account;
using Spinvoice.QuickBooks.Domain;
using Spinvoice.QuickBooks.Invoice;
using Spinvoice.QuickBooks.ViewModels;
using Spinvoice.Utils;

namespace QuickBooksTool
{
    public class MainViewModel : IMainViewModel
    {
        private readonly IExternalCompanyRepository _externalCompanyRepository;
        private readonly IExternalAccountRepository _externalAccountRepository;
        private readonly IExternalInvoiceService _externalInvoiceService;
        private readonly IExternalConnectionWatcher _externalConnectionWatcher;
        private readonly IWindowManager _windowManager;
        private readonly Func<IQuickBooksConnectViewModel> _connectViewModelFactory;
        private IExternalCompany _selectedCompany;

        public MainViewModel(
            IExternalCompanyRepository externalCompanyRepository,
            IExternalAccountRepository externalAccountRepository,
            IExternalInvoiceService externalInvoiceService,
            IExternalConnectionWatcher externalConnectionWatcher,
            IWindowManager windowManager,
            Func<IQuickBooksConnectViewModel> connectViewModelFactory)
        {
            _externalCompanyRepository = externalCompanyRepository;
            _externalAccountRepository = externalAccountRepository;
            _externalInvoiceService = externalInvoiceService;
            _externalConnectionWatcher = externalConnectionWatcher;
            _windowManager = windowManager;
            _connectViewModelFactory = connectViewModelFactory;
            ConnectCommand = new RelayCommand(ShowConnectDialog);
            PositionsToAccountItemCommand = new RelayCommand(PositionsToAccountItem);
            AddInvoiceCommand = new RelayCommand(AddInvoice);
            RemoveInvoiceCommand = new RelayCommand(RemoveInvoice);
        }

        public ICommand ConnectCommand { get; }
        public ICommand PositionsToAccountItemCommand { get; private set; }
        public ObservableCollection<IExternalCompany> Companies { get; } = new ObservableCollection<IExternalCompany>();

        public IExternalCompany SelectedCompany
        {
            get { return _selectedCompany; }
            set
            {
                _selectedCompany = value;
                UpdateInvoice();
                OnPropertyChanged();
            }
        }

        private void UpdateInvoice()
        {
            AvailableInvoices.Clear();
            SelectedInvoices.Clear();
            if (_selectedCompany != null)
            {
                AvailableInvoices.AddRange(_externalInvoiceService.GetByExternalCompany(_selectedCompany.Id));
            }
        }

        public ObservableCollection<IExternalAccount> Accounts { get; } = new ObservableCollection<IExternalAccount>();
        public IExternalAccount SelectedAccount { get; set; }

        public ObservableCollection<Bill> AvailableInvoices { get; } = new ObservableCollection<Bill>();
        public Bill InvoiceToAdd { get; set; }
        public ICommand AddInvoiceCommand { get; }

        public ObservableCollection<Bill> SelectedInvoices { get; } = new ObservableCollection<Bill>();
        public Bill InvoiceToRemove { get; set; }
        public ICommand RemoveInvoiceCommand { get; }

        public DateTime SelectedDate { get; set; }

        private void ShowConnectDialog()
        {
            var connectViewModel = _connectViewModelFactory();
            _windowManager.ShowDialog(connectViewModel);
            if (!_externalConnectionWatcher.IsConnected)
            {
                return;
            }

            Companies.Clear();
            Companies.AddRange(_externalCompanyRepository.GetAll().OrderBy(company => company.Name));
            Accounts.Clear();
            Accounts.AddRange(_externalAccountRepository.GetAll().OrderBy(account => account.Name));
        }

        private void PositionsToAccountItem()
        {
            if (!SelectedInvoices.Any())
            {
                return;
            }
            if (SelectedAccount == null)
            {
                return;
            }

            if (MessageBox.Show("Are you sure?", "Update Invoices", MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            var totalAmount = SelectedInvoices.Sum(bill => bill.TotalAmt);

            _externalInvoiceService.Add(new Bill
            {
                TotalAmt = totalAmount,
                Line = new[] {new Line
                {
                    Amount = totalAmount,
                    AmountSpecified = true,
                    DetailType = LineDetailTypeEnum.AccountBasedExpenseLineDetail,
                    DetailTypeSpecified = true,
                    AnyIntuitObject = new AccountBasedExpenseLineDetail
                    {
                        AccountRef = new ReferenceType
                        {
                            Value = SelectedAccount.Id
                        }
                    }
                }},
                CurrencyRef = SelectedInvoices.First().CurrencyRef,
                VendorRef = new ReferenceType
                {
                    Value = SelectedCompany.Id
                },
                ExchangeRateSpecified = false,
                TxnDate = SelectedDate,
                TxnDateSpecified = true,
                PrivateNote = string.Join(", ", SelectedInvoices.Select(x => x.DocNumber))
            });

            foreach (var selectedInvoice in SelectedInvoices)
            {
                _externalInvoiceService.Delete(new Bill
                {
                    Id = selectedInvoice.Id,
                    SyncToken = "0"
                });
            }

            MessageBox.Show("Done!", "Update Invoices", MessageBoxButton.OK, MessageBoxImage.Information);

            UpdateInvoice();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddInvoice()
        {
            if (InvoiceToAdd == null)
            {
                return;
            }

            SelectedInvoices.Add(InvoiceToAdd);
            AvailableInvoices.Remove(InvoiceToAdd);
        }

        private void RemoveInvoice()
        {
            if (InvoiceToRemove == null)
            {
                return;
            }

            AvailableInvoices.Add(InvoiceToRemove);
            SelectedInvoices.Remove(InvoiceToRemove);
        }
    }
}