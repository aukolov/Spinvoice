﻿using System.Windows.Navigation;
using Spinvoice.QuickBooks.ViewModels;

namespace Spinvoice.QuickBooks.Views
{
    public partial class QuickBooksConnectView
    {
        public QuickBooksConnectView()
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
