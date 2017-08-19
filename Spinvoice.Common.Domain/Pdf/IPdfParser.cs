namespace Spinvoice.Common.Domain.Pdf
{
    public interface IPdfParser
    {
        PdfModel Parse(string filePath);
    }
}