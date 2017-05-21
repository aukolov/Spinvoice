namespace Spinvoice.QuickBooks.Web
{
    public static class QuickBooksUrlService
    {
        public static string GetExternalInviceUrl(string externalId)
        {
            // ReSharper disable once JoinDeclarationAndInitializer
            string region;
#if DEBUG
            region = "uk";
#else
            region = "uk";
#endif
            return $"https://{region}.qbo.intuit.com/app/bill?txnId={externalId}";
        }
    }
}