namespace Spinvoice.QuickBooks.Domain
{
    public interface IOAuthParams
    {
        string ClientId { get; }
        string ClientSecret { get; }
        string Region { get; }
        string BaseUrl { get; }
    }
}