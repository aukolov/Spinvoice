using Spinvoice.Common.Domain.Pdf;

namespace Spinvoice.Infrastructure.Pdf
{
    public interface IPdfParser
    {
        PdfModel Parse(string filePath);
    }
}