using System;
using System.Linq;
using Spinvoice.Common.Domain.Pdf;
using Spinvoice.Domain.Pdf;
using Spinvoice.Utils;

namespace Spinvoice.Application.ViewModels.Invoices
{
    public class PdfXrayPageViewModel
    {
        public PdfXrayPageViewModel(PageModel pageModel)
        {
            Sentences = pageModel.Sentences.ToArray();
        }

        public event Action<SentenceModel> TextClicked;

        public SentenceModel[] Sentences { get; }

        public void OnTextClicked(SentenceModel sentence)
        {
            TextClicked.Raise(sentence);
        }
    }
}