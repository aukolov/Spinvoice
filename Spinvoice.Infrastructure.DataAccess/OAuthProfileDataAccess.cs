using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.Infrastructure.DataAccess
{
    public class OAuthProfileDataAccess : BaseDataAccess<OAuthProfile>, IOAuthProfileDataAccess
    {
        public OAuthProfileDataAccess(IDocumentStoreContainer documentStoreContainer)
            : base(documentStoreContainer)
        {
        }
    }
}