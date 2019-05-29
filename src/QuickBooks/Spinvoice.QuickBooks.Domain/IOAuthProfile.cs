using System;

namespace Spinvoice.QuickBooks.Domain
{
    public interface IOAuthProfile
    {
        string AccessToken { get; }
        string RefreshToken { get; }
        string RealmId { get; }
        DateTime ExpirationDateTime { get; }
        bool IsReady { get; }
        event Action Updated;

        void UpdateRealm(string dataSource);

        void UpdateAccess(
            string accessToken,
            string refreshToken,
            DateTime expirationDateTime);
    }
}