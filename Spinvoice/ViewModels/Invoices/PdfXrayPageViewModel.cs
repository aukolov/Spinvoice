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
            Pairs = Sentences.Select(model => new Tuple<SentenceModel, SentenceModel>(model, pageModel.Above(model)))
                .Where(tuple => tuple.Item2 != null)
                .ToArray();
        }

        public Tuple<SentenceModel, SentenceModel>[] Pairs { get; }

        public event Action<SentenceModel> TextClicked;

        public SentenceModel[] Sentences { get; }

        public void OnTextClicked(SentenceModel sentence)
        {
            TextClicked.Raise(sentence);
        }
    }
}