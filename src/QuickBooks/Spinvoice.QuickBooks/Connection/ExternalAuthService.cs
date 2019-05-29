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

        public bool TryConnect(
            out ServiceContext serviceContext,
            IOAuthProfile authProfile,
            IOAuthParams oauthParams)
        {
            if (!authProfile.IsReady)
            {
                serviceContext = null;
                return false;
            }
            var oauthRequestValidator = new OAuth2RequestValidator(authProfile.AccessToken);
            serviceContext = new ServiceContext(
                authProfile.RealmId,
                IntuitServicesType.QBO,
                oauthRequestValidator);
            serviceContext.IppConfiguration.BaseUrl.Qbo = "https://sandbox-quickbooks.api.intuit.com/";
            serviceContext.IppConfiguration.MinorVersion.Qbo = "12";

            var dataService = new DataService(serviceContext);

            try
            {
                dataService.FindAll(new Intuit.Ipp.Data.Account(), 1, 1);
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