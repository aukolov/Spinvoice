using System;
using System.Collections.Generic;

namespace Spinvoice.Domain.Company
{
    public interface IBaseDataAccess<T> : IDisposable
    {
        T[] GetAll();
        T Get(string id);
        void AddOrUpdate(T company);
        void DeleteAll();
        void AddOrUpdate(IEnumerable<T> entities);
    }
}