using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Spinvoice.Domain.ExternalBook;
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
            var oathRepositoryMock = new Mock<IOAuthRepository>();
            oathRepositoryMock.Setup(repository => repository.Profile).Returns(Secret.GetOAuthProfile());
            oathRepositoryMock.Setup(repository => repository.Params).Returns(new OAuthParams());

            _externalCompanyService = new ExternalCompanyService(
                new ExternalCompanyTranslator(),
                new ExternalConnection(oathRepositoryMock.Object));
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