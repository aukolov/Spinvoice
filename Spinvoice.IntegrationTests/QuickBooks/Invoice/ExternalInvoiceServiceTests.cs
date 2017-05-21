using System;
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.ExternalBook;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Invoice;

namespace Spinvoice.IntegrationTests.QuickBooks.Invoice
{
    [TestFixture]
    public class ExternalInvoiceServiceTests
    {
        private ExternalInvoiceService _externalInvoiceService;

        [SetUp]
        public void Setup()
        {
            var oathRepositoryMock = new Mock<IOAuthRepository>();
            oathRepositoryMock.Setup(repository => repository.Profile).Returns(Secret.GetOAuthProfile());
            oathRepositoryMock.Setup(repository => repository.Params).Returns(new OAuthParams());

            _externalInvoiceService = new ExternalInvoiceService(
                new ExternalInvoiceTranslator(),
                new ExternalConnection(oathRepositoryMock.Object));
        }

        [Test]
        public void CreatesBill()
        {
            var invoice = new Domain.Accounting.Invoice
            {
                CompanyName = "Test Co",
                Currency = "USD",
                Date = new DateTime(2017, 5, 17),
                InvoiceNumber = "INV NO 123",
                ExchangeRate = 1.05123m,
                NetAmount = 1000,
                Positions = new ObservableCollection<Position>
                {
                    new Position
                    {
                        Name = "Apples",
                        Amount = 700
                    },
                    new Position
                    {
                        Name = "Oranges",
                        Amount = 300
                    }
                }
            };

            _externalInvoiceService.Save(invoice);
        }
    }
}