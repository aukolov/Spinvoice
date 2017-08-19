using System;
using Spinvoice.Common.Domain.Pdf;

namespace Spinvoice.Application.Services
{
    public interface IFileParseServiceProxy : IDisposable
    {
        int Sum(int a, int b);
        PdfModel Parse(string filePath);
    }
}