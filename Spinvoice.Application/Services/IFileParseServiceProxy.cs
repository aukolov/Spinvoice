using System;
using Spinvoice.Common.Domain.Pdf;

namespace Spinvoice.Application.Services
{
    public interface IFileParseServiceProxy : IDisposable
    {
        PdfModel Parse(string filePath);
    }
}