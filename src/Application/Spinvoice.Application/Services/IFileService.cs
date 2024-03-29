﻿using System.Collections.Generic;

namespace Spinvoice.Application.Services
{
    public interface IFileService
    {
        IEnumerable<string> GetSubDirectories(string directoryPath);
        IEnumerable<string> GetFiles(string directoryPath);
        bool FileExists(string filePath);
        bool DirectoryExists(string directoryPath);
        bool HasExtension(string filePath, string fileExtension);
    }
}