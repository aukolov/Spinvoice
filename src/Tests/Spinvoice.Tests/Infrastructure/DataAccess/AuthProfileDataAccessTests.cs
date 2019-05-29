using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Spinvoice.Infrastructure.DataAccess;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.Tests.Infrastructure.DataAccess
{
    [TestFixture]
    public class AuthProfileDataAccessTests
    {
        private AuthProfileDataAccess _authProfileDataAccess;
        private DataDirectoryProvider _dataDirectoryProvider;

        [SetUp]
        public void Setup()
        {
            _dataDirectoryProvider = new DataDirectoryProvider("TestData");
            DropDatabaseFiles();
            var documentStoreRepository = new DocumentStoreContainer(_dataDirectoryProvider);
            _authProfileDataAccess = new AuthProfileDataAccess(documentStoreRepository);
            _authProfileDataAccess.DeleteAll();
        }

        [TearDown]
        public void TearDown()
        {
            _authProfileDataAccess?.Dispose();
            DropDatabaseFiles();
        }

        private void DropDatabaseFiles()
        {
            if (Directory.Exists(_dataDirectoryProvider.Path))
            {
                Directory.Delete(_dataDirectoryProvider.Path, true);
            }
        }

        [Test]
        public void GetsUpdatedProfile()
        {
            const string accessToken = "access_token";
            const string realmId = "123456";
            const string refreshToken = "refresh_token";
            var profile = new AuthProfile
            {
                AccessToken = accessToken,
                ExpirationDateTime = new DateTime(2019, 11, 27, 18, 32, 44, 123),
                RealmId = realmId,
                RefreshToken = refreshToken
            };
            _authProfileDataAccess.AddOrUpdate(profile);

            var loadedCompany = _authProfileDataAccess.GetAll().Single();

            Assert.AreEqual(accessToken, loadedCompany.AccessToken);
            Assert.AreEqual(realmId, loadedCompany.RealmId);
            Assert.AreEqual(refreshToken, loadedCompany.RefreshToken);
        }
    }
}