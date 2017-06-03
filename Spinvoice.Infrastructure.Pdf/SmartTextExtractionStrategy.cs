using System;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text.pdf.parser;
using Spinvoice.Domain.Pdf;

namespace Spinvoice.Infrastructure.Pdf
{
    public class SmartTextExtractionStrategy : ITextExtractionStrategy
    {
        private const int SlashZeroThreshold = 100;

        private List<TextRenderInfo> _currentBlock;
        private readonly List<List<TextRenderInfo>> _blocks;
        private int _slashZeroCount;

        public List<List<SentenceModel>> BlockSentences { get; private set; }

        public SmartTextExtractionStrategy()
        {
            _blocks = new List<List<TextRenderInfo>>();
        }

        public void BeginTextBlock()
        {
            _currentBlock = new List<TextRenderInfo>();
        }

        public void RenderText(TextRenderInfo renderInfo)
        {
            CountSlashZero(renderInfo);
            _currentBlock.Add(renderInfo);
            //var r = renderInfo.GetBaseline().GetBoundingRectange();
            //Console.WriteLine($"Test: {renderInfo.PdfString} " +
            //             $"{r.X}-{r.Y} " +
            //             $"{r.Width}-{r.Height} ");
        }

        private void CountSlashZero(TextRenderInfo renderInfo)
        {
            if (_slashZeroCount < SlashZeroThreshold)
            {
                var s = renderInfo.PdfString.ToString();
                _slashZeroCount += s.Count(c => c == '\0');
            }
        }

        public void EndTextBlock()
        {
            _blocks.Add(_currentBlock);
            _currentBlock = null;
        }

        public void RenderImage(ImageRenderInfo renderInfo)
        {
            // Ignore
        }

        public string GetResultantText()
        {
            BlockSentences = new List<List<SentenceModel>>();

            foreach (var block in _blocks)
            {
                var sentences = new List<SentenceModel>();
                string accumulatedText = null;
                for (var i = 0; i < block.Count; i++)
                {
                    var brick = block[i];
                    if (accumulatedText == null)
                    {
                        accumulatedText = brick.PdfString.ToString();
                    }
                    else
                    {
                        var prevBrick = block[i - 1];
                        var currentRect = brick.GetBaseline().GetBoundingRectange();
                        var prevRect = prevBrick.GetBaseline().GetBoundingRectange();
                        if (RoughEqual(currentRect.Y, prevRect.Y)
                            && RoughEqual(prevRect.X + prevRect.Width, currentRect.X))
                        {
                            accumulatedText += brick.PdfString.ToString();
                        }
                        else
                        {
                            sentences.Add(new SentenceModel(Decode(accumulatedText)));
                            accumulatedText = brick.PdfString.ToString();
                        }
                    }
                    if (i == block.Count - 1)
                    {
                        sentences.Add(new SentenceModel(Decode(accumulatedText)));
                    }
                }
                BlockSentences.Add(sentences);
            }

            return "";
        }

        private string Decode(string text)
        {
            if (text == null) return null;
            return _slashZeroCount < SlashZeroThreshold ? text : text.Replace("\0", "");
        }

        private static bool RoughEqual(float currentRectY, float prevRectX)
        {
            return Math.Abs(currentRectY - prevRectX) < 1;
        }
    }
}