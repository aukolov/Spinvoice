using System;
using Moq;
using NUnit.Framework;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.IntegrationTests.QuickBooks
{
    [TestFixture]
    public class ExternalExchangeRateIntegrationTests
    {
        private ExternalConnection _externalConnection;

        [SetUp]
        public void Setup()
        {
            var oathRepositoryMock = new Mock<IOAuthRepository>();
            oathRepositoryMock.Setup(repository => repository.Profile).Returns(QuickBooksUtils.GetOAuthProfile());
            oathRepositoryMock.Setup(repository => repository.Params).Returns(new OAuthParams());

            _externalConnection = new ExternalConnection(oathRepositoryMock.Object);
        }

        [Test]
        public void GetsExchangeRate()
        {
            var exchangeRate = _externalConnection.GetExchangeRate(
                new DateTime(2017, 5, 17), 
                "GBP");

            Assert.IsNotNull(exchangeRate);
            Assert.AreEqual(1.2967m, exchangeRate.Rate);
        }

    }
}