using NUnit.Framework;
using Spinvoice.Domain.Company;
using Spinvoice.Infrastructure.DataAccess;

namespace Spinvoice.Tests.Infrastructure.DataAccess
{
    [TestFixture]
    public class CompanyDataAccessTests
    {
        private CompanyDataAccess _companyDataAccess;

        [SetUp]
        public void Setup()
        {
            _companyDataAccess = new CompanyDataAccess();
            _companyDataAccess.DeleteAll();
        }

        [TearDown]
        public void TearDown()
        {
            _companyDataAccess.DeleteAll();
            _companyDataAccess?.Dispose();
        }

        public string GetCaller([System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            return memberName;
        }

        [Test]
        public void GetsEmptyCompanyArrayIfNothingAdded()
        {
            var companies = _companyDataAccess.GetAll();

            Assert.AreEqual(0, companies.Length);
        }

        [Test]
        public void GetsAddedCompany()
        {
            var company = new Company
            {
                Name = GetCaller()
            };
            _companyDataAccess.Add(company);

            var loadedCompany = _companyDataAccess.Get(company.Id);

            Assert.IsNotNull(loadedCompany);
        }

        [Test]
        public void AssignsIdOnAdd()
        {
            var company = new Company
            {
                Name = GetCaller()
            };
            _companyDataAccess.Add(company);

            Assert.IsTrue(!string.IsNullOrEmpty(company.Id));
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
            _companyDataAccess.Add(company);

            var loadedCompany = _companyDataAccess.Get(company.Id);

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
            _companyDataAccess.Add(company);

            var companies = _companyDataAccess.GetAll();

            Assert.AreEqual(1, companies.Length);
            Assert.AreEqual("GetsAllCompaniesIncludingAddedCompany", companies[0].Name);
        }

        [Test]
        public void Gets20AddedCompanies()
        {
            for (var i = 0; i < 20; i++)
            {
                _companyDataAccess.Add(new Company
                {
                    Name = GetCaller() + i
                });
            }

            var companies = _companyDataAccess.GetAll();

            Assert.AreEqual(20, companies.Length);
        }
    }
}