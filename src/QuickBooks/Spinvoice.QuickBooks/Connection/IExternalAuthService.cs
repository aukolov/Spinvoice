using Intuit.Ipp.Core;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.Connection
{
    public interface IExternalAuthService
    {
        bool TryConnect(out ServiceContext serviceContext, IOAuthProfile authProfile, IOAuthParams oauthParams);
    }
}