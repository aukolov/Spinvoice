using System;

namespace Spinvoice.Domain.ExternalBook
{
    public interface IOAuthRepository
    {
        IOAuthProfile Profile { get; }
        IOAuthParams Params { get; }
        IDisposable GetProfileForUpdate(out IOAuthProfile profile);
    }
}