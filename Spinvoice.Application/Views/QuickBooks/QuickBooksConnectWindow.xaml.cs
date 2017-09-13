using System.Windows;
using System.Windows.Navigation;
using Spinvoice.QuickBooks.ViewModels;

namespace Spinvoice.Application.Views.QuickBooks
{
    /// <summary>
    /// Interaction logic for QuickBooksConnectWindow.xaml
    /// </summary>
    public partial class QuickBooksConnectWindow : Window
    {
        public QuickBooksConnectWindow()
        {
            InitializeComponent();
        }

        private void WebBrowser_OnNavigated(object sender, NavigationEventArgs e)
        {
            var viewModel = DataContext as QuickBooksConnectViewModel;
            viewModel?.OnNavigated(e.Uri);
        }
    }
}
