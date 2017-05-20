using System;
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.ExternalBook;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Invoice;

namespace Spinvoice.Tests.QuickBooks.Invoice
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
            var invoice = new Spinvoice.Domain.Accounting.Invoice
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
                        Description = "Apples",
                        Amount = 700
                    },
                    new Position
                    {
                        Description = "Oranges",
                        Amount = 300
                    }
                }
            };

            _externalInvoiceService.Save(invoice);
        }
    }
}