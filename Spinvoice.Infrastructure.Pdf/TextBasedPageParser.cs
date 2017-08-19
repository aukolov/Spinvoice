using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Spinvoice.Common.Domain.Pdf;
using Spinvoice.Domain.Pdf;

namespace Spinvoice.Infrastructure.Pdf
{
    internal class TextBasedPageParser : ITextBasedPageParser
    {
        public List<List<SentenceModel>> Parse(PdfReader pdfReader, int pageNumber)
        {
            var strategy = new SmartTextExtractionStrategy();
            PdfTextExtractor.GetTextFromPage(pdfReader, pageNumber, strategy);
            return strategy.BlockSentences;
        }
    }
}