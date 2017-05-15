using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Web;
using System.Windows.Input;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using Spinvoice.Domain.Annotations;
using Spinvoice.Domain.QuickBooks;
using Spinvoice.Domain.UI;
using Spinvoice.Utils;

namespace Spinvoice.QuickBooks
{
    public class QuickBooksConnectViewModel : INotifyPropertyChanged
    {
        private const string DummyProtocol = "https://";
        private const string DummyHost = "www.spinvoice-dummy-host.com";
        private readonly IOAuthParams _oauthParams;
        private readonly IWindowManager _windowManager;

        private readonly OAuthProfile _oauthProfile;
        private bool _caughtCallback;
        private string _oauthVerifier = "";

        private IToken _requestToken;
        private string _url;

        public QuickBooksConnectViewModel(
            OAuthProfile oauthProfile,
            IOAuthParams oauthParams,
            IWindowManager windowManager)
        {
            _oauthProfile = oauthProfile;
            _oauthParams = oauthParams;
            _windowManager = windowManager;

            StartOAuthHandshake();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Url
        {
            get { return _url; }
            set
            {
                if (_url == value) return;
                _url = value;
                OnPropertyChanged();
            }
        }

        private void StartOAuthHandshake()
        {
            var oauthSession = CreateOAuthSession();
            _requestToken = oauthSession.GetRequestToken();

            Url = _oauthParams.UserAuthUrl
                  + "?oauth_token=" + _requestToken.Token
                  + "&oauth_callback=" + UriUtility.UrlEncode(DummyProtocol + DummyHost);
        }

        public void OnNavigated(Uri uri)
        {
            if (uri.Host == DummyHost)
            {
                var query = HttpUtility.ParseQueryString(uri.Query);
                _oauthVerifier = query["oauth_verifier"];
                _oauthProfile.RealmId = query["realmId"];
                _oauthProfile.DataSource = query["dataSource"];
                _caughtCallback = true;
                Url = "about:blank";
            }
            else if (_caughtCallback)
            {
                var accessToken = ExchangeRequestTokenForAccessToken(_requestToken);
                _oauthProfile.AccessToken = accessToken.Token;
                _oauthProfile.AccessSecret = accessToken.TokenSecret;
                _oauthProfile.ExpirationDateTime = DateTime.Now.AddMonths(6);
                _windowManager.Close(this);
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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}