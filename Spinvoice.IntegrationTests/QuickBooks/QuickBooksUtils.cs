using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Spinvoice.Domain.ExternalBook;

namespace Spinvoice.IntegrationTests.QuickBooks
{
    public class QuickBooksUtils
    {
        private static OAuthProfile _profile;

        public static IOAuthProfile GetOAuthProfile()
        {
            return _profile ?? (_profile = Authenticate());
        }

        private static OAuthProfile Authenticate()
        {
            var path = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"..\..\..\",
                @"Tests\Tools\QuickBooksAuth\bin\Debug\QuickBooksAuth.exe");
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
            var accessSecret = process.StandardOutput.ReadLine();
            var realmId = process.StandardOutput.ReadLine();
            var dataSource = process.StandardOutput.ReadLine();

            return new OAuthProfile
            {
                AccessSecret = accessSecret,
                AccessToken = accessToken,
                DataSource = dataSource,
                RealmId = realmId,
                ExpirationDateTime = DateTime.Now.AddMonths(1)
            };
        }
    }
}