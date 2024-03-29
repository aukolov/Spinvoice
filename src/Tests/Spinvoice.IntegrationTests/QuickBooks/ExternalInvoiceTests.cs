﻿using System;
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
    public class ExternalInvoiceTests
    {
        private IExternalInvoiceAndBillService _externalInvoiceAndBillService;
        private IExternalItemRepository _externalItemRepository;
        private IExternalCompanyRepository _externalCompanyRepository;
        private IExternalInvoiceCrudService _externalInvoiceCrudService;

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
            _externalInvoiceCrudService = new ExternalInvoiceCrudService(externalConnection);
        }

        [Test]
        public void CreatesInvoice()
        {
            var applesName = "Apples " + Guid.NewGuid();
            var externalApples = _externalItemRepository.AddInventory(applesName);
            Assert.IsNotNull(externalApples.Id);

            var orangesName = "Oranges " + Guid.NewGuid();
            var externalOranges = _externalItemRepository.AddInventory(orangesName);
            Assert.IsNotNull(externalOranges.Id);

            var companyName = "Test Co " + Guid.NewGuid();
            var externalCompany = _externalCompanyRepository.Create(companyName, Side.Customer, "GBP");
            Assert.IsNotNull(externalCompany.Id);

            var invoiceNumber = "INV NO " + new Random(Environment.TickCount).Next();
            var invoice = new Invoice
            {
                Side = Side.Customer,
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

            var externalInvoice = _externalInvoiceCrudService.GetById(externalInvoiceId);
            Assert.AreEqual(invoiceNumber, externalInvoice.DocNumber);
            var bills = _externalInvoiceCrudService.GetByExternalCompany(externalCompany.Id);
            Assert.AreEqual(invoiceNumber, bills.Single().DocNumber);
        }

        [Test]
        public void UpdatesInvoice()
        {
            // Setup.
            var applesName = "Apples " + Guid.NewGuid();
            var externalApples = _externalItemRepository.AddInventory(applesName);
            Assert.IsNotNull(externalApples.Id);

            var orangesName = "Oranges " + Guid.NewGuid();
            var externalOranges = _externalItemRepository.AddInventory(orangesName);
            Assert.IsNotNull(externalOranges.Id);

            var companyName1 = "Test Co " + Guid.NewGuid();
            var externalCompany1 = _externalCompanyRepository.Create(companyName1, Side.Customer, "GBP");
            Assert.IsNotNull(externalCompany1.Id);

            var companyName2 = "Test Co " + Guid.NewGuid();
            var externalCompany2 = _externalCompanyRepository.Create(companyName2, Side.Customer, "EUR");
            Assert.IsNotNull(externalCompany2.Id);

            // Create invoice.
            var invoiceNumber1 = "INV NO " + new Random(Environment.TickCount).Next();
            var invoice = new Invoice
            {
                Side = Side.Customer,
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

            var externalInvoice = _externalInvoiceCrudService.GetById(externalInvoiceId2);
            Assert.AreEqual(externalCompany2.Id, externalInvoice.CustomerRef.Value);
            Assert.AreEqual("Euro", externalInvoice.CurrencyRef.name);
            Assert.AreEqual(new DateTime(2017, 5, 18), externalInvoice.TxnDate);
            Assert.AreEqual(invoiceNumber2, externalInvoice.DocNumber);
            Assert.AreEqual(1.06234m, externalInvoice.ExchangeRate);
            Assert.AreEqual(2000, externalInvoice.TotalAmt);
            Assert.AreEqual(2, externalInvoice.Line.Count(x => x.DetailType == LineDetailTypeEnum.SalesItemLineDetail));

            Assert.AreEqual(1500, externalInvoice.Line[0].Amount);
            Assert.AreEqual(250, ((SalesItemLineDetail)externalInvoice.Line[0].AnyIntuitObject).Qty);
            Assert.AreEqual(externalApples.Id, ((SalesItemLineDetail)externalInvoice.Line[0].AnyIntuitObject).ItemRef.Value);
            Assert.AreEqual(applesName, ((SalesItemLineDetail)externalInvoice.Line[0].AnyIntuitObject).ItemRef.name);

            Assert.AreEqual(500, externalInvoice.Line[1].Amount);
            Assert.AreEqual(110, ((SalesItemLineDetail)externalInvoice.Line[1].AnyIntuitObject).Qty);
            Assert.AreEqual(externalOranges.Id, ((SalesItemLineDetail)externalInvoice.Line[1].AnyIntuitObject).ItemRef.Value);
            Assert.AreEqual(orangesName, ((SalesItemLineDetail)externalInvoice.Line[1].AnyIntuitObject).ItemRef.name);
        }
    }
}