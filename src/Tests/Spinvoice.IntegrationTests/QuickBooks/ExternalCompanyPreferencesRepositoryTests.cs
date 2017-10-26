using Moq;
using NUnit.Framework;
using Spinvoice.QuickBooks.Company;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.IntegrationTests.QuickBooks
{
    public class ExternalCompanyPreferencesRepositoryTests
    {
        private IExternalCompanyPreferencesRepository _externalCompanyRepository;

        [SetUp]
        public void Setup()
        {
            var oathRepositoryMock = new Mock<IOAuthRepository>();
            oathRepositoryMock.Setup(repository => repository.Profile).Returns(QuickBooksUtils.GetOAuthProfile());
            oathRepositoryMock.Setup(repository => repository.Params).Returns(new OAuthParams());

            _externalCompanyRepository = new ExternalCompanyPreferencesRepository(
                new ExternalConnection(oathRepositoryMock.Object, new ExternalAuthService()));
        }

        [Test]
        public void GetsHomeCurrency()
        {
            var homeCurrency = _externalCompanyRepository.HomeCurrency;
            Assert.AreEqual("USD", homeCurrency);
        }
    }
}