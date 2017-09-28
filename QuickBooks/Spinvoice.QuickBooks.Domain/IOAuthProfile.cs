using System;

namespace Spinvoice.QuickBooks.Domain
{
    public interface IOAuthProfile
    {
        string AccessToken { get; }
        string AccessSecret { get; }
        string RealmId { get; }
        string DataSource { get; }
        DateTime ExpirationDateTime { get; }
        bool IsReady { get; }
        event Action Updated;

        void UpdateRealm(
            string realmId,
            string dataSource);

        void UpdateAccess(
            string accessToken,
            string accessSecret,
            DateTime expirationDateTime);
    }
}