namespace Spinvoice.Domain.Pdf
{
    public interface IPdfParser
    {
        PdfModel Parse(string filePath);
    }
}