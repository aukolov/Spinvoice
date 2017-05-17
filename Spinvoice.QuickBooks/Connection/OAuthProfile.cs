using System;
using Spinvoice.Utils;

namespace Spinvoice.QuickBooks.Connection
{
    public class OAuthProfile
    {
        public string AccessToken { get; private set; }
        public string AccessSecret { get; private set; }
        public string RealmId { get; private set; }
        public string DataSource { get; private set; }
        public DateTime ExpirationDateTime { get; private set; }

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