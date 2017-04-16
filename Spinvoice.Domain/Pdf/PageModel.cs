using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Spinvoice.Domain.Pdf
{
    public class PageModel
    {
        public PageModel(List<List<string>> blocks) : this(new List<BlockModel>(blocks.Select(sentences => new BlockModel(sentences))))
        {
        }

        public PageModel(List<BlockModel> blocks)
        {
            Blocks = blocks.AsReadOnly();
        }

        public IReadOnlyList<BlockModel> Blocks { get; private set; }
    }
}