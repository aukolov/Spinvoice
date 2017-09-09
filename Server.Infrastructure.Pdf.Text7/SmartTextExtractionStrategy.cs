using System;
using System.Collections.Generic;
using System.Linq;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Spinvoice.Common.Domain.Pdf;
using Spinvoice.Infrastructure.Pdf;

namespace Spinvoice.Server.Infrastructure.Pdf.Text7
{
    public class SmartTextExtractionStrategy : ITextExtractionStrategy
    {
        private readonly BricksToSentensesTranslator _bricksToSentensesTranslator;
        private readonly List<List<TextRenderInfo>> _blocks;
        private List<TextRenderInfo> _currentBlock;
        private double _maxY;

        public List<List<SentenceModel>> BlockSentences { get; private set; }

        public SmartTextExtractionStrategy()
        {
            _blocks = new List<List<TextRenderInfo>>();
            _bricksToSentensesTranslator = new BricksToSentensesTranslator(
                xDelta: 1,
                yDelta: 1,
                addSpaces: false);
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

        private IBrick InfoToBrick(TextRenderInfo info)
        {
            var baseRectange = info.GetBaseline().GetBoundingRectangle();
            var accentRectange = info.GetAscentLine().GetBoundingRectangle();

            var text = info.GetText();

            var brick = new Brick(text,
                baseRectange.GetX(),
                _maxY - baseRectange.GetY(),
                baseRectange.GetWidth(),
                accentRectange.GetY() - baseRectange.GetY());
            return brick;
        }

        public void EventOccurred(IEventData data, EventType type)
        {
            switch (type)
            {
                case EventType.BEGIN_TEXT:
                    _currentBlock = new List<TextRenderInfo>();
                    break;
                case EventType.RENDER_TEXT:
                    var textRenderInfo = (TextRenderInfo) data;
                    textRenderInfo.PreserveGraphicsState();
                    _currentBlock.Add(textRenderInfo);
                    var rectange = textRenderInfo.GetAscentLine().GetBoundingRectangle();
                    _maxY = Math.Max(_maxY, rectange.GetY());

                    break;
                case EventType.END_TEXT:
                    _blocks.Add(_currentBlock);
                    _currentBlock = null;
                    break;
            }
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return new List<EventType>
            {
                EventType.BEGIN_TEXT,
                EventType.END_TEXT,
                EventType.RENDER_TEXT
            };
        }

    }
}