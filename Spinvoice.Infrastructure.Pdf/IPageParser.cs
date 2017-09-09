using System.Collections.Generic;
using Spinvoice.Common.Domain.Pdf;

namespace Spinvoice.Infrastructure.Pdf
{
    public interface IPageParser
    {
        List<List<SentenceModel>> Parse(string pdfFilePath, int pageNumber);
    }
}