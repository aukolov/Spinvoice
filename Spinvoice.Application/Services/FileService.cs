using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Spinvoice.Application.Services
{
    public class FileService : IFileService
    {
        private readonly FileNamesComparer _fileNamesComparer;

        public FileService()
        {
            _fileNamesComparer = new FileNamesComparer();
        }

        public IEnumerable<string> GetSubDirectories(string directoryPath)
        {
            return Directory.GetDirectories(directoryPath).OrderBy(s => s, _fileNamesComparer);
        }

        public IEnumerable<string> GetFiles(string directoryPath)
        {
            return Directory.GetFiles(directoryPath).OrderBy(s => s, _fileNamesComparer);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public bool DirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        public bool HasExtension(string filePath, string fileExtension)
        {
            return filePath != null && Path.GetExtension(filePath).ToLower() == fileExtension;
        }

    }
}