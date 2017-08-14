using System;
using System.Linq;
using Spinvoice.Domain.Pdf;
using Spinvoice.Utils;

namespace Spinvoice.Application.ViewModels.Invoices
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
        public event Action<SentenceModel> TextClicked;

        public PdfXrayPageViewModel[] Pages { get; }

        private void OnTextClicked(SentenceModel sentence)
        {
            TextClicked.Raise(sentence);
        }
    }
}
