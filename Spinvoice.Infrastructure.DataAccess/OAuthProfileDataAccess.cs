using Spinvoice.Domain.ExternalBook;

namespace Spinvoice.Infrastructure.DataAccess
{
    public class OAuthProfileDataAccess : BaseDataAccess<OAuthProfile>, IOAuthProfileDataAccess
    {
        public OAuthProfileDataAccess(IDocumentStoreRepository documentStoreRepository)
            : base(documentStoreRepository)
        {
        }
    }
}