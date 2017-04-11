using System.Collections.ObjectModel;

namespace Spinvoice.Domain.Company
{
    public class CompanyRepository
    {
        private readonly ICompanyDataAccess _companyDataAccess;

        public CompanyRepository(ICompanyDataAccess companyDataAccess)
        {
            _companyDataAccess = companyDataAccess;
            Companies = new ObservableCollection<Company>();
            foreach (var company in companyDataAccess.GetAll())
            {
                Companies.Add(company);
            }
        }

        public ObservableCollection<Company> Companies { get; private set; }
    }
}