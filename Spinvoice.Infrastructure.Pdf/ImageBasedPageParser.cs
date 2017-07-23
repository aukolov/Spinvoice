using System;
using System.Collections.Generic;
using System.Drawing;
using iTextSharp.text.pdf;
using Spinvoice.Domain.Pdf;
using Tesseract;

namespace Spinvoice.Infrastructure.Pdf
{
    internal class ImageBasedPageParser : IPageParser
    {
        private const PageIteratorLevel IteratorLevel = PageIteratorLevel.Word;

        private readonly BricksToSentensesTranslator _bricksToSentensesTranslator;
        private readonly PdfImageExtractor _pdfImageExtractor;

        public ImageBasedPageParser()
        {
            _bricksToSentensesTranslator = new BricksToSentensesTranslator();
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
            var resultIterator = page.GetIterator();
            while (resultIterator.Next(IteratorLevel))
            {
                Rect rect;
                if (resultIterator.TryGetBoundingBox(IteratorLevel, out rect))
                {
                    var text = resultIterator.GetText(IteratorLevel);
                    //var rectText = $"{rect.X1} x {rect.Y1}, {rect.Width} x {rect.Height}";
                    //Console.WriteLine($"'{text}' - {rectText}");
                    bricks.Add(new Brick(
                        text,
                        rect.X1 * scale,
                        rect.Y1 * scale,
                        rect.Width * scale,
                        rect.Height * scale));
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