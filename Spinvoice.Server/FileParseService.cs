using System.ServiceModel;

namespace Spinvoice.Server
{
    [ServiceContract]
    public interface IFileParseService
    {
        [OperationContract]
        int Sum(int a, int b);
    }

    public class FileParseService : IFileParseService
    {
        public FileParseService()
        {
            
        }

        public int Sum(int a, int b)
        {
            return a + b;
        }
    }
}
