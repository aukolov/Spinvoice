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

        public ReadOnlyCollection<PageModel> Pages { get; private set; }

        public IEnumerable<BlockModel> BlockModels
        {
            get { return Pages.SelectMany(model => model.Blocks); }
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
    }
}