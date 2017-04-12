using System;

namespace Spinvoice.Domain.Company
{
    public interface ICompanyDataAccess : IDisposable
    {
        Company[] GetAll();
        Company Get(string id);
        void AddOrUpdate(Company company);
        void DeleteAll();
    }
}