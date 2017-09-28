using System;
using System.Linq;
using Intuit.Ipp.Data;
using Moq;
using NUnit.Framework;
using Spinvoice.Domain.Accounting;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Domain;
using Spinvoice.QuickBooks.Item;

namespace Spinvoice.IntegrationTests.QuickBooks
{
    [TestFixture]
    public class ExternalItemServiceTests
    {
        private ExternalConnection _externalConnection;
        private ExternalItemRepository _externalItemRepository;

        [SetUp]
        public void Setup()
        {
            var oathRepositoryMock = new Mock<IOAuthRepository>();
            oathRepositoryMock.Setup(repository => repository.Profile).Returns(QuickBooksUtils.GetOAuthProfile());
            oathRepositoryMock.Setup(repository => repository.Params).Returns(new OAuthParams());

            _externalConnection = new ExternalConnection(oathRepositoryMock.Object);
            var accountsChartRepositoryMock = new Mock<IAccountsChartRepository>();
            accountsChartRepositoryMock.Setup(repository => repository.AccountsChart)
                .Returns(SandboxAccountChartProvider.Get());
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
        public void CreatesItem()
        {
            var externalItem = _externalItemRepository.Add("Test Item " + Guid.NewGuid());

            Assert.IsTrue(!string.IsNullOrEmpty(externalItem.Id));
        }

    }
}