using System;

namespace Spinvoice.QuickBooks.Domain
{
    public interface IOAuthRepository
    {
        IOAuthProfile Profile { get; }
        IOAuthParams Params { get; }
        IDisposable GetProfileForUpdate(out IOAuthProfile profile);
    }
}