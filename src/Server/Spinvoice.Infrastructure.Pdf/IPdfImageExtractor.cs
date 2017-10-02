using System.Drawing;
using iTextSharp.text.pdf;

namespace Spinvoice.Infrastructure.Pdf
{
    public interface IPdfImageExtractor
    {
        Image ExtractImagesFromPdf(PdfReader reader, int pageNumber);
    }
}