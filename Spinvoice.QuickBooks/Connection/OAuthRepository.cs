using System;
using System.Linq;
using Spinvoice.Domain.ExternalBook;
using Spinvoice.Utils;

namespace Spinvoice.QuickBooks.Connection
{
    public class OAuthRepository : IOAuthRepository
    {
        private readonly IOAuthProfileDataAccess _oauthProfileDataAccess;
        private readonly OAuthProfile _profile;

        public OAuthRepository(IOAuthProfileDataAccess oauthProfileDataAccess)
        {
            _oauthProfileDataAccess = oauthProfileDataAccess;
            var profiles = _oauthProfileDataAccess.GetAll();
            if (profiles.Length > 1)
            {
                _oauthProfileDataAccess.DeleteAll();
                profiles = _oauthProfileDataAccess.GetAll();
            }
            _profile = profiles.SingleOrDefault() ?? new OAuthProfile();
            Params = new OAuthParams();
        }

        public IOAuthProfile Profile => _profile;

        public IOAuthParams Params { get; }
        public IDisposable GetProfileForUpdate(out IOAuthProfile profile)
        {
            profile = _profile;
            return new RelayDisposable(() => _oauthProfileDataAccess.AddOrUpdate(_profile));
        }
    }
}