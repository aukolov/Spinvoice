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

        private readonly BricksToSentensesTranslator _bricksToSentensesTranslator;
        private readonly List<List<TextRenderInfo>> _blocks;
        private List<TextRenderInfo> _currentBlock;
        private int _slashZeroCount;
        private double _maxY;

        public List<List<SentenceModel>> BlockSentences { get; private set; }

        public SmartTextExtractionStrategy()
        {
            _blocks = new List<List<TextRenderInfo>>();
            _bricksToSentensesTranslator = new BricksToSentensesTranslator();
        }

        public void BeginTextBlock()
        {
            _currentBlock = new List<TextRenderInfo>();
        }

        public void RenderText(TextRenderInfo renderInfo)
        {
            CountSlashZero(renderInfo);
            _currentBlock.Add(renderInfo);
            var rectange = renderInfo.GetAscentLine().GetBoundingRectange();
            _maxY = Math.Max(_maxY, rectange.Y);
        }

        private void CountSlashZero(TextRenderInfo renderInfo)
        {
            if (_slashZeroCount >= SlashZeroThreshold) return;

            var s = renderInfo.PdfString.ToString();
            _slashZeroCount += s.Count(c => c == '\0');
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
            var brickLists = _blocks
                .Select(b => b.Select(InfoToBrick).ToArray())
                .ToArray();

            var blockSentences = _bricksToSentensesTranslator.Translate(brickLists);
            BlockSentences = blockSentences;

            return "";
        }

        private bool ReplaceZeros => _slashZeroCount < SlashZeroThreshold;

        private IBrick InfoToBrick(TextRenderInfo info)
        {
            var baseRectange = info.GetBaseline().GetBoundingRectange();
            var accentRectange = info.GetAscentLine().GetBoundingRectange();

            var text = info.PdfString.ToString();
            if (ReplaceZeros)
            {
                text = text.Replace("\0", "");
            }

            var brick = new Brick(text,
                baseRectange.X,
                _maxY - baseRectange.Y,
                baseRectange.Width,
                accentRectange.Y - baseRectange.Y);
            return brick;
        }
    }
}