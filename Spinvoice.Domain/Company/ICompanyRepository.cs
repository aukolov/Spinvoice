using System;
using System.Collections.ObjectModel;

namespace Spinvoice.Domain.Company
{
    public interface ICompanyRepository
    {
        Company GetByName(string name);
        IDisposable GetByNameForUpdateOrCreate(string name, out Company company);
    }
}