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
        private readonly IExternalBillCrudService _externalBillCrudService;
        private readonly IExternalConnectionWatcher _externalConnectionWatcher;
        private readonly IWindowManager _windowManager;
        private readonly Func<IQuickBooksConnectViewModel> _connectViewModelFactory;
        private IExternalCompany _selectedCompany;

        public MainViewModel(
            IExternalCompanyRepository externalCompanyRepository,
            IExternalAccountRepository externalAccountRepository,
            IExternalBillCrudService externalBillCrudService,
            IExternalConnectionWatcher externalConnectionWatcher,
            IWindowManager windowManager,
            Func<IQuickBooksConnectViewModel> connectViewModelFactory)
        {
            _externalCompanyRepository = externalCompanyRepository;
            _externalAccountRepository = externalAccountRepository;
            _externalBillCrudService = externalBillCrudService;
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
                AvailableInvoices.AddRange(
                    _externalBillCrudService
                        .GetByExternalCompany(_selectedCompany.Id)
                        .Select(bill => new InvoiceViewModel(bill)));
            }
        }

        public ObservableCollection<IExternalAccount> Accounts { get; } = new ObservableCollection<IExternalAccount>();
        public IExternalAccount SelectedAccount { get; set; }

        public ObservableCollection<InvoiceViewModel> AvailableInvoices { get; } = new ObservableCollection<InvoiceViewModel>();
        public ICommand AddInvoiceCommand { get; }

        public ObservableCollection<InvoiceViewModel> SelectedInvoices { get; } = new ObservableCollection<InvoiceViewModel>();
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

            var totalAmount = SelectedInvoices.Sum(bill => bill.Invoice.TotalAmt);

            _externalBillCrudService.Add(new Bill
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
                CurrencyRef = SelectedInvoices.First().Invoice.CurrencyRef,
                VendorRef = new ReferenceType
                {
                    Value = SelectedCompany.Id
                },
                ExchangeRateSpecified = false,
                TxnDate = SelectedDate,
                TxnDateSpecified = true,
                PrivateNote = string.Join(", ", SelectedInvoices.Select(x => x.Invoice.DocNumber))
            });

            foreach (var selectedInvoice in SelectedInvoices)
            {
                _externalBillCrudService.Delete(new Bill
                {
                    Id = selectedInvoice.Invoice.Id,
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
            foreach (var invoice in AvailableInvoices.Where(model => model.IsSelected).ToArray())
            {
                SelectedInvoices.Add(invoice);
                AvailableInvoices.Remove(invoice);
            }
        }

        private void RemoveInvoice()
        {
            foreach (var invoice in SelectedInvoices.Where(model => model.IsSelected).ToArray())
            {
                AvailableInvoices.Add(invoice);
                SelectedInvoices.Remove(invoice);
            }
        }
    }
}