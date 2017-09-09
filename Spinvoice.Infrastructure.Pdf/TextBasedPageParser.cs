using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Spinvoice.Common.Domain.Pdf;

namespace Spinvoice.Infrastructure.Pdf
{
    internal class TextBasedPageParser : ITextBasedPageParser
    {
        public List<List<SentenceModel>> Parse(string pdfFilePath, int pageNumber)
        {
            using (var reader = new PdfReader(pdfFilePath))
            {
                var strategy = new SmartTextExtractionStrategy();
                PdfTextExtractor.GetTextFromPage(reader, pageNumber, strategy);
                return strategy.BlockSentences;
            }
        }
    }
}