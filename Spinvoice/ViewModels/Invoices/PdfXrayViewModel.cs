using System;
using System.Linq;
using Spinvoice.Domain.Pdf;
using Spinvoice.Utils;

namespace Spinvoice.ViewModels.Invoices
{
    public class PdfXrayViewModel
    {
        public PdfXrayViewModel(PdfModel pdfModel)
        {
            Pages = pdfModel.Pages.Select(p =>
            {
                var viewModel = new PdfXrayPageViewModel(p);
                viewModel.TextClicked += OnTextClicked;
                return viewModel;
            }).ToArray();
        }
        public event Action<string> TextClicked;

        public PdfXrayPageViewModel[] Pages { get; }

        private void OnTextClicked(string text)
        {
            TextClicked.Raise(text);
        }
    }
}
