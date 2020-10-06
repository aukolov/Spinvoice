using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Intuit.Ipp.OAuth2PlatformClient;
using Spinvoice.Common.Presentation;
using Spinvoice.Domain.Annotations;
using Spinvoice.QuickBooks.Domain;
using Spinvoice.Utils;

namespace Spinvoice.QuickBooks.ViewModels
{
    public class QuickBooksConnectViewModel : IQuickBooksConnectViewModel
    {
        private const string RedirectUrl = "https://ushkita.com:3107/Auth";

        private readonly IOAuthRepository _oauthRepository;
        private readonly IWindowManager _windowManager;

        private readonly OAuth2Client _oAuth2Client;

        public QuickBooksConnectViewModel(
            IOAuthRepository oauthRepository,
            IWindowManager windowManager)
        {
            _oauthRepository = oauthRepository;
            _windowManager = windowManager;
            ApplyCommand = new RelayCommand(OnApply);

            _oAuth2Client = new OAuth2Client(
                _oauthRepository.Params.ClientId,
                _oauthRepository.Params.ClientSecret,
                RedirectUrl,
                _oauthRepository.Params.Region);
            OpenBrowserCommand = new RelayCommand(() =>
            {
                var scopes = new List<OidcScopes> {OidcScopes.Accounting};
                var authorizeUrl = _oAuth2Client.GetAuthorizationURL(scopes);
                System.Diagnostics.Process.Start(authorizeUrl);
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand ApplyCommand { get; }
        public RelayCommand OpenBrowserCommand { get; }
        public string AuthKey { get; set; }

        private void OnApply()
        {
            if (string.IsNullOrEmpty(AuthKey))
            {
                ShowError("Auth Key is empty...");
                return;
            }

            var match = Regex.Match(AuthKey, @"(?<code>[A-Za-z0-9]+)\.(?<realmId>\d+)");
            if (!match.Success)
            {
                ShowError("Auth Key is malformed...");
                return;
            }

            _oauthRepository.Profile.UpdateRealm(match.Groups["realmId"].Value);
            var code = match.Groups["code"].Value;

            var accessTokenTask = ExchangeRequestTokenForAccessToken(code);
            accessTokenTask.ContinueWith(
                x =>
                {
                    using (_oauthRepository.GetProfileForUpdate(out var profile))
                    {
                        profile.UpdateAccess(
                            x.Result.AccessToken,
                            x.Result.RefreshToken,
                            DateTime.Now.AddMonths(6));
                    }

                    _windowManager.Close(this);
                },
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(
                System.Windows.Application.Current.MainWindow,
                message,
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private async Task<TokenResponse> ExchangeRequestTokenForAccessToken(string code)
        {
            var token = await _oAuth2Client.GetBearerTokenAsync(code);
            return token;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}