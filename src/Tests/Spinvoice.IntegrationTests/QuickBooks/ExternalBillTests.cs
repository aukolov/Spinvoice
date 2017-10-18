using System;
using System.Collections.ObjectModel;
using System.Linq;
using Intuit.Ipp.Data;
using Moq;
using NUnit.Framework;
using Spinvoice.Domain.Accounting;
using Spinvoice.QuickBooks.Company;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Domain;
using Spinvoice.QuickBooks.Invoice;
using Spinvoice.QuickBooks.Item;
using Invoice = Spinvoice.Domain.Accounting.Invoice;

namespace Spinvoice.IntegrationTests.QuickBooks
{
    [TestFixture]
    public class ExternalBillTests
    {
        private IExternalInvoiceAndBillService _externalInvoiceAndBillService;
        private IExternalItemRepository _externalItemRepository;
        private IExternalCompanyRepository _externalCompanyRepository;
        private IExternalBillCrudService _externalBillCrudService;

        [SetUp]
        public void Setup()
        {
            var oathRepositoryMock = new Mock<IOAuthRepository>();
            oathRepositoryMock.Setup(repository => repository.Profile).Returns(QuickBooksUtils.GetOAuthProfile());
            oathRepositoryMock.Setup(repository => repository.Params).Returns(new OAuthParams());

            var externalConnection = new ExternalConnection(oathRepositoryMock.Object, new ExternalAuthService());
            var accountsChartRepositoryMock = new Mock<IAccountsChartRepository>();
            accountsChartRepositoryMock.Setup(repository => repository.AccountsChart)
                .Returns(SandboxAccountChartProvider.Get());
            _externalCompanyRepository = new ExternalCompanyRepository(externalConnection);
            _externalItemRepository = new ExternalItemRepository(
                accountsChartRepositoryMock.Object,
                externalConnection);
            _externalInvoiceAndBillService = new ExternalInvoiceAndBillService(
                new ExternalInvoiceUpdater(accountsChartRepositoryMock.Object),
                externalConnection);
            _externalBillCrudService = new ExternalBillCrudService(externalConnection);
        }

        [Test]
        public void CreatesBill()
        {
            var applesName = "Apples " + Guid.NewGuid();
            var externalApples = _externalItemRepository.AddInventory(applesName);
            Assert.IsNotNull(externalApples.Id);

            var orangesName = "Oranges " + Guid.NewGuid();
            var externalOranges = _externalItemRepository.AddInventory(orangesName);
            Assert.IsNotNull(externalOranges.Id);

            var companyName = "Test Co " + Guid.NewGuid();
            var externalCompany = _externalCompanyRepository.Create(companyName, Side.Vendor, "GBP");
            Assert.IsNotNull(externalCompany.Id);

            var invoiceNumber = "INV NO " + new Random(Environment.TickCount).Next();
            var invoice = new Invoice
            {
                Side = Side.Vendor,
                CompanyName = companyName,
                ExternalCompanyId = externalCompany.Id,
                Currency = "GBP",
                Date = new DateTime(2017, 5, 17),
                InvoiceNumber = invoiceNumber,
                ExchangeRate = 1.05123m,
                NetAmount = 1000,
                Positions = new ObservableCollection<Position>
                    {
                        new Position
                        {
                            Name = applesName,
                            Amount =  700,
                            Quantity = 100,
                            ExternalId = externalApples.Id
                        },
                        new Position
                        {
                            Name = orangesName,
                            Amount = 300,
                            Quantity =  110,
                            ExternalId = externalOranges.Id
                        }
                    }
            };

            var externalInvoiceId = _externalInvoiceAndBillService.Save(invoice);

            var bill = _externalBillCrudService.GetById(externalInvoiceId);
            Assert.AreEqual(invoiceNumber, bill.DocNumber);
            var bills = _externalBillCrudService.GetByExternalCompany(externalCompany.Id);
            Assert.AreEqual(invoiceNumber, bills.Single().DocNumber);
        }

        [Test]
        public void CreatesBillWithVatAndTransportationCosts()
        {
            var companyName = "Test Co " + Guid.NewGuid();
            var externalCompany = _externalCompanyRepository.Create(companyName, Side.Vendor, "GBP");
            Assert.IsNotNull(externalCompany.Id);

            var invoice = new Invoice
            {
                Side = Side.Vendor,
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

            _externalInvoiceAndBillService.Save(invoice);
        }

        [Test]
        public void UpdatesBill()
        {
            // Setup.
            var applesName = "Apples " + Guid.NewGuid();
            var externalApples = _externalItemRepository.AddInventory(applesName);
            Assert.IsNotNull(externalApples.Id);

            var orangesName = "Oranges " + Guid.NewGuid();
            var externalOranges = _externalItemRepository.AddInventory(orangesName);
            Assert.IsNotNull(externalOranges.Id);

            var companyName1 = "Test Co " + Guid.NewGuid();
            var externalCompany1 = _externalCompanyRepository.Create(companyName1, Side.Vendor, "GBP");
            Assert.IsNotNull(externalCompany1.Id);

            var companyName2 = "Test Co " + Guid.NewGuid();
            var externalCompany2 = _externalCompanyRepository.Create(companyName2, Side.Vendor, "EUR");
            Assert.IsNotNull(externalCompany2.Id);

            // Create invoice.
            var invoiceNumber1 = "INV NO " + new Random(Environment.TickCount).Next();
            var invoice = new Invoice
            {
                Side = Side.Vendor,
                CompanyName = companyName1,
                ExternalCompanyId = externalCompany1.Id,
                Currency = "GBP",
                Date = new DateTime(2017, 5, 17),
                InvoiceNumber = invoiceNumber1,
                ExchangeRate = 1.05123m,
                NetAmount = 700,
                Positions = new ObservableCollection<Position>
                {
                    new Position
                    {
                        Name = applesName,
                        Amount =  700,
                        Quantity = 100,
                        ExternalId = externalApples.Id
                    }
                }
            };

            var externalInvoiceId1 = _externalInvoiceAndBillService.Save(invoice);
            invoice.ExternalId = externalInvoiceId1;

            // Update invoice.
            var invoiceNumber2 = invoiceNumber1 + "*";
            invoice.CompanyName = companyName2;
            invoice.ExternalCompanyId = externalCompany2.Id;
            invoice.Currency = "EUR";
            invoice.Date = new DateTime(2017, 5, 18);
            invoice.InvoiceNumber = invoiceNumber2;
            invoice.ExchangeRate = 1.06234m;
            invoice.NetAmount = 2000;
            invoice.Positions.Single().Amount = 1500;
            invoice.Positions.Single().Quantity = 250;
            invoice.Positions.Add(new Position
            {
                Name = orangesName,
                Amount = 500,
                Quantity = 110,
                ExternalId = externalOranges.Id
            });

            var externalInvoiceId2 = _externalInvoiceAndBillService.Save(invoice);

            // Verify.
            Assert.AreEqual(externalInvoiceId1, externalInvoiceId2);

            var bill = _externalBillCrudService.GetById(externalInvoiceId2);
            Assert.AreEqual(externalCompany2.Id, bill.VendorRef.Value);
            Assert.AreEqual("Euro", bill.CurrencyRef.name);
            Assert.AreEqual(new DateTime(2017, 5, 18), bill.TxnDate);
            Assert.AreEqual(invoiceNumber2, bill.DocNumber);
            Assert.AreEqual(1.06234m, bill.ExchangeRate);
            Assert.AreEqual(2000, bill.TotalAmt);
            Assert.AreEqual(2, bill.Line.Length);

            Assert.AreEqual(1500, bill.Line[0].Amount);
            Assert.AreEqual(250, ((ItemBasedExpenseLineDetail)bill.Line[0].AnyIntuitObject).Qty);
            Assert.AreEqual(externalApples.Id, ((ItemBasedExpenseLineDetail)bill.Line[0].AnyIntuitObject).ItemRef.Value);
            Assert.AreEqual(applesName, ((ItemBasedExpenseLineDetail)bill.Line[0].AnyIntuitObject).ItemRef.name);

            Assert.AreEqual(500, bill.Line[1].Amount);
            Assert.AreEqual(110, ((ItemBasedExpenseLineDetail)bill.Line[1].AnyIntuitObject).Qty);
            Assert.AreEqual(externalOranges.Id, ((ItemBasedExpenseLineDetail)bill.Line[1].AnyIntuitObject).ItemRef.Value);
            Assert.AreEqual(orangesName, ((ItemBasedExpenseLineDetail)bill.Line[1].AnyIntuitObject).ItemRef.name);
        }
    }
}