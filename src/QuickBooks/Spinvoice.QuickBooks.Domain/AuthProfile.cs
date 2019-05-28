using System;
using Spinvoice.Utils;

namespace Spinvoice.QuickBooks.Domain
{
    public class AuthProfile : IOAuthProfile
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string IdentityToken { get; set; }
        public string RealmId { get; set; }
        public DateTime ExpirationDateTime { get; set; }

        public event Action Updated;

        public bool IsReady => !string.IsNullOrEmpty(AccessToken)
                               && !string.IsNullOrEmpty(RefreshToken)
                               && !string.IsNullOrEmpty(RealmId)
                               && DateTime.Now < ExpirationDateTime.AddDays(-7);

        public void UpdateRealm(string realmId)
        {
            RealmId = realmId;
            Updated.Raise();
        }

        public void UpdateAccess(
            string accessToken,
            string refreshToken,
            string identityToken,
            DateTime expirationDateTime)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            IdentityToken = identityToken;
            ExpirationDateTime = expirationDateTime;

            Updated.Raise();
        }

    }
}