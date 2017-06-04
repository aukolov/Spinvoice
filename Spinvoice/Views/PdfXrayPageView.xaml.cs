using System.Windows.Controls;
using System.Windows.Input;
using Spinvoice.ViewModels.Invoices;

namespace Spinvoice.Views
{
    public partial class PdfXrayPageView
    {
        public PdfXrayPageView()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var textBlock = sender as TextBlock;
            if (textBlock == null)
            {
                return;
            }

            var viewModel = DataContext as PdfXrayPageViewModel;
            viewModel?.OnTextClicked(textBlock.Text);
        }
    }
}
