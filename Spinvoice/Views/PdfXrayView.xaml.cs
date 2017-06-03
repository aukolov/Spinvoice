using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Spinvoice.Views
{
    public partial class PdfXrayView
    {
        public PdfXrayView()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var textBlock = (TextBlock)sender;
            Clipboard.SetText(textBlock.Text);
        }
    }
}
