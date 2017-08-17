using System;

namespace Spinvoice.Application.Services
{
    public interface IServerManager : IDisposable
    {
        void Start();
    }
}