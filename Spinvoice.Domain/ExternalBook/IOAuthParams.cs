namespace Spinvoice.Domain.ExternalBook
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