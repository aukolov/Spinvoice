using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Spinvoice.Common.Domain.Pdf
{
    [DataContract]
    public class BlockModel
    {
        public BlockModel(int blockNumber, List<SentenceModel> sentences)
        {
            BlockNumber = blockNumber;
            Sentences = sentences;
        }

        [DataMember]
        public int BlockNumber { get; private set; }
        [DataMember]
        public List<SentenceModel> Sentences { get; private set; }
    }
}