using Spinvoice.QuickBooks.Domain;

namespace QuickBooksAuth
{
    internal class TestAuthProfileDataAccess : IAuthProfileDataAccess
    {
        private AuthProfile _profile;

        public TestAuthProfileDataAccess()
        {
            _profile = new AuthProfile();
        }

        public AuthProfile[] GetAll()
        {
            return new[] { _profile };
        }

        public void AddOrUpdate(AuthProfile entity)
        {
            _profile = entity;
        }

        public void DeleteAll()
        {
        }
    }
}