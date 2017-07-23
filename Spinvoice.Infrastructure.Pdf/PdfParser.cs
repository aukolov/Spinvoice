using System.Collections.Generic;
using System.Linq;
using iTextSharp.text.pdf;
using Spinvoice.Domain.Pdf;

namespace Spinvoice.Infrastructure.Pdf
{
    public class PdfParser : IPdfParser
    {
        private readonly IPageParser[] _parsers;

        public PdfParser()
        {
            _parsers = new IPageParser[]
            {
                new TextBasedPageParser(),
                new ImageBasedPageParser()
            };
        }

        public PdfModel Parse(string filePath)
        {
            var reader = new PdfReader(filePath);
            var fileName = System.IO.Path.GetFileName(filePath);
            var pdfModel = new PdfModel(
                fileName,
                Enumerable.Range(1, reader.NumberOfPages).Select(i =>
                {
                    List<List<SentenceModel>> sentenceModels = null;
                    foreach (var parser in _parsers)
                    {
                        var list = parser.Parse(reader, i);
                        if (list.Any())
                        {
                            sentenceModels = list;
                            break;
                        }
                    }
                    var blockModels = (sentenceModels ?? new List<List<SentenceModel>>())
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