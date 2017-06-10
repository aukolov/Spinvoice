using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Spinvoice.Domain.Pdf
{
    public class PdfModel
    {
        public PdfModel(List<PageModel> pages)
        {
            Pages = pages.AsReadOnly();
        }

        public ReadOnlyCollection<PageModel> Pages { get; }

        public IEnumerable<BlockModel> BlockModels
        {
            get { return Pages.SelectMany(model => model.Blocks); }
        }

        public IEnumerable<SentenceModel> Sentences
        {
            get { return BlockModels.SelectMany(model => model.Sentences); }
        }

        public string GetText()
        {
            var sb = new StringBuilder();

            for (var i = 0; i < Pages.Count; i++)
            {
                sb.AppendLine($"Page {i}");
                var blocks = Pages[i].Blocks;
                for (var j = 0; j < blocks.Count; j++)
                {
                    sb.AppendLine($"\tBlock {j}");
                    foreach (var sentence in blocks[j].Sentences)
                    {
                        sb.AppendLine($"\t\t{sentence}");
                    }
                }
            }

            return sb.ToString();
        }

        public IEnumerable<LocationRange> Find(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                yield break;
            }

            foreach (var pageModel in Pages)
            {
                foreach (var locationRange in pageModel.Find(text))
                {
                    yield return locationRange;
                }
            }
        }

        public IEnumerable<SentenceModel> FindSentence(string text)
        {
            foreach (var sentence in Sentences)
            {
                if (sentence.Text == text)
                    yield return sentence;
            }
        }
    }
}