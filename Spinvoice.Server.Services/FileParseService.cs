using Spinvoice.Common.Domain.Pdf;

namespace Spinvoice.Server.Services
{
    public class FileParseService : IFileParseService
    {
        private readonly IPdfParser _pdfParser;

        public FileParseService(IPdfParser pdfParser)
        {
            _pdfParser = pdfParser;
        }

        public int Sum(int a, int b)
        {
            return a + b;
        }

        public PdfModel Parse(string filePath)
        {
            return _pdfParser.Parse(filePath);
        }
    }
}
