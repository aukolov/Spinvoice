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
            const string accessToken = "eyJlbmMiOiJBMTI4Q0JDLUhTMjU2IiwiYWxnIjoiZGlyIn0..85zlk6rxvAD4XobBNUfB5w.RrMhOQPetKMsJkRXrpg1RMckZBu9JQ4LZVoEMGlAQUoCQcVqDs1bJqMRQppd1WVaXQwLmnUV1k6KmmatRoshO5Gn_X0mvLeFzCy-wH8qaZPPD6d_omB-7y8crkUcAc355fN33o6Ga192GnL1sIEelkZIWi9LS8VT3YhU945N-6HG1-GZIKzwKLzAhfcOuQNJ5Z2DcNiycR67ctWtBc_7p0VOqLBETox-dVsE5QegVkkSYcWZwaXbmK1V0Eqv7ODUZX3AoGpMdv1dEgiS2RthfDUIiQ5etIQAW9ouSIKqbwAmLyVt1-71eD_-I9cRM4Pn1L7AD96VhLjI0ME0hQMLdPEd8n062vkzNXPWD1OEy4s_1Kdmi92_CDLi-jCGp4sh67VUYu2fwO87caXE9-uyG7eDB5tzK1rVRwQUt_mH8oTOGy7xQYxPOnbYwAZixXkE8_xDCXIN34HKk6KVY0-tzhssEsXOLrdkGyyrLvH6lNrtpq4xSev92cZChu4wrkV3W8RcJKMCV-I397D3kRSBKynFKFEBD7YA3iglkMVntsw9JsycKTdHeUMGnPr6V1sPdHFrykzZ1GvsbtewXCfnr0eGu2rN5Y9rHSX61DEb7OkAjvaq-66qx0E0nuwlzahRiAubuK2mOW-eu4mR9eQUpJf-lhrv2cY_nuIYBBo7164.aPU-glhI6uDMstFilu1ttQ";
            const string realmId = "123145830825719";
            const string refreshToken = "Q011567697257hLSuvjgPlFMLKBp8gib9U4wLz5bb310uyKcRl";
            var profile = new AuthProfile
            {
                AccessToken = accessToken,
                ExpirationDateTime = new DateTime(2019, 11, 27, 18, 32, 44, 123),
                IdentityToken = null,
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