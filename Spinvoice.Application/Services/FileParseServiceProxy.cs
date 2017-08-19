using Spinvoice.Application.ServerReference;
using Spinvoice.Common.Domain.Pdf;

namespace Spinvoice.Application.Services
{
    public class FileParseServiceProxy : IFileParseServiceProxy
    {
        private readonly FileParseServiceClient _fileParseServiceClient;

        public FileParseServiceProxy()
        {
            _fileParseServiceClient = new FileParseServiceClient();
        }

        public int Sum(int a, int b)
        {
            return _fileParseServiceClient.Sum(a, b);
        }

        public PdfModel Parse(string filePath)
        {
            return _fileParseServiceClient.Parse(filePath);
        }

        public void Dispose()
        {
            _fileParseServiceClient.Close();
        }
    }
}