using System;

namespace Spinvoice.Domain.QuickBooks
{
    public class OAuthProfile
    {
        public string AccessToken { get; set; }
        public string AccessSecret { get; set; }
        public string RealmId { get; set; }
        public string DataSource { get; set; }
        public DateTime ExpirationDateTime { get; set; }
    }
}