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