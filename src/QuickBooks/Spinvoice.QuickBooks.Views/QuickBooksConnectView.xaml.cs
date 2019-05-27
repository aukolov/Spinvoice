using System.Windows.Navigation;
using Spinvoice.QuickBooks.ViewModels;

namespace Spinvoice.QuickBooks.Views
{
    public partial class QuickBooksConnectView
    {
        public QuickBooksConnectView()
        {
            InitializeComponent();
        }

        private void WebBrowser_OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (!(DataContext is QuickBooksConnectViewModel viewModel)) return;

            viewModel.OnNavigating(e.Uri, out var cancel);
            e.Cancel = cancel;
        }
    }
}
