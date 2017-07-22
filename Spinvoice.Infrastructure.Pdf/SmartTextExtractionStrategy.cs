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
        private double _maxY;

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
                .Select(b => b.Select(info => new TextRenderInfoBrick(info, ReplaceZeros, _maxY)).ToArray())
                .ToArray();

            var blockSentences = SentenceBuilder.BuildSentences(brickLists);
            BlockSentences = blockSentences;

            return "";
        }

        private bool ReplaceZeros => _slashZeroCount < SlashZeroThreshold;
    }
}