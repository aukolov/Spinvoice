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