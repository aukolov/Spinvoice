using Spinvoice.QuickBooks.Domain;

namespace QuickBooksAuth
{
    internal class TestOAuthProfileDataAccess : IOAuthProfileDataAccess
    {
        private OAuthProfile _profile;

        public TestOAuthProfileDataAccess()
        {
            _profile = new OAuthProfile();
        }

        public OAuthProfile[] GetAll()
        {
            return new[] { _profile };
        }

        public void AddOrUpdate(OAuthProfile entity)
        {
            _profile = entity;
        }

        public void DeleteAll()
        {
        }
    }
}