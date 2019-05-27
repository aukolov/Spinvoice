﻿using System;
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

        private static OAuthProfile RestoreProfile()
        {
            var accessToken = Environment.GetEnvironmentVariable("QBU_ACCESS_TOKEN", EnvironmentVariableTarget.User);
            var refreshToken = Environment.GetEnvironmentVariable("QBU_REFRESH_TOKEN", EnvironmentVariableTarget.User);
            var identityToken = Environment.GetEnvironmentVariable("QBU_IDENTITY_TOKEN", EnvironmentVariableTarget.User);
            var realmId = Environment.GetEnvironmentVariable("QBU_REALM_ID", EnvironmentVariableTarget.User);
            var dataSource = Environment.GetEnvironmentVariable("QBU_DATA_SOURCE", EnvironmentVariableTarget.User);

            if (accessToken == null
                || refreshToken == null
                || identityToken == null
                || realmId == null
                || dataSource == null)
            {
                return null;
            }

            var authProfile = new OAuthProfile
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                IdentityToken = identityToken,
                RealmId = realmId,
                ExpirationDateTime = DateTime.Now.AddMonths(1)
            };
            if (!CheckProfile(authProfile))
            {
                return null;
            }
            return authProfile;
        }


        private static OAuthProfile AuthenticateWithApp()
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
            var identityToken = process.StandardOutput.ReadLine();
            var realmId = process.StandardOutput.ReadLine();

            var authProfile = new OAuthProfile
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                IdentityToken = identityToken,
                RealmId = realmId,
                ExpirationDateTime = DateTime.Now.AddMonths(1)
            };
            if (!CheckProfile(authProfile))
            {
                return null;
            }
            return authProfile;
        }

        private static bool CheckProfile(OAuthProfile authProfile)
        {
            return ExternalAuthService.TryConnect(out _, authProfile, AuthParams);
        }

        private static void StoreProfile(OAuthProfile authProfile)
        {
            Environment.SetEnvironmentVariable("QBU_ACCESS_TOKEN", authProfile.AccessToken, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("QBU_REFRESH_TOKEN", authProfile.RefreshToken, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("QBU_IDENTITY_TOKEN", authProfile.IdentityToken, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("QBU_REALM_ID", authProfile.RealmId, EnvironmentVariableTarget.User);
        }
    }
}