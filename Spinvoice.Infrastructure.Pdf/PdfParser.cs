using System;
using System.Drawing;
using System.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Spinvoice.Domain.Pdf;
using Tesseract;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace Spinvoice.Infrastructure.Pdf
{
    public class PdfParser : IPdfParser
    {
        private PdfImageExtractor _pdfImageExtractor;

        public PdfParser()
        {
            _pdfImageExtractor = new PdfImageExtractor();
        }

        public PdfModel Parse(string filePath)
        {
            var reader = new PdfReader(filePath);
            var fileName = System.IO.Path.GetFileName(filePath);
            var pdfModel = new PdfModel(
                fileName,
                Enumerable.Range(1, reader.NumberOfPages).Select(i =>
                {
                    var strategy = new SmartTextExtractionStrategy();
                    PdfTextExtractor.GetTextFromPage(reader, i, strategy);
                    var blockModels = strategy.BlockSentences
                        .Select((sentences, j) => new BlockModel(j, sentences))
                        .ToList();

                    //if (!blockModels.Any())
                    //{
                    //    var image = PdfImageExtractor.ExtractImagesFromPdf(reader, i);
                    //    var engine = new TesseractEngine(@"c:\1\tessdata", "eng");
                    //    if (image != null)
                    //    {
                    //        var page = engine.Process(new Bitmap(image));
                    //        var resultIterator = page.GetIterator();
                    //        var pageIteratorLevel = PageIteratorLevel.Word;
                    //        while (resultIterator.Next(pageIteratorLevel))
                    //        {
                    //            Rect bounds;
                    //            var boundsText = "?";
                    //            if (resultIterator.TryGetBoundingBox(pageIteratorLevel, out bounds))
                    //            {
                    //                boundsText = $"{bounds.X1} x {bounds.Y1}, {bounds.Width} x {bounds.Height}";
                    //            }
                    //            Console.WriteLine($"'{resultIterator.GetText(pageIteratorLevel)}' - {boundsText}");
                    //        }
                    //        //var analyseLayout = page.AnalyseLayout();

                    //        image.Save($@"c:\1\images{i}.bmp", ImageFormat.Bmp);
                    //    }
                    //}

                    return new PageModel(i - 1, blockModels);
                }).ToList());

            return pdfModel;
        }

        public bool IsPdf(string filePath)
        {
            return filePath != null && System.IO.Path.GetExtension(filePath).ToLower() == ".pdf";
        }
    }
}