namespace Spinvoice.Infrastructure.DataAccess
{
    public class DataDirectoryProvider : IDataDirectoryProvider
    {
        public DataDirectoryProvider(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}