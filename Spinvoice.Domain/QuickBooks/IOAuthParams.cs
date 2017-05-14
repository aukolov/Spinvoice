namespace Spinvoice.Domain.QuickBooks
{
    public interface IOAuthParams
    {
        string ConsumerKey { get; }
        string ConsumerSecret { get; }
        string UserAuthUrl { get; }
        string RequestTokenUrl { get; }
        string AccessTokenUrl { get; }
    }
}