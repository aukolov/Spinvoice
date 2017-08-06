using System;
using System.Collections.Generic;
using System.Drawing;
using iTextSharp.text.pdf;
using NLog;
using Spinvoice.Domain.Pdf;
using Tesseract;

namespace Spinvoice.Infrastructure.Pdf
{
    internal class ImageBasedPageParser : IImageBasedPageParser
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ITesseractDataPathProvider _tesseractDataPathProvider;
        private readonly BricksToSentensesTranslator _bricksToSentensesTranslator;
        private readonly PdfImageExtractor _pdfImageExtractor;

        public ImageBasedPageParser(
            ITesseractDataPathProvider tesseractDataPathProvider)
        {
            _tesseractDataPathProvider = tesseractDataPathProvider;
            _bricksToSentensesTranslator = new BricksToSentensesTranslator(
                xDelta: 7, 
                yDelta: 3, 
                addSpaces: true);
            _pdfImageExtractor = new PdfImageExtractor();
        }

        public List<List<SentenceModel>> Parse(PdfReader pdfReader, int pageNumber)
        {
            var image = _pdfImageExtractor.ExtractImagesFromPdf(pdfReader, pageNumber);
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

            var engine = new TesseractEngine(_tesseractDataPathProvider.Path, "eng");
            Page page;
            try
            {
                page = engine.Process(new Bitmap(image));
            }
            catch (ArgumentException ex)
            {
                Logger.Error(ex, "Error while processing image.");
                return new List<List<SentenceModel>>();
            }

            var bricks = ExtractBricks(page, scale);
            var sentenceModels = _bricksToSentensesTranslator.Translate(
                new List<IBrick[]>
                {
                    bricks.ToArray()
                });
            return sentenceModels;
        }

        private static List<IBrick> ExtractBricks(Page page, double scale)
        {
            var bricks = new List<IBrick>();
            var wordIterator = page.GetIterator();
            wordIterator.Begin();
            var lineIterator = page.GetIterator();
            lineIterator.Begin();

            Rect? baseLine = null;
            do
            {
                while (string.IsNullOrWhiteSpace(wordIterator.GetText(PageIteratorLevel.Word))
                       && wordIterator.Next(PageIteratorLevel.Word))
                {
                }

                if (wordIterator.IsAtBeginningOf(PageIteratorLevel.TextLine))
                {
                    baseLine = null;

                    while (string.IsNullOrWhiteSpace(lineIterator.GetText(PageIteratorLevel.TextLine))
                           && lineIterator.Next(PageIteratorLevel.TextLine))
                    {
                    }

                    Rect baseLineCandidate;
                    if (lineIterator.TryGetBaseline(PageIteratorLevel.TextLine, out baseLineCandidate))
                    {
                        baseLine = baseLineCandidate;
                    }
                    //Console.WriteLine("------------------");
                    //Console.WriteLine($"Line: {lineIterator.GetText(PageIteratorLevel.TextLine)}");
                    lineIterator.Next(PageIteratorLevel.TextLine);
                }

                var brick = TryExtractBrick(scale, wordIterator, baseLine);
                if (brick != null)
                {
                    bricks.Add(brick);
                }

            } while (wordIterator.Next(PageIteratorLevel.Word));
            return bricks;
        }

        private static Brick TryExtractBrick(double scale, ResultIterator wordIterator, Rect? baseLine)
        {
            var word = wordIterator.GetText(PageIteratorLevel.Word);
            if (string.IsNullOrWhiteSpace(word))
            {
                return null;
            }

            Rect rect;
            Brick brick = null;
            if (wordIterator.TryGetBoundingBox(PageIteratorLevel.Word, out rect))
            {
                //var rectText = $"{rect.X1} x {rect.Y1}, {rect.Width} x {rect.Height}";
                //Console.WriteLine($"Word #{i}: '{word}' - {rectText}");
                //Console.Write($"{word} ");

                brick = new Brick(
                    word,
                    rect.X1 * scale,
                    (baseLine?.Y2 ?? rect.Y2) * scale,
                    rect.Width * scale,
                    rect.Height * scale);
            }
            return brick;
        }
    }
}