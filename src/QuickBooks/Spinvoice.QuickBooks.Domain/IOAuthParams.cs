namespace Spinvoice.QuickBooks.Domain
{
    public interface IOAuthParams
    {
        string ClientId { get; }
        string ClientSecret { get; }
        string UserAuthUrl { get; }
        string RequestTokenUrl { get; }
        string AccessTokenUrl { get; }
    }
}