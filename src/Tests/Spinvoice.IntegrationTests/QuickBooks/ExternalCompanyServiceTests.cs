using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Spinvoice.QuickBooks.Company;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.IntegrationTests.QuickBooks
{
    [TestFixture]
    public class ExternalCompanyServiceTests
    {
        private ExternalCompanyRepository _externalCompanyRepository;

        [SetUp]
        public void Setup()
        {
            var oathRepositoryMock = new Mock<IOAuthRepository>();
            oathRepositoryMock.Setup(repository => repository.Profile).Returns(QuickBooksUtils.GetOAuthProfile());
            oathRepositoryMock.Setup(repository => repository.Params).Returns(new OAuthParams());

            _externalCompanyRepository = new ExternalCompanyRepository(
                new ExternalConnection(oathRepositoryMock.Object, new ExternalAuthService()));
        }

        [Test]
        public void CreatesCompany()
        {
            var companyName = "Test Co " + Guid.NewGuid().ToString("N");

            var externalCompany = _externalCompanyRepository.Create(companyName, Side.Vendor, "USD");

            Assert.IsFalse(string.IsNullOrEmpty(externalCompany.Id));
            var loadedExternalCompany = _externalCompanyRepository.GetAllVendors()
                .SingleOrDefault(company => company.Id == externalCompany.Id);
            Assert.IsNotNull(loadedExternalCompany);
            Assert.AreEqual(companyName, loadedExternalCompany.Name);
        }
    }
}