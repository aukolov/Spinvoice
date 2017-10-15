using System;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.Web
{
    public static class QuickBooksUrlService
    {
        public static string GetExternalInvoiceUrl(Side side, string externalId)
        {
            // ReSharper disable once JoinDeclarationAndInitializer
            string region;
#if DEBUG
            region = "sandbox";
#else
            region = "uk";
#endif
            string entityName;
            switch (side)
            {
                case Side.Vendor:
                    entityName = "bill";
                    break;
                case Side.Customer:
                    entityName = "invoice";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
            return $"https://{region}.qbo.intuit.com/app/{entityName}?txnId={externalId}";
        }
    }
}