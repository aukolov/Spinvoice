using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using NLog;
using Spinvoice.Utils;

namespace Spinvoice.Services
{
    public class ClipboardService : IDisposable
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        const int WmDrawclipboard = 0x308;
        const int WmChangecbchain = 0x030D;

        private readonly IntPtr _currentWindowHandle;
        private IntPtr _nextClipboardViewerHandle;

        public event Action ClipboardChanged;
        private bool _disposed;

        public ClipboardService()
        {
            var windowInteropHelper = new WindowInteropHelper(Application.Current.MainWindow);
            _currentWindowHandle = windowInteropHelper.Handle;
            var hwndSource = HwndSource.FromHwnd(_currentWindowHandle);
            if (hwndSource == null) throw new ApplicationException("");
            hwndSource.AddHook(Hook);

            _nextClipboardViewerHandle = (IntPtr)SetClipboardViewer((int)_currentWindowHandle);
        }

        ~ClipboardService()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        public string GetText()
        {
            return ExecuteClipboardOperation(Clipboard.GetText);
        }

        public bool CheckContainsText()
        {
            return ExecuteClipboardOperation(Clipboard.ContainsText);
        }

        public bool TrySetText(string text)
        {
            return ExecuteClipboardOperation(() =>
            {
                Clipboard.SetText(text);
                return true;
            });
        }

        private T ExecuteClipboardOperation<T>(Func<T> func)
        {
            for (var i = 0; i < 5; i++)
            {
                try
                {
                    return func();
                }
                catch (COMException e)
                {
                    if (e.ErrorCode == -2147221040)
                    {
                        _logger.Error(e, "Failed to read text from clipboard.");
                        Thread.Sleep(100);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return default(T);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                }
                ChangeClipboardChain(_currentWindowHandle, _nextClipboardViewerHandle);
                _disposed = true;
            }
        }

        private IntPtr Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WmChangecbchain)
            {
                if (_nextClipboardViewerHandle == wParam)
                {
                    _nextClipboardViewerHandle = lParam;
                }
                else if (_nextClipboardViewerHandle != IntPtr.Zero)
                {
                    SendMessage(_nextClipboardViewerHandle, msg, wParam, lParam);
                }
            }
            else if (msg == WmDrawclipboard)
            {
                if (_nextClipboardViewerHandle != IntPtr.Zero)
                {
                    SendMessage(_nextClipboardViewerHandle, msg, wParam, lParam);
                }
                ClipboardChanged.Raise();
            }

            return IntPtr.Zero;
        }

        [DllImport("User32.dll")]
        private static extern int SetClipboardViewer(int hWmdNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(
            IntPtr hWnd,
            int wMsg,
            IntPtr wParam,
            IntPtr lParam);


    }
}
