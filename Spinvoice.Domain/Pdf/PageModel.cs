using System.Collections.Generic;

namespace Spinvoice.Domain.Pdf
{
    public class PageModel
    {
        public PageModel(int pageNumber, List<BlockModel> blocks)
        {
            PageNumber = pageNumber;
            Blocks = blocks.AsReadOnly();
        }

        public int PageNumber { get; }

        public IReadOnlyList<BlockModel> Blocks { get; private set; }

        public IEnumerable<LocationRange> Find(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                yield break;
            }
            for (var startSentence = 0; startSentence < Blocks.Count; startSentence++)
            {
                for (var startBlock = 0; startBlock < Blocks[startSentence].Sentences.Count; startBlock++)
                {
                    int endBlock;
                    int endSentence;
                    if (IsMatch(startSentence, startBlock, text, out endBlock, out endSentence))
                    {
                        yield return new LocationRange(
                            new Location(PageNumber, startSentence, startBlock),
                            new Location(PageNumber, endBlock, endSentence));
                    }
                }
            }
        }

        private bool IsMatch(int startBlock, int startSentence, string text, out int endBlock, out int endSentence)
        {
            endBlock = 0;
            endSentence = 0;

            var i = 0;
            var currentBlock = startBlock;
            var currentSentence = startSentence;
            var j = 0;
            j = SkipSpaces(Blocks[currentBlock].Sentences[currentSentence], j);
            while (i < text.Length)
            {
                var ch = text[i];
                if (ch == ' ')
                {
                    //if (j == Blocks[currentBlock].Sentences[currentSentence].Length)
                    currentSentence++;
                    j = 0;
                    i++;
                }
                else
                {
                    if (ch != Blocks[currentBlock].Sentences[currentSentence][j])
                    {
                        return false;
                    }
                    i++;
                    j++;
                }
            }
            j = SkipSpaces(Blocks[currentBlock].Sentences[currentSentence], j);
            if (j < Blocks[currentBlock].Sentences[currentSentence].Length)
            {
                return false;
            }
            endBlock = currentBlock;
            endSentence = currentSentence;
            return true;
        }

        private static int SkipSpaces(string text, int startIndex)
        {
            var i = startIndex;
            while (i < text.Length && text[i] == ' ')
            {
                i++;
            }
            return i;
        }
    }
}