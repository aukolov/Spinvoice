using System;

namespace Spinvoice.Domain.Company
{
    public interface ICompanyDataAccess : IDisposable
    {
        Domain.Company.Company[] GetAll();
    }
}