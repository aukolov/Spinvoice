using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using Spinvoice.Utils;

namespace Spinvoice.Domain.Company
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ICompanyDataAccess _companyDataAccess;
        private readonly ObservableCollection<Company> _companies;

        public CompanyRepository(ICompanyDataAccess companyDataAccess)
        {
            _companyDataAccess = companyDataAccess;
            _companies = new ObservableCollection<Company>();
            foreach (var company in companyDataAccess.GetAll())
            {
                _companies.Add(company);
            }
        }

        public Company GetByName(string name)
        {
            return _companies.FirstOrDefault(company => company.Name == name);
        }

        public IDisposable GetByNameForUpdateOrCreate(string name, out Company company)
        {
            company = GetByName(name);
            if (company == null)
            {
                company = new Company
                {
                    Name = name
                };
                _companies.Add(company);
            }
            var localCompany = company;
            return new RelayDisposable(() => _companyDataAccess.AddOrUpdate(localCompany));
        }

        public Company[] GetAll()
        {
            return _companyDataAccess.GetAll();
        }
    }
}