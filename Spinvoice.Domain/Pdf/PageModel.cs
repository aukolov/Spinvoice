using System;
using System.Collections.Generic;
using System.Linq;

namespace Spinvoice.Domain.Pdf
{
    public class PageModel
    {
        public PageModel(int pageNumber, List<BlockModel> blocks)
        {
            PageNumber = pageNumber;
            Blocks = blocks.AsReadOnly();
            Sentences = blocks
                .SelectMany(bm => bm.Sentences)
                //.OrderBy(model => model.Bottom)
                //.ThenBy(model => model.Left)
                .ToArray();
        }

        public int PageNumber { get; }

        public IReadOnlyList<BlockModel> Blocks { get; }

        public IReadOnlyList<SentenceModel> Sentences { get; }

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
                    if (j >= Blocks[currentBlock].Sentences[currentSentence].Text.Length)
                    {
                        return false;
                    }
                    if (ch != Blocks[currentBlock].Sentences[currentSentence].Text[j])
                    {
                        return false;
                    }
                    i++;
                    j++;
                }
            }
            j = SkipSpaces(Blocks[currentBlock].Sentences[currentSentence].Text, j);
            if (j < Blocks[currentBlock].Sentences[currentSentence].Text.Length)
            {
                return false;
            }
            endBlock = currentBlock;
            endSentence = currentSentence;
            return true;
        }

        public SentenceModel Above(SentenceModel sentence)
        {
            var i = GetIndex(sentence);
            if (i < 0)
            {
                return null;
            }

            i -= 1;
            var midX = sentence.Left + sentence.Width / 2;
            while (i >= 0)
            {
                var candidate = Sentences[i];
                if (candidate.Top < sentence.Top)
                {
                    if (candidate.Left <= midX && midX <= candidate.Right)
                    {
                        return candidate;
                    }
                }
                i -= 1;
            }
            return null;
        }

        public IEnumerable<SentenceModel> Below(SentenceModel sentence)
        {
            var i = GetIndex(sentence);
            if (i < 0)
            {
                yield break;
            }

            i += 1;
            var midX = sentence.Left + sentence.Width / 2;
            while (i < Sentences.Count)
            {
                var candidate = Sentences[i];
                if (candidate.Top > sentence.Top)
                {
                    if (candidate.Left >= midX && midX <= sentence.Right)
                    {
                        yield return candidate;
                    }
                }
                i += 1;
            }
        }

        public IEnumerable<SentenceModel> Left(SentenceModel sentence)
        {
            var i = GetIndex(sentence);
            if (i < 0)
            {
                yield break;
            }

            i -= 1;
            while (i > 0)
            {
                var candidate = Sentences[i];
                if (Math.Abs(candidate.Top - sentence.Top) < 5)
                {
                    if (candidate.Left < sentence.Left)
                    {
                        yield return candidate;
                    }
                }
                i -= 1;
            }
        }

        private int GetIndex(SentenceModel sentence)
        {
            var index = sentence.PageIndex;
            if (index < 0) return -1;
            if (index > Sentences.Count) return -1;
            return Sentences[index] == sentence ? index : -1;
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
                    var sentence = Blocks[currentBlock].Sentences[currentSentence];

                    while (index < sentence.Text.Length)
                    {
                        if (!IsWhiteSpace(sentence.Text[index]))
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