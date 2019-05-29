using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.IntegrationTests.QuickBooks
{
    public class QuickBooksUtils
    {
        private static readonly ExternalAuthService ExternalAuthService;
        private static readonly OAuthParams AuthParams;

        static QuickBooksUtils()
        {
            ExternalAuthService = new ExternalAuthService();
            AuthParams = new OAuthParams();
        }

        public static IOAuthProfile GetOAuthProfile()
        {
            var profile = RestoreProfile();
            if (profile != null)
            {
                return profile;
            }
            profile = AuthenticateWithApp();
            if (profile != null)
            {
                StoreProfile(profile);
                return profile;
            }
            return null;
        }

        private static AuthProfile RestoreProfile()
        {
            var accessToken = Environment.GetEnvironmentVariable("QBU_ACCESS_TOKEN", EnvironmentVariableTarget.User);
            var refreshToken = Environment.GetEnvironmentVariable("QBU_REFRESH_TOKEN", EnvironmentVariableTarget.User);
            var realmId = Environment.GetEnvironmentVariable("QBU_REALM_ID", EnvironmentVariableTarget.User);

            if (accessToken == null
                || refreshToken == null
                || realmId == null)
            {
                return null;
            }

            var authProfile = new AuthProfile
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RealmId = realmId,
                ExpirationDateTime = DateTime.Now.AddMonths(1)
            };
            if (!CheckProfile(authProfile))
            {
                return null;
            }
            return authProfile;
        }


        private static AuthProfile AuthenticateWithApp()
        {
            var path = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"..\..\..\",
                @"Tools\QuickBooksAuth\bin\Debug\QuickBooksAuth.exe");
            var process = new Process
            {
                StartInfo =
                {
                    FileName = path,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
            process.Start();
            var accessToken = process.StandardOutput.ReadLine();
            var refreshToken = process.StandardOutput.ReadLine();
            var realmId = process.StandardOutput.ReadLine();

            var authProfile = new AuthProfile
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RealmId = realmId,
                ExpirationDateTime = DateTime.Now.AddMonths(1)
            };
            if (!CheckProfile(authProfile))
            {
                return null;
            }
            return authProfile;
        }

        private static bool CheckProfile(AuthProfile authProfile)
        {
            return ExternalAuthService.TryConnect(out _, authProfile, AuthParams);
        }

        private static void StoreProfile(AuthProfile authProfile)
        {
            Environment.SetEnvironmentVariable("QBU_ACCESS_TOKEN", authProfile.AccessToken, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("QBU_REFRESH_TOKEN", authProfile.RefreshToken, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("QBU_REALM_ID", authProfile.RealmId, EnvironmentVariableTarget.User);
        }
    }
}