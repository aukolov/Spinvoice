using System;
using Spinvoice.Utils;

namespace Spinvoice.Domain.ExternalBook
{
    public class OAuthProfile : IOAuthProfile
    {
        public string AccessToken { get; set; }
        public string AccessSecret { get; set; }
        public string RealmId { get; set; }
        public string DataSource { get; set; }
        public DateTime ExpirationDateTime { get; set; }

        public event Action Updated;

        public bool IsReady => !string.IsNullOrEmpty(AccessToken)
                               && !string.IsNullOrEmpty(AccessSecret)
                               && !string.IsNullOrEmpty(RealmId)
                               && !string.IsNullOrEmpty(DataSource)
                               && DateTime.Now < ExpirationDateTime.AddDays(-7);

        public void UpdateRealm(
            string realmId,
            string dataSource)
        {
            RealmId = realmId;
            DataSource = dataSource;

            Updated.Raise();
        }

        public void UpdateAccess(
            string accessToken,
            string accessSecret,
            DateTime expirationDateTime)
        {
            AccessToken = accessToken;
            AccessSecret = accessSecret;
            ExpirationDateTime = expirationDateTime;

            Updated.Raise();
        }

    }
}