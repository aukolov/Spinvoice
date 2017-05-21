using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Intuit.Ipp.Data;
using Spinvoice.Domain.ExternalBook;
using Spinvoice.QuickBooks.Account;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.Utils;

namespace Spinvoice.QuickBooks.Item
{
    public class ExternalItemRepository : IExternalItemRepository
    {
        private readonly IExternalAccountRepository _externalAccountRepository;
        private readonly ExternalConnection _externalConnection;
        private readonly ObservableCollection<IExternalItem> _externalItems;
        private Dictionary<string, IExternalItem> _externalItemsByName;

        public ExternalItemRepository(
            IExternalAccountRepository externalAccountRepository,
            ExternalConnection externalConnection)
        {
            _externalAccountRepository = externalAccountRepository;
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

        public IExternalItem Add(string name)
        {
            var assetAccount = _externalAccountRepository.InventoryAsset;
            var costOfGoodsSold = _externalAccountRepository.CostOfGoodsSold;
            var salesOfProductIncome = _externalAccountRepository.SalesOfProductIncome;

            var item = new Intuit.Ipp.Data.Item
            {
                Name = name,
                Type = ItemTypeEnum.Inventory,
                TypeSpecified = true,
                AssetAccountRef = new ReferenceType
                {
                    Value = assetAccount.Id
                },
                ExpenseAccountRef = new ReferenceType
                {
                    Value = costOfGoodsSold.Id
                },
                IncomeAccountRef = new ReferenceType
                {
                    Value = salesOfProductIncome.Id
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
    }
}