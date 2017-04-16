using System;
using System.Collections.Generic;
using iTextSharp.text.pdf.parser;

namespace Spinvoice.Infrastructure.Pdf
{
    public class SmartTextExtractionStrategy : ITextExtractionStrategy
    {
        private List<TextRenderInfo> _currentBlock;
        private readonly List<List<TextRenderInfo>> _blocks;

        public List<List<string>> BlockSentences { get; private set; }

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
            _currentBlock.Add(renderInfo);
            //var r = renderInfo.GetBaseline().GetBoundingRectange();
            //Console.WriteLine($"{renderInfo.PdfString} " +
            //                  $"{renderInfo.GetFont().Encoding} " +
            //                  $"{renderInfo.GetFont().FullFontName} ");
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
            BlockSentences = new List<List<string>>();

            foreach (var block in _blocks)
            {
                var sentences = new List<string>();
                string accumulatedText = null;
                for (int i = 0; i < block.Count; i++)
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
                            sentences.Add(accumulatedText);
                            accumulatedText = brick.PdfString.ToString();
                        }
                        if (i == block.Count - 1)
                        {
                            sentences.Add(accumulatedText);
                        }
                    }
                }
                BlockSentences.Add(sentences);
            }

            return "";
        }

        private static bool RoughEqual(float currentRectY, float prevRectX)
        {
            return Math.Abs(currentRectY - prevRectX) < 0.1;
        }
    }
}