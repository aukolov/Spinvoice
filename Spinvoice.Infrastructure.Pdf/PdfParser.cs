using System.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Spinvoice.Domain.Pdf;

namespace Spinvoice.Infrastructure.Pdf
{
    public class PdfParser : IPdfParser
    {
        public PdfModel Parse(string filePath)
        {
            var reader = new PdfReader(filePath);
            var pdfModel = new PdfModel(Enumerable.Range(1, reader.NumberOfPages).Select(i =>
            {
                var strategy = new SmartTextExtractionStrategy();
                PdfTextExtractor.GetTextFromPage(reader, i, strategy);
                var blockModels = strategy.BlockSentences.Select(blocks => new BlockModel(blocks)).ToList();
                return new PageModel(blockModels);
            }).ToList());

            return pdfModel;
        }
    }
}
