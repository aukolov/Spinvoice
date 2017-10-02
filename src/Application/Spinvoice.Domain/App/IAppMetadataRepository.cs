using System;

namespace Spinvoice.Domain.App
{
    public interface IAppMetadataRepository
    {
        AppMetadata Get();
        IDisposable GetForUpdate(out AppMetadata appMetadata);
    }
}