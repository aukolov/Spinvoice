using System;

namespace Spinvoice.Application.Services
{
    public interface IServerApplicationService : IDisposable
    {
        void Start();
    }
}