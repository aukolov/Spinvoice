using System.Collections.Generic;
using System.Linq;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.Utils;

namespace Spinvoice.IntegrationTests.Mocks
{
    public class CompanyDataAccessMock : ICompanyDataAccess
    {
        private readonly List<Company> _companies = new List<Company>();
        private int _idSequence = 1;

        public void Dispose()
        {
            
        }

        public Company[] GetAll()
        {
            return _companies.ToArray();
        }

        public Company Get(string id)
        {
            return _companies.SingleOrDefault(company => company.Id == id);
        }

        public void AddOrUpdate(Company company)
        {
            if (company.Id != null) return;

            company.Id = _idSequence.ToString();
            _idSequence++;
            _companies.Add(company);
        }

        public void DeleteAll()
        {
            _companies.Clear();
        }

        public void AddOrUpdate(IEnumerable<Company> entities)
        {
            entities.ForEach(AddOrUpdate);
        }
    }
}