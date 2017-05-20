using System;

namespace Spinvoice.Domain.Company
{
    public interface ICompanyRepository
    {
        Company GetByName(string name);
        IDisposable GetByNameForUpdateOrCreate(string name, out Company company);
        Company[] GetAll();
    }
}