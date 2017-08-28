using Spinvoice.Application.ServerReference;
using Spinvoice.Common.Domain.Pdf;

namespace Spinvoice.Application.Services
{
    public class FileParseServiceProxy : IFileParseServiceProxy
    {
        public PdfModel Parse(string filePath)
        {
            var fileParseServiceClient = new FileParseServiceClient();
            var pdfModel = fileParseServiceClient.Parse(filePath);
            fileParseServiceClient.Close();
            return pdfModel;
        }
    }
}