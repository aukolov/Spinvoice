using System.ServiceModel;
using Spinvoice.Common.Domain.Pdf;

namespace Spinvoice.Server.Services
{
    [ServiceContract]
    public interface IFileParseService
    {
        [OperationContract]
        int Sum(int a, int b);

        [OperationContract]
        PdfModel Parse(string filePath);
    }

    public class FileParseServiceMock : IFileParseService
    {
        public int Sum(int a, int b)
        {
            throw new System.NotImplementedException();
        }

        public PdfModel Parse(string filePath)
        {
            throw new System.NotImplementedException();
        }
    }
}