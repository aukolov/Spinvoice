using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Spinvoice.Domain.Pdf;

namespace Spinvoice.Infrastructure.Pdf
{
    internal class TextBasedPageParser : IPageParser
    {
        public List<List<SentenceModel>> Parse(PdfReader pdfReader, int pageNumber)
        {
            var strategy = new SmartTextExtractionStrategy();
            PdfTextExtractor.GetTextFromPage(pdfReader, pageNumber, strategy);
            return strategy.BlockSentences;
        }
    }
}