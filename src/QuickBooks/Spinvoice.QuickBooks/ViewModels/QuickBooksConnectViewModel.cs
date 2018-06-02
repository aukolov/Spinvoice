using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Web;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using Spinvoice.Common.Presentation;
using Spinvoice.Domain.Annotations;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.ViewModels
{
    public class QuickBooksConnectViewModel : IQuickBooksConnectViewModel
    {
        private const string DummyProtocol = "https://";
        private const string DummyHost = "www.spinvoice-dummy-host.com";
        private readonly IOAuthRepository _oauthRepository;
        private readonly IWindowManager _windowManager;
        private bool _caughtCallback;
        private string _oauthVerifier = "";

        private IToken _requestToken;
        private string _url;

        public QuickBooksConnectViewModel(
            IOAuthRepository oauthRepository,
            IWindowManager windowManager)
        {
            _oauthRepository = oauthRepository;
            _windowManager = windowManager;

            StartOAuthHandshake();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Url
        {
            get => _url;
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

            Url = _oauthRepository.Params.UserAuthUrl
                  + "?oauth_token=" + _requestToken.Token
                  + "&oauth_callback=" + UriUtility.UrlEncode(DummyProtocol + DummyHost);
        }

        public void OnNavigated(Uri uri)
        {
            if (uri.Host == DummyHost)
            {
                var query = HttpUtility.ParseQueryString(uri.Query);
                _oauthVerifier = query["oauth_verifier"];
                _oauthRepository.Profile.UpdateRealm(
                    query["realmId"],
                    query["dataSource"]);
                _caughtCallback = true;
                Url = "about:blank";
            }
            else if (_caughtCallback)
            {
                var accessToken = ExchangeRequestTokenForAccessToken(_requestToken);
                using (_oauthRepository.GetProfileForUpdate(out var profile))
                {
                    profile.UpdateAccess(
                        accessToken.Token,
                        accessToken.TokenSecret,
                        DateTime.Now.AddMonths(6));
                }
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
                    ConsumerKey = _oauthRepository.Params.ConsumerKey,
                    ConsumerSecret = _oauthRepository.Params.ConsumerSecret,
                    SignatureMethod = SignatureMethod.HmacSha1
                },
                _oauthRepository.Params.RequestTokenUrl,
                _oauthRepository.Params.UserAuthUrl,
                _oauthRepository.Params.AccessTokenUrl);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}