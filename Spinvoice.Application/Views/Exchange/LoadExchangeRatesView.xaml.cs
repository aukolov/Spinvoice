using System.Diagnostics;
using System.Windows.Navigation;

namespace Spinvoice.Application.Views.Exchange
{
    /// <summary>
    /// Interaction logic for LoadExchangeRatesView.xaml
    /// </summary>
    public partial class LoadExchangeRatesView
    {
        public LoadExchangeRatesView()
        {
            InitializeComponent();
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}
