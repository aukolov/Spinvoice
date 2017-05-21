using System;

namespace Spinvoice.Domain.ExternalBook
{
    public interface IExternalConnectionWatcher
    {
        event Action Connected;
        bool IsConnected { get; }
    }
}