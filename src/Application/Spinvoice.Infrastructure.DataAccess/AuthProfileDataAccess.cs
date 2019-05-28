using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.Infrastructure.DataAccess
{
    public class AuthProfileDataAccess : BaseDataAccess<AuthProfile>, IAuthProfileDataAccess
    {
        public AuthProfileDataAccess(IDocumentStoreContainer documentStoreContainer)
            : base(documentStoreContainer)
        {
        }
    }
}