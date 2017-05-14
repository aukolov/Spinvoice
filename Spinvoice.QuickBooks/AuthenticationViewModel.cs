using System;
using System.ComponentModel;
using System.Web;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using Spinvoice.Domain.QuickBooks;

namespace Spinvoice.QuickBooks
{
    public class AuthenticationViewModel : INotifyPropertyChanged
    {
        private const string DummyProtocol = "https://";
        private const string DummyHost = "www.spinvoice-dummy-host.com";

        private readonly OAuthProfile _oauthProfile;
        private readonly IOAuthParams _oauthParams;

        private IToken _requestToken;
        private string _oauthVerifier = "";
        private bool _caughtCallback;

        public AuthenticationViewModel(
            OAuthProfile oauthProfile,
            IOAuthParams oauthParams)
        {
            _oauthProfile = oauthProfile;
            _oauthParams = oauthParams;
            StartOAuthHandshake();
        }

        private void StartOAuthHandshake()
        {
            var oauthSession = CreateOAuthSession();
            _requestToken = oauthSession.GetRequestToken();

            oauthBrowser.Navigate(_oauthParams.UserAuthUrl
                                  + "?oauth_token=" + _requestToken.Token
                                  + "&oauth_callback=" + UriUtility.UrlEncode(DummyProtocol + DummyHost));
        }

        private void oauthBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.Host == DummyHost)
            {
                var query = HttpUtility.ParseQueryString(e.Url.Query);
                _oauthVerifier = query["oauth_verifier"];
                _oauthProfile.RealmId = query["realmId"];
                _oauthProfile.DataSource = query["dataSource"];
                _caughtCallback = true;
                oauthBrowser.Navigate("about:blank");
            }
            else if (_caughtCallback)
            {
                var accessToken = ExchangeRequestTokenForAccessToken(_requestToken);
                _oauthProfile.AccessToken = accessToken.Token;
                _oauthProfile.AccessSecret = accessToken.TokenSecret;
                _oauthProfile.ExpirationDateTime = DateTime.Now.AddMonths(6);
                Close();
            }
        }

        private IToken ExchangeRequestTokenForAccessToken(IToken requestToken)
        {
            var oauthSession = CreateOAuthSession();
            return oauthSession.ExchangeRequestTokenForAccessToken(requestToken, _oauthVerifier);
        }

        private IOAuthSession CreateOAuthSession()
        {
            return new OAuthSession(
                new OAuthConsumerContext
                {
                    ConsumerKey = _oauthParams.ConsumerKey,
                    ConsumerSecret = _oauthParams.ConsumerSecret,
                    SignatureMethod = SignatureMethod.HmacSha1
                },
                _oauthParams.RequestTokenUrl,
                _oauthParams.UserAuthUrl,
                _oauthParams.AccessTokenUrl);
        }
    }
}