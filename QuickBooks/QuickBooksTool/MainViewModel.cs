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
        private bool _updateAllInvoices;

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
                Invoices.Clear();
                if (_selectedCompany != null)
                {
                    Invoices.AddRange(_externalInvoiceService.GetByExternalCompany(_selectedCompany.Id));
                }
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Bill> Invoices { get; } = new ObservableCollection<Bill>();

        public bool UpdateAllInvoices
        {
            get { return _updateAllInvoices; }
            set
            {
                _updateAllInvoices = value;
                OnPropertyChanged();
            }
        }

        public Bill SelectedInvoice { get; set; }
        public ObservableCollection<IExternalAccount> Accounts { get; } = new ObservableCollection<IExternalAccount>();
        public IExternalAccount SelectedAccount { get; set; }

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
            if (UpdateAllInvoices && !Invoices.Any())
            {
                return;
            }
            if (!UpdateAllInvoices && SelectedInvoice == null)
            {
                return;
            }

            if (MessageBox.Show("Are you sure?", "Update Invoices", MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            if (UpdateAllInvoices)
            {
                foreach (var invoice in Invoices)
                {
                    UpdateInvoice(invoice.Id);
                }
            }
            else
            {
                UpdateInvoice(SelectedInvoice.Id);
            }

            MessageBox.Show("Done!", "Update Invoices", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateInvoice(string externalInvoiceId)
        {
            var bill = _externalInvoiceService.GetById(externalInvoiceId);
            var positions = bill.Line.Where(line => line.DetailType == LineDetailTypeEnum.ItemBasedExpenseLineDetail).ToArray();
            if (!positions.Any())
            {
                return;
            }
            var positionSum = positions
                .Where(line => line.AmountSpecified)
                .Sum(line => line.Amount);
            var newLines = bill.Line.Where(line => line.DetailType != LineDetailTypeEnum.ItemBasedExpenseLineDetail).ToList();
            if (positionSum > 0)
            {
                newLines.Add(new Line
                {
                    Amount = positionSum,
                    AmountSpecified = true,
                    DetailType = LineDetailTypeEnum.AccountBasedExpenseLineDetail,
                    DetailTypeSpecified = true,
                    Description = "special",
                    AnyIntuitObject = new AccountBasedExpenseLineDetail
                    {
                        AccountRef = new ReferenceType
                        {
                            Value = SelectedAccount.Id
                        }
                    }
                });
            }
            bill.Line = newLines.ToArray();

            _externalInvoiceService.Update(bill);
            return;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}