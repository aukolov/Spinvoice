namespace Spinvoice.Infrastructure.Pdf
{
    internal class TesseractDataPathProvider : ITesseractDataPathProvider
    {
        public string Path => "tessdata";
    }
}