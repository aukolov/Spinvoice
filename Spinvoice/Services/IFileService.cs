using System.Collections.Generic;

namespace Spinvoice.Services
{
    public interface IFileService
    {
        IEnumerable<string> GetSubDirectories(string directoryPath);
        IEnumerable<string> GetFiles(string directoryPath);
        bool FileExists(string filePath);
    }
}