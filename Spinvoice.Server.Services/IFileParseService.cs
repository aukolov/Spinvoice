using System.ServiceModel;
using Spinvoice.Common.Domain.Pdf;

namespace Spinvoice.Server.Services
{
    [ServiceContract]
    public interface IFileParseService
    {
        [OperationContract]
        PdfModel Parse(string filePath);
    }

    // ReSharper disable once UnusedMember.Global
    public class FileParseServiceMock : IFileParseService
    {
        public PdfModel Parse(string filePath)
        {
            throw new System.NotImplementedException();
        }
    }
}