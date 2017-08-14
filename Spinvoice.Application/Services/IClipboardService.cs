using System;

namespace Spinvoice.Application.Services
{
    public interface IClipboardService : IDisposable
    {
        event Action ClipboardChanged;
        string GetText();
        bool CheckContainsText();
        bool TrySetText(string text);
    }
}