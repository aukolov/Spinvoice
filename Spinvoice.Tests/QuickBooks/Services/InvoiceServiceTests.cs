using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.QuickBooks;
using Spinvoice.QuickBooks.Services;

namespace Spinvoice.Tests.QuickBooks.Services
{
    [TestFixture]
    public class InvoiceServiceTests
    {
        [Test]
        public void CreatesBill()
        {
            var invoice = new Invoice
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

            var invoiceService = new InvoiceService(
                new OAuthParams(),
                Secret.GetOAuthProfile(),
                new InvoiceToBillTranslator());

            invoiceService.Save(invoice);
        }
    }
}