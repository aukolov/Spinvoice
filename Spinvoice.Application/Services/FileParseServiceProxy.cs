using Spinvoice.Application.ServerReference;

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
    }
}