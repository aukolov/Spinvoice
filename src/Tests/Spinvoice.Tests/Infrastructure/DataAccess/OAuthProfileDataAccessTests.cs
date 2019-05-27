using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Spinvoice.Domain.Company;
using Spinvoice.Infrastructure.DataAccess;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.Tests.Infrastructure.DataAccess
{
    [TestFixture]
    public class OAuthProfileDataAccessTests
    {
        private OAuthProfileDataAccess _oAuthProfileDataAccess;
        private DataDirectoryProvider _dataDirectoryProvider;

        [SetUp]
        public void Setup()
        {
            _dataDirectoryProvider = new DataDirectoryProvider("TestData");
            DropDatabaseFiles();
            var documentStoreRepository = new DocumentStoreContainer(_dataDirectoryProvider);
            _oAuthProfileDataAccess = new OAuthProfileDataAccess(documentStoreRepository);
            _oAuthProfileDataAccess.DeleteAll();
        }

        [TearDown]
        public void TearDown()
        {
            _oAuthProfileDataAccess?.Dispose();
            DropDatabaseFiles();
        }

        private void DropDatabaseFiles()
        {
            if (Directory.Exists(_dataDirectoryProvider.Path))
            {
                Directory.Delete(_dataDirectoryProvider.Path, true);
            }
        }

        public string GetCaller([System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            return memberName;
        }

        [Test]
        public void GetsUpdatedProfile()
        {
            var profile = new OAuthProfile
            {
                AccessToken = "access-token",
                RealmId = "realm-id"
            };
            _oAuthProfileDataAccess.AddOrUpdate(profile);

            var loadedCompany = _oAuthProfileDataAccess.GetAll().Single();

            Assert.AreEqual("access-token", loadedCompany.AccessToken);
            Assert.AreEqual("realm-id", loadedCompany.RealmId);
        }

        [Test]
        public void AssignsIdOnAdd()
        {
            var profile = new OAuthProfile
            {
                AccessToken = "access-token",
                RealmId = "realm-id"
            };
            _oAuthProfileDataAccess.AddOrUpdate(profile);

            Assert.IsTrue(!string.IsNullOrEmpty(profile.Id));
        }

        [Test]
        public void GetsAddedCompanyWithAllFields()
        {
            var company = new Company
            {
                Name = GetCaller(),
                Country = "Germany",
                Currency = "EUR"
            };
            _oAuthProfileDataAccess.AddOrUpdate(company);

            var loadedCompany = _oAuthProfileDataAccess.Get(company.Id);

            Assert.IsNotNull(loadedCompany);
            Assert.AreEqual(company.Id, loadedCompany.Id);
            Assert.AreEqual("GetsAddedCompanyWithAllFields", loadedCompany.Name);
            Assert.AreEqual("Germany", loadedCompany.Country);
            Assert.AreEqual("EUR", loadedCompany.Currency);
        }

        [Test]
        public void GetsAllCompaniesIncludingAddedCompany()
        {
            var company = new Company
            {
                Name = GetCaller(),
                Country = "Germany",
                Currency = "EUR"
            };
            _oAuthProfileDataAccess.AddOrUpdate(company);

            var companies = _oAuthProfileDataAccess.GetAll();

            Assert.AreEqual(1, companies.Length);
            Assert.AreEqual("GetsAllCompaniesIncludingAddedCompany", companies[0].Name);
        }

        [Test]
        public void Gets20AddedCompanies()
        {
            for (var i = 0; i < 20; i++)
            {
                _oAuthProfileDataAccess.AddOrUpdate(new Company
                {
                    Name = GetCaller() + i
                });
            }

            var companies = _oAuthProfileDataAccess.GetAll();

            Assert.AreEqual(20, companies.Length);
        }

        [Test]
        public void UpdatesSameCompany()
        {
            var company = new Company
            {
                Name = "name1"
            };
            _oAuthProfileDataAccess.AddOrUpdate(company);
            var companyId = company.Id;

            company.Name = "name2";
            _oAuthProfileDataAccess.AddOrUpdate(company);

            Assert.AreEqual(companyId, company.Id);
        }

        [Test]
        public void UpdatesExistingCompany()
        {
            var company = new Company
            {
                Country = "Cyprus",
                Currency = "EUR",
                Name = "Name1"
            };
            _oAuthProfileDataAccess.AddOrUpdate(company);

            company.Country = "Russia";
            company.Currency = "RUB";
            company.Name = "Name2";
            _oAuthProfileDataAccess.AddOrUpdate(company);

            var loadedCompany = _oAuthProfileDataAccess.Get(company.Id);
            Assert.AreEqual("Russia", loadedCompany.Country);
            Assert.AreEqual("RUB", loadedCompany.Currency);
            Assert.AreEqual("Name2", loadedCompany.Name);
        }

        [Test]
        public void DoesNotCreateNewCompanyOnUpdate()
        {
            var company = new Company
            {
                Name = "name1"
            };
            _oAuthProfileDataAccess.AddOrUpdate(company);
            Console.WriteLine($@"Company ID: {company.Id}");

            company.Name = "name2";
            _oAuthProfileDataAccess.AddOrUpdate(company);
            Console.WriteLine($@"Company ID: {company.Id}");

            var companies = _oAuthProfileDataAccess.GetAll();
            Assert.AreEqual(1, companies.Length);
        }
    }
}