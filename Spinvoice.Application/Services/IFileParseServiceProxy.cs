using Spinvoice.Common.Domain.Pdf;

namespace Spinvoice.Application.Services
{
    public interface IFileParseServiceProxy
    {
        PdfModel Parse(string filePath);
    }
}