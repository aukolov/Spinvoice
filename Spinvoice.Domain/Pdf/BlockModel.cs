using System.Collections.Generic;

namespace Spinvoice.Domain.Pdf
{
    public class BlockModel
    {
        public BlockModel(List<string> sentences)
        {
            Sentences = sentences.AsReadOnly();
        }

        public IReadOnlyList<string> Sentences { get; private set; }
    }
}