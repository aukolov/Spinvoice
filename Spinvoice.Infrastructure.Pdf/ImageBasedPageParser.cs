using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using iTextSharp.text.pdf;
using Spinvoice.Domain.Pdf;
using Tesseract;

namespace Spinvoice.Infrastructure.Pdf
{
    internal class ImageBasedPageParser : IPageParser
    {
        private readonly BricksToSentensesTranslator _bricksToSentensesTranslator;
        private readonly PdfImageExtractor _pdfImageExtractor;

        public ImageBasedPageParser()
        {
            _bricksToSentensesTranslator = new BricksToSentensesTranslator(xDelta: 7, yDelta: 3, addSpaces: true);
            _pdfImageExtractor = new PdfImageExtractor();
        }

        public List<List<SentenceModel>> Parse(PdfReader pdfReader, int pageNumber)
        {
            var image = _pdfImageExtractor.ExtractImagesFromPdf(pdfReader, pageNumber);
            var engine = new TesseractEngine(@"tessdata", "eng");
            if (image == null)
            {
                return new List<List<SentenceModel>>();
            }

            var scale = 1d;
            if (image.Height != 0 && image.Width != 0)
            {
                var widthScale = PdfPageSize.Width / image.Width;
                var heightScale = PdfPageSize.Height / image.Height;

                scale = Math.Min(widthScale, heightScale);
            }

            Page page;
            try
            {
                page = engine.Process(new Bitmap(image));
            }
            catch (ArgumentException)
            {
                return new List<List<SentenceModel>>();
            }

            var bricks = new List<IBrick>();
            var wordIterator = page.GetIterator();
            var lineIterator = page.GetIterator();
            while (lineIterator.Next(PageIteratorLevel.TextLine))
            {
                var line = lineIterator.GetText(PageIteratorLevel.TextLine);
                Rect baseLine;
                var hasBaseLine = lineIterator.TryGetBaseline(PageIteratorLevel.TextLine, out baseLine);
                var lineWords = line.Split(' ')
                    .Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                //Console.WriteLine($"Line {lineWords.Length}: '{line}'");

                for (var i = 0; i < lineWords.Length; i++)
                {
                    while (wordIterator.Next(PageIteratorLevel.Word) &&
                           string.IsNullOrWhiteSpace(wordIterator.GetText(PageIteratorLevel.Word)))
                    {

                    }

                    Rect rect;
                    if (wordIterator.TryGetBoundingBox(PageIteratorLevel.Word, out rect))
                    {
                        var word = wordIterator.GetText(PageIteratorLevel.Word);
                        //var rectText = $"{rect.X1} x {rect.Y1}, {rect.Width} x {rect.Height}";
                        //Console.WriteLine($"Word #{i}: '{word}' - {rectText}");

                        bricks.Add(new Brick(
                            word,
                            rect.X1 * scale,
                            (hasBaseLine ? baseLine.Y2 : rect.Y2) * scale,
                            rect.Width * scale,
                            rect.Height * scale));
                    }
                }
            }

            var sentenceModels = _bricksToSentensesTranslator.Translate(
                new List<IBrick[]>
                {
                    bricks.ToArray()
                });
            return sentenceModels;
        }
    }
}