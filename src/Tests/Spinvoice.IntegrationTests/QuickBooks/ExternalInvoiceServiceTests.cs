using System;
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using NUnit.Framework;
using Spinvoice.Domain.Accounting;
using Spinvoice.QuickBooks.Company;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Domain;
using Spinvoice.QuickBooks.Invoice;
using Spinvoice.QuickBooks.Item;

namespace Spinvoice.IntegrationTests.QuickBooks
{
    [TestFixture]
    public class ExternalInvoiceServiceTests
    {
        private ExternalInvoiceService _externalInvoiceService;
        private ExternalItemRepository _externalItemRepository;
        private ExternalCompanyRepository _externalCompanyRepository;

        [SetUp]
        public void Setup()
        {
            var oathRepositoryMock = new Mock<IOAuthRepository>();
            oathRepositoryMock.Setup(repository => repository.Profile).Returns(QuickBooksUtils.GetOAuthProfile());
            oathRepositoryMock.Setup(repository => repository.Params).Returns(new OAuthParams());

            var externalConnection = new ExternalConnection(oathRepositoryMock.Object);
            var accountsChartRepositoryMock = new Mock<IAccountsChartRepository>();
            accountsChartRepositoryMock.Setup(repository => repository.AccountsChart)
                .Returns(SandboxAccountChartProvider.Get());
            _externalCompanyRepository = new ExternalCompanyRepository(externalConnection);
            _externalItemRepository = new ExternalItemRepository(
                accountsChartRepositoryMock.Object,
                externalConnection);
            _externalInvoiceService = new ExternalInvoiceService(
                new ExternalInvoiceTranslator(accountsChartRepositoryMock.Object),
                externalConnection);
        }

        [Test]
        public void CreatesBill()
        {
            var applesName = "Apples " + Guid.NewGuid();
            var externalApples = _externalItemRepository.Add(applesName);
            Assert.IsNotNull(externalApples.Id);

            var orangesName = "Oranges " + Guid.NewGuid();
            var externalOranges = _externalItemRepository.Add(orangesName);
            Assert.IsNotNull(externalOranges.Id);

            var companyName = "Test Co " + Guid.NewGuid();
            var externalCompany = _externalCompanyRepository.Create(companyName, "GBP");
            Assert.IsNotNull(externalCompany.Id);

            for (var i = 1; i <= 10; i++)
            {
                var invoiceNumber = "INV NO " + new Random(Environment.TickCount).Next();
                var invoice = new Invoice
                {
                    CompanyName = companyName,
                    ExternalCompanyId = externalCompany.Id,
                    Currency = "GBP",
                    Date = new DateTime(2017, 5, 17),
                    InvoiceNumber = invoiceNumber,
                    ExchangeRate = 1.05123m,
                    NetAmount = i * 1000,
                    Positions = new ObservableCollection<Position>
                    {
                        new Position
                        {
                            Name = applesName,
                            Amount = i * 700,
                            Quantity = 100,
                            ExternalId = externalApples.Id
                        },
                        new Position
                        {
                            Name = orangesName,
                            Amount = 300,
                            Quantity = i * 110,
                            ExternalId = externalOranges.Id
                        }
                    }
                };

                var externalInvoiceId = _externalInvoiceService.Save(invoice);

                var bill = _externalInvoiceService.GetById(externalInvoiceId);
                Assert.AreEqual(invoiceNumber, bill.DocNumber);
                //var bills = _externalInvoiceService.GetByExternalCompany(externalCompany.Id);
                //Assert.AreEqual(invoiceNumber, bills.Single().DocNumber);
            }
        }

        [Test]
        public void CreatesBillWithVatAndTransportationCosts()
        {
            var companyName = "Test Co " + Guid.NewGuid();
            var externalCompany = _externalCompanyRepository.Create(companyName, "GBP");
            Assert.IsNotNull(externalCompany.Id);

            var invoice = new Invoice
            {
                CompanyName = companyName,
                ExternalCompanyId = externalCompany.Id,
                Currency = "GBP",
                Date = new DateTime(2017, 5, 17),
                InvoiceNumber = "INV NO 123",
                ExchangeRate = 1.05123m,
                NetAmount = 1000,
                Positions = new ObservableCollection<Position>(),
                VatAmount = 15.12m,
                TransportationCosts = 54.33m
            };

            _externalInvoiceService.Save(invoice);
        }

    }
}