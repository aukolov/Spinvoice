using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Spinvoice.QuickBooks.Company;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Domain;
using Spinvoice.QuickBooks.Reporting;

namespace Spinvoice.IntegrationTests.QuickBooks
{
    [TestFixture]
    public class InventoryValuationReportTests
    {
        private InventoryValudationReportService _service;

        [SetUp]
        public void Setup()
        {
            var oathRepositoryMock = new Mock<IOAuthRepository>();
            oathRepositoryMock.Setup(repository => repository.Profile).Returns(QuickBooksUtils.GetOAuthProfile());
            oathRepositoryMock.Setup(repository => repository.Params).Returns(new OAuthParams());

            var externalConnection = new ExternalConnection(oathRepositoryMock.Object, new ExternalAuthService());
            _service = new InventoryValudationReportService(externalConnection);
        }

        [Test]
        public void ReceivesReport()
        {
            var items = _service.Execute(new DateTime(2018, 6, 16));
            Assert.IsNotNull(items);
            Assert.IsTrue(items.Length > 0);
            Assert.IsTrue(items.Any(x => x.Quantity > 0 && x.Amount > 0));
        }
    }
}