using System.Windows.Controls;
using System.Windows.Input;
using Spinvoice.Application.ViewModels.Invoices;
using Spinvoice.Common.Domain.Pdf;
using Spinvoice.Domain.Pdf;

namespace Spinvoice.Application.Views
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

            var sentence = textBlock.DataContext as SentenceModel;
            if (sentence == null)
            {
                return;
            }

            var viewModel = DataContext as PdfXrayPageViewModel;
            viewModel?.OnTextClicked(sentence);
        }
    }
}
