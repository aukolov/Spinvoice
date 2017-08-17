using System.ServiceModel;

namespace Spinvoice.Server.Services
{
    [ServiceContract]
    public interface IFileParseService
    {
        [OperationContract]
        int Sum(int a, int b);
    }
}