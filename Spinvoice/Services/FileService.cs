using System.Collections.Generic;
using System.IO;

namespace Spinvoice.Services
{
    public class FileService : IFileService
    {
        public IEnumerable<string> GetSubDirectories(string directoryPath)
        {
            return Directory.GetDirectories(directoryPath);
        }

        public IEnumerable<string> GetFiles(string directoryPath)
        {
            return Directory.GetFiles(directoryPath);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}