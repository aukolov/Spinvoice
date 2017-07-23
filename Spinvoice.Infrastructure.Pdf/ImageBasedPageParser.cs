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

            var page = engine.Process(new Bitmap(image));
            var resultIterator = page.GetIterator();

            var bricks = new List<IBrick>();
            while (resultIterator.Next(IteratorLevel))
            {
                Rect rect;
                if (resultIterator.TryGetBoundingBox(IteratorLevel, out rect))
                {
                    var rectText = $"{rect.X1} x {rect.Y1}, {rect.Width} x {rect.Height}";
                    var text = resultIterator.GetText(IteratorLevel);
                    Console.WriteLine($"'{text}' - {rectText}");
                    bricks.Add(new Brick(
                        text,
                        rect.X1,
                        rect.Y1,
                        rect.Width,
                        rect.Height));
                }
            }

            var sentenceModels = _bricksToSentensesTranslator.Translate(
                new List<IBrick[]>
                {
                    bricks.Take(25).ToArray()
                });
            return sentenceModels;
        }
    }
}