using System;
using System.Linq;
using Spinvoice.Domain.Pdf;
using Spinvoice.Utils;

namespace Spinvoice.ViewModels.Invoices
{
    public class PdfXrayPageViewModel
    {
        public PdfXrayPageViewModel(PageModel pageModel)
        {
            Sentences = pageModel.Sentences.ToArray();
        }

        public event Action<string> TextClicked;

        public SentenceModel[] Sentences { get; }

        public void OnTextClicked(string text)
        {
            TextClicked.Raise(text);
        }
    }
}