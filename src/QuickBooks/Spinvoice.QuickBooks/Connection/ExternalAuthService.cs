using System;
using Intuit.Ipp.Core;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Security;
using NLog;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.Connection
{
    public class ExternalAuthService : IExternalAuthService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public bool TryConnect(out ServiceContext serviceContext, IOAuthProfile oauthRepositoryProfile, IOAuthParams oauthRepositoryParams)
        {
            if (!oauthRepositoryProfile.IsReady)
            {
                serviceContext = null;
                return false;
            }
            var oauthRequestValidator = new OAuthRequestValidator(
                oauthRepositoryProfile.AccessToken,
                oauthRepositoryProfile.AccessSecret,
                oauthRepositoryParams.ClientId,
                oauthRepositoryParams.ClientSecret);
            serviceContext = new ServiceContext(
                oauthRepositoryProfile.RealmId,
                IntuitServicesType.QBO,
                oauthRequestValidator);
            var dataService = new DataService(serviceContext);

            try
            {
                dataService.FindAll(new Intuit.Ipp.Data.Account());
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error when testing connection with QuickBooks.");
                return false;
            }
            return true;
        }
    }
}