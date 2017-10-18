using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Intuit.Ipp.Data;
using Spinvoice.Domain.Accounting;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Domain;
using Spinvoice.Utils;

namespace Spinvoice.QuickBooks.Item
{
    public class ExternalItemRepository : IExternalItemRepository
    {
        private readonly IAccountsChartRepository _accountsChartRepository;
        private readonly ExternalConnection _externalConnection;
        private readonly ObservableCollection<IExternalItem> _externalItems;
        private Dictionary<string, IExternalItem> _externalItemsByName;

        public ExternalItemRepository(
            IAccountsChartRepository accountsChartRepository,
            ExternalConnection externalConnection)
        {
            _accountsChartRepository = accountsChartRepository;
            _externalConnection = externalConnection;
            _externalItems = new ObservableCollection<IExternalItem>();
            _externalItemsByName = new Dictionary<string, IExternalItem>();
            _externalConnection.Connected += TryLoad;
            TryLoad();
        }

        private void TryLoad()
        {
            if (!_externalConnection.IsConnected
                || _externalItems.Any())
            {
                return;
            }

            var externalItem = _externalConnection
                .GetAll<Intuit.Ipp.Data.Item>()
                .Select(item => new ExternalItem(item));
            _externalItems.Clear();
            _externalItems.AddRange(externalItem);
            _externalItemsByName = _externalItems.ToDictionary(item => item.Name);
        }

        public IExternalItem Get(string name)
        {
            IExternalItem item;
            _externalItemsByName.TryGetValue(name, out item);
            return item;
        }

        public IExternalItem AddInventory(string name)
        {
            var item = new Intuit.Ipp.Data.Item
            {
                Name = name,
                Type = ItemTypeEnum.Inventory,
                TypeSpecified = true,
                AssetAccountRef = new ReferenceType
                {
                    Value = _accountsChartRepository.AccountsChart.AssetExternalAccountId
                },
                ExpenseAccountRef = new ReferenceType
                {
                    Value = _accountsChartRepository.AccountsChart.ExpenseExternalAccountId
                },
                IncomeAccountRef = new ReferenceType
                {
                    Value = _accountsChartRepository.AccountsChart.IncomeExternalAccountId
                },
                TrackQtyOnHand = true,
                TrackQtyOnHandSpecified = true,
                QtyOnHand = 0,
                QtyOnHandSpecified = true,
                InvStartDate = new DateTime(2015, 1, 1),
                InvStartDateSpecified = true
            };
            var addedItem = _externalConnection.Add(item);

            var externalItem = new ExternalItem(addedItem);
            _externalItemsByName.Add(name, externalItem);
            return externalItem;
        }

        public IExternalItem AddService(string name, string externalAccountId, Side side)
        {
            var item = new Intuit.Ipp.Data.Item
            {
                Name = name,
                Type = ItemTypeEnum.Service,
                TypeSpecified = true
            };

            switch (side)
            {
                case Side.Vendor:
                    item.ExpenseAccountRef = new ReferenceType
                    {
                        Value = externalAccountId
                    };
                    break;
                case Side.Customer:
                    item.IncomeAccountRef = new ReferenceType
                    {
                        Value = externalAccountId
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }

            var addedItem = _externalConnection.Add(item);

            var externalItem = new ExternalItem(addedItem);
            _externalItemsByName.Add(name, externalItem);
            return externalItem;
        }

    }
}