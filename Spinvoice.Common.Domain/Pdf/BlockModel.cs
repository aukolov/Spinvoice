using System.Collections.Generic;

namespace Spinvoice.Common.Domain.Pdf
{
    public class BlockModel
    {
        public BlockModel(int blockNumber, List<SentenceModel> sentences)
        {
            BlockNumber = blockNumber;
            Sentences = sentences.AsReadOnly();
        }

        public int BlockNumber { get; }

        public IReadOnlyList<SentenceModel> Sentences { get; }
    }
}