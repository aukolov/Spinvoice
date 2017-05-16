using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Spinvoice.QuickBooks.ViewModels;

namespace Spinvoice.Views.QuickBooks
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
