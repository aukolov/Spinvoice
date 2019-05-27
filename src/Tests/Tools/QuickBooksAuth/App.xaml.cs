using System;
using System.Linq;
using System.Windows;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.ViewModels;
using Spinvoice.QuickBooks.Views;

namespace QuickBooksAuth
{
    public partial class App
    {
        private TestOAuthProfileDataAccess _dataAccess;

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            _dataAccess = new TestOAuthProfileDataAccess();
            var window = new QuickBooksConnectWindow
            {
                ShowInTaskbar = true,
                DataContext = new QuickBooksConnectViewModel(
                    new OAuthRepository(_dataAccess),
                    new TestWindowManager())
            };
            window.Show();
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            var profile = _dataAccess.GetAll().Single();
            Console.WriteLine(profile.AccessToken);
            Console.WriteLine(profile.RefreshToken);
            Console.WriteLine(profile.IdentityToken);
            Console.WriteLine(profile.RealmId);
        }
    }

}
