using System.ServiceModel;
using Spinvoice.Common.Domain.Pdf;

namespace Spinvoice.Server.Services
{
    [ServiceContract]
    public interface IFileParseService
    {
        [OperationContract]
        int Sum(int a, int b);

        PdfModel Parse(string filePath);
    }
}