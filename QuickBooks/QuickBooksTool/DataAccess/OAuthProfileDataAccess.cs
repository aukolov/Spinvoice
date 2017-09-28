using Spinvoice.QuickBooks.Domain;

namespace QuickBooksTool.DataAccess
{
    public class OAuthProfileDataAccess : IOAuthProfileDataAccess
    {
        private OAuthProfile _profile;

        public OAuthProfileDataAccess()
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
