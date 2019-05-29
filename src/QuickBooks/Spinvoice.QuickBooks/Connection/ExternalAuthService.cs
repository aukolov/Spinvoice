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
            var oauthRequestValidator = new OAuth2RequestValidator("eyJlbmMiOiJBMTI4Q0JDLUhTMjU2IiwiYWxnIjoiZGlyIn0..BAWMD7cXVmFazkncCfjxog.WR0HWjgXVT4mUnAJsnPz7IkcR1wEpc3ZJFT59uiwH6D7RvxIPHBdw35wb8eqyJfbo9OZwWLE90F8HsCzm3CEMb3pridePMZvftPmLzd9RBqOmQyYdEZRo8yinrGIxJ_IFvH44rhkID1M9JM3O2ZmiB5rK2k530rGYBzFyvUvTDpGHty1J5phLdlalrn9kU3k71n7MYLEjPwJdH2zCoCM9WCnEcGNn7GxJpy8_DDx23avhDIneOyQ9rW9YQvLoF_qi-EGp4ujwPn3KRZlVYpv8RDjMR3e7zSE4j2cUACDDkb_SX56_jsamgRzjJCYo3v9-6A2woWXVYEeHrCTZaENq7fPPRbWzwtRB50RG9awcJdoHWnAUfdMs-p3P0y-f9HNrwC1BRIDyAgobZYMhXMyIt8SGjCKABjH9vGK2O0Dp-kzUPcab9UZBS4GlPV0wD_jbVKT_LvCjmi7YlCXW2ePidZo2vNMT6dm-m_H3u4t7R64rEdVT9HfYcX765s60Kirio-K8__Z5Gh_C5WJYk8Wxmp3WPnIBrSZ4wJtbsTeZaxEO3doTdmc2oViA_HG0_49i088g39fqFZuzGqVEH_w1R3D0LgH-QHgPVQ-utuLA5jA_NbiD6I1e-WlzHGW_7UKNoTCLJcg6QrBvFBvnW04UMBg778JFmTDHA6_aZ-M4N0.i5DtenTIuo3-pQN2YXtrjw");
            serviceContext = new ServiceContext(
                authProfile.RealmId,
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