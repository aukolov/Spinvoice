using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Spinvoice.Common.Domain.Pdf;
using Spinvoice.Infrastructure.Pdf;

namespace Spinvoice.Server.Infrastructure.Pdf.Text7
{
    public class Text7BasedPageParser : IText7BasedPageParser
    {
        public List<List<SentenceModel>> Parse(string filePath, int pageNumber)
        {
            using (var pdfReader = new PdfReader(filePath))
            {
                using (var pdfDocument = new PdfDocument(pdfReader))
                {
                    var pdfPage = pdfDocument.GetPage(pageNumber);
                    var strategy = new SmartTextExtractionStrategy();
                    PdfTextExtractor.GetTextFromPage(pdfPage, strategy);

                    return strategy.BlockSentences;
                }
            }
        }
    }
}