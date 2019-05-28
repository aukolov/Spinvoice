using Spinvoice.QuickBooks.Domain;

namespace QuickBooksTool.DataAccess
{
    public class AuthProfileDataAccess : IAuthProfileDataAccess
    {
        private AuthProfile _profile;

        public AuthProfileDataAccess()
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
