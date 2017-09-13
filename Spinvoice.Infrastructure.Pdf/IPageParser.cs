using System.Collections.Generic;
using iTextSharp.text.pdf;
using Spinvoice.Common.Domain.Pdf;
using Spinvoice.Domain.Pdf;

namespace Spinvoice.Infrastructure.Pdf
{
    internal interface IPageParser
    {
        List<List<SentenceModel>> Parse(PdfReader pdfReader, int pageNumber);
    }
}