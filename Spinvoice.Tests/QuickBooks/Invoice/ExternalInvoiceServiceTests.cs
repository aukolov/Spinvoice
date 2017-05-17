using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Spinvoice.Domain.Accounting;
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
            var oauthParams = new OAuthParams();
            var oauthProfile = Secret.GetOAuthProfile();
            _externalInvoiceService = new ExternalInvoiceService(
                new ExternalInvoiceTranslator(),
                new ExternalConnection(oauthProfile, oauthParams));
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