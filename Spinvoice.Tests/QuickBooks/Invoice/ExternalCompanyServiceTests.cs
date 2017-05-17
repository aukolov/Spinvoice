using System;
using System.Linq;
using NUnit.Framework;
using Spinvoice.QuickBooks.Company;
using Spinvoice.QuickBooks.Connection;

namespace Spinvoice.Tests.QuickBooks.Invoice
{
    [TestFixture]
    public class ExternalCompanyServiceTests
    {
        private ExternalCompanyService _externalCompanyService;

        [SetUp]
        public void Setup()
        {
            var oauthParams = new OAuthParams();
            var oauthProfile = Secret.GetOAuthProfile();
            _externalCompanyService = new ExternalCompanyService(
                new ExternalCompanyTranslator(),
                new ExternalConnection(oauthProfile, oauthParams));
        }

        [Test]
        public void CreatesCompany()
        {
            var companyName = "Test Co" + Guid.NewGuid().ToString("N");
            var externalCompany = new ExternalCompany
            {
                Name = companyName
            };

            _externalCompanyService.Save(externalCompany);

            Assert.IsFalse(string.IsNullOrEmpty(externalCompany.Id));
            var loadedExternalCompany = _externalCompanyService.GetAll()
                .SingleOrDefault(company => company.Id == externalCompany.Id);
            Assert.IsNotNull(loadedExternalCompany);
            Assert.AreEqual(companyName, loadedExternalCompany.Name);
        }
    }
}