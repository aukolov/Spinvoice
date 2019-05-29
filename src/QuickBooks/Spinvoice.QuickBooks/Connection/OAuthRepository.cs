using System;
using System.Linq;
using Spinvoice.QuickBooks.Domain;
using Spinvoice.Utils;

namespace Spinvoice.QuickBooks.Connection
{
    public class OAuthRepository : IOAuthRepository
    {
        private readonly IAuthProfileDataAccess _oauthProfileDataAccess;
        private readonly AuthProfile _profile;

        public OAuthRepository(IAuthProfileDataAccess oauthProfileDataAccess)
        {
            _oauthProfileDataAccess = oauthProfileDataAccess;
            var profiles = _oauthProfileDataAccess.GetAll();
            if (profiles.Length > 1)
            {
                _oauthProfileDataAccess.DeleteAll();
                profiles = _oauthProfileDataAccess.GetAll();
            }
            _profile = profiles.SingleOrDefault() ?? new AuthProfile();
            Params = new OAuthParams();
        }

        public IOAuthProfile Profile => _profile;

        public IOAuthParams Params { get; }
        public IDisposable GetProfileForUpdate(out IOAuthProfile profile)
        {
            profile = _profile;
            return new RelayDisposable(() => {});
        }
    }
}