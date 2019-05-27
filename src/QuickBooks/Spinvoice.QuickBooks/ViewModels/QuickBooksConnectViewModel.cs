using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using Intuit.Ipp.OAuth2PlatformClient;
using Spinvoice.Common.Presentation;
using Spinvoice.Domain.Annotations;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.ViewModels
{
    public class QuickBooksConnectViewModel : IQuickBooksConnectViewModel
    {
        private const string RedirectUrl = "https://developer.intuit.com/v2/OAuth2Playground/RedirectUrl";
        private readonly IOAuthRepository _oauthRepository;
        private readonly IWindowManager _windowManager;
        private bool _caughtCallback;

        private string _url;
        private readonly OAuth2Client _oAuth2Client;
        private string _code;

        public QuickBooksConnectViewModel(
            IOAuthRepository oauthRepository,
            IWindowManager windowManager)
        {
            _oauthRepository = oauthRepository;
            _windowManager = windowManager;

            _oAuth2Client = new OAuth2Client(
                _oauthRepository.Params.ClientId,
                _oauthRepository.Params.ClientSecret,
                RedirectUrl,
                "sandbox");
            var scopes = new List<OidcScopes> { OidcScopes.Accounting };
            var authorizeUrl = _oAuth2Client.GetAuthorizationURL(scopes);
            Url = authorizeUrl;
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

        public void OnNavigating(Uri uri, out bool cancel)
        {
            if (uri.AbsoluteUri.StartsWith(RedirectUrl))
            {
                var query = HttpUtility.ParseQueryString(uri.Query);
                _oauthRepository.Profile.UpdateRealm(query["realmId"]);
                _code = query["code"];
                _caughtCallback = true;
                Url = "about:blank";

                var accessTokenTask = ExchangeRequestTokenForAccessToken();
                accessTokenTask.ContinueWith(
                    x =>
                    {
                        using (_oauthRepository.GetProfileForUpdate(out var profile))
                        {
                            profile.UpdateAccess(
                                x.Result.AccessToken,
                                x.Result.RefreshToken,
                                x.Result.IdentityToken,
                                DateTime.Now.AddMonths(6));
                        }
                    },
                    CancellationToken.None,
                    TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.FromCurrentSynchronizationContext());

                _windowManager.Close(this);
                cancel = true;
                return;
            }

            cancel = false;
        }

        private async Task<TokenResponse> ExchangeRequestTokenForAccessToken()
        {
            var token = await _oAuth2Client.GetBearerTokenAsync(_code);
            return token;
        }

        private IOAuthSession CreateOAuthSession()
        {
            return new OAuthSession(
                new OAuthConsumerContext
                {
                    ConsumerKey = _oauthRepository.Params.ClientId,
                    ConsumerSecret = _oauthRepository.Params.ClientSecret,
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