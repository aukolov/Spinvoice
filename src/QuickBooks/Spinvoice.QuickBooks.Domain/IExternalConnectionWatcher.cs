using System;

namespace Spinvoice.QuickBooks.Domain
{
    public interface IExternalConnectionWatcher
    {
        event Action Connected;
        bool IsConnected { get; }
    }
}