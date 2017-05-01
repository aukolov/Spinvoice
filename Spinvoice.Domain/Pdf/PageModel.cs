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
            var startBlock = 0;
            while (startBlock < Blocks.Count)
            {
                var startSentence = 0;
                while (startSentence < Blocks[startBlock].Sentences.Count)
                {
                    int endBlock;
                    int endSentence;
                    if (IsMatch(startBlock, startSentence, text, out endBlock, out endSentence))
                    {
                        yield return new LocationRange(
                            new Location(PageNumber, startBlock, startSentence),
                            new Location(PageNumber, endBlock, endSentence));
                        startBlock = endBlock;
                        startSentence = endSentence + 1;
                    }
                    else
                    {
                        startSentence++;
                    }
                }
                startBlock++;
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

            SkipSpaces(ref currentBlock, ref currentSentence, ref j);
            while (i < text.Length)
            {
                var ch = text[i];
                if (IsWhiteSpace(ch))
                {
                    i = SkipSpaces(text, i);
                    if (i < text.Length)
                    {
                        SkipSpaces(ref currentBlock, ref currentSentence, ref j);
                    }
                }
                else
                {
                    if (j >= Blocks[currentBlock].Sentences[currentSentence].Length)
                    {
                        return false;
                    }
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

        private static bool IsWhiteSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        private static int SkipSpaces(string text, int startIndex)
        {
            var i = startIndex;
            while (i < text.Length && IsWhiteSpace(text[i]))
            {
                i++;
            }
            return i;
        }

        private void SkipSpaces(ref int currentBlock, ref int currentSentence, ref int index)
        {
            while (currentBlock < Blocks.Count)
            {
                while (currentSentence < Blocks[currentBlock].Sentences.Count)
                {
                    var text = Blocks[currentBlock].Sentences[currentSentence];

                    while (index < text.Length)
                    {
                        if (!IsWhiteSpace(text[index]))
                        {
                            return;
                        }
                        index++;
                    }
                    index = 0;
                    currentSentence++;
                }
                currentBlock++;
                currentSentence = 0;
                index = 0;
            }
        }
    }
}