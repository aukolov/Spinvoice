using System;
using System.Linq;
using Intuit.Ipp.Data;
using Moq;
using NUnit.Framework;
using Spinvoice.Domain.Accounting;
using Spinvoice.QuickBooks.Account;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Domain;
using Spinvoice.QuickBooks.Item;

namespace Spinvoice.IntegrationTests.QuickBooks
{
    [TestFixture]
    public class ExternalItemRepositoryTests
    {
        private ExternalConnection _externalConnection;
        private ExternalItemRepository _externalItemRepository;
        private ExternalAccountRepository _externalAccountRepository;

        [SetUp]
        public void Setup()
        {
            var oathRepositoryMock = new Mock<IOAuthRepository>();
            oathRepositoryMock.Setup(repository => repository.Profile).Returns(QuickBooksUtils.GetOAuthProfile());
            oathRepositoryMock.Setup(repository => repository.Params).Returns(new OAuthParams());

            _externalConnection = new ExternalConnection(oathRepositoryMock.Object, new ExternalAuthService());
            var accountsChartRepositoryMock = new Mock<IAccountsChartRepository>();
            accountsChartRepositoryMock.Setup(repository => repository.AccountsChart)
                .Returns(SandboxAccountChartProvider.Get());
            _externalAccountRepository = new ExternalAccountRepository(_externalConnection);
            _externalItemRepository = new ExternalItemRepository(
                accountsChartRepositoryMock.Object,
                _externalConnection);
        }

        [Test]
        public void GetsAllItems()
        {
            var items = _externalConnection.GetAll<Item>().ToArray();
            Assert.IsTrue(items.Length > 0);
        }

        [Test]
        public void CreatesInventoryItem()
        {
            var externalItem = _externalItemRepository.AddInventory("Test Inventory " + Guid.NewGuid());

            Assert.IsTrue(!string.IsNullOrEmpty(externalItem.Id));
        }

        [Test]
        public void CreatesIncomeServiceItem()
        {
            var externalAccount = _externalAccountRepository.GetAll().FirstOrDefault(account => account.Name == "Services");
            Assert.IsNotNull(externalAccount);

            var externalItem = _externalItemRepository.AddService(
                "Test Service " + Guid.NewGuid(), 
                externalAccount.Id, 
                Side.Customer);

            Assert.IsTrue(!string.IsNullOrEmpty(externalItem.Id));
        }

        [Test]
        public void CreatesExpenseServiceItem()
        {
            var externalAccount = _externalAccountRepository.GetAll().FirstOrDefault(account => account.Name == "Purchases");
            Assert.IsNotNull(externalAccount);

            var externalItem = _externalItemRepository.AddService(
                "Test Service " + Guid.NewGuid(),
                externalAccount.Id,
                Side.Vendor);

            Assert.IsTrue(!string.IsNullOrEmpty(externalItem.Id));
        }

    }
}