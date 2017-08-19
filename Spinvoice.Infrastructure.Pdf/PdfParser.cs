using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text.pdf;
using Spinvoice.Common.Domain.Pdf;
using Spinvoice.Domain.Pdf;
// ReSharper disable SuggestBaseTypeForParameter

namespace Spinvoice.Infrastructure.Pdf
{
    internal class PdfParser : IPdfParser
    {
        private readonly IPageParser[] _parsers;

        public PdfParser(
            ITextBasedPageParser textBasedPageParser,
            IImageBasedPageParser imageBasedPageParser)
        {
            _parsers = new IPageParser[]
            {
                textBasedPageParser,
                imageBasedPageParser
            };
        }

        public PdfModel Parse(string filePath)
        {
            int numberOfPages;
            using (var reader = new PdfReader(filePath))
            {
                numberOfPages = reader.NumberOfPages;
            }

            var pageModels = ParallelEnumerable.Range(1, numberOfPages)
                .Select(i => ParsePage(filePath, i))
                .ToList();

            var pdfModel = new PdfModel(Path.GetFileName(filePath), pageModels);

            return pdfModel;
        }

        private PageModel ParsePage(string filePath, int i)
        {
            using (var reader = new PdfReader(filePath))
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
            }
        }
    }
}