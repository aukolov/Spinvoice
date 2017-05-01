using System;
using System.Linq;
using Spinvoice.Domain;
using Spinvoice.Domain.App;
using Spinvoice.Utils;

namespace Spinvoice.Infrastructure.DataAccess
{
    public class AppMetadataRepository : IAppMetadataRepository
    {
        private readonly IAppMetadataDataAccess _appMetadataDataAccess;

        public AppMetadataRepository(IAppMetadataDataAccess appMetadataDataAccess)
        {
            _appMetadataDataAccess = appMetadataDataAccess;
        }

        public AppMetadata Get()
        {
            var appMetadata = _appMetadataDataAccess.GetAll().FirstOrDefault();
            if (appMetadata == null)
            {
                appMetadata = new AppMetadata();
                _appMetadataDataAccess.AddOrUpdate(appMetadata);
            }
            return appMetadata;
        }

        public IDisposable GetForUpdate(out AppMetadata appMetadata)
        {
            appMetadata = Get();
            var localAppMetadata = appMetadata;
            return new RelayDisposable(() => _appMetadataDataAccess.AddOrUpdate(localAppMetadata));
        }

    }
}