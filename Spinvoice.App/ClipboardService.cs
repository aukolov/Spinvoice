using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Spinvoice.App
{
    class ClipboardService : IDisposable
    {
        const int WmDrawclipboard = 0x308;
        const int WmChangecbchain = 0x030D;

        private readonly IntPtr _currentWindowHandle;
        private IntPtr _nextClipboardViewerHandle;

        public Action ClipboardChanged;
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
            switch (msg)
            {
                case WmChangecbchain:
                    if (_nextClipboardViewerHandle == wParam)
                    {
                        _nextClipboardViewerHandle = lParam;
                    }
                    else if (_nextClipboardViewerHandle != IntPtr.Zero)
                    {
                        SendMessage(_nextClipboardViewerHandle, msg, wParam, lParam);
                    }
                    break;
                case WmDrawclipboard:
                    if (_nextClipboardViewerHandle != IntPtr.Zero)
                    {
                        SendMessage(_nextClipboardViewerHandle, msg, wParam, lParam);
                    }
                    ClipboardChanged.Raise();
                    break;
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
