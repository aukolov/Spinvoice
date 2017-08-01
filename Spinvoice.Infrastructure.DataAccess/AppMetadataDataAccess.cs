using Spinvoice.Domain.App;

namespace Spinvoice.Infrastructure.DataAccess
{
    public class AppMetadataDataAccess : BaseDataAccess<AppMetadata>, IAppMetadataDataAccess
    {
        public AppMetadataDataAccess(
            IDocumentStoreContainer documentStoreContainer) 
            : base(documentStoreContainer)
        {
        }
    }
}