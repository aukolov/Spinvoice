using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using Spinvoice.App.Annotations;

namespace Spinvoice.App
{
    public sealed class AppViewModel : INotifyPropertyChanged, IDisposable
    {
        private ClipboardService _clipboardService;
        private string _clipboardText;

        public AppViewModel()
        {
            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                _clipboardService = new ClipboardService();
                OnClipboardChanged();
                _clipboardService.ClipboardChanged += OnClipboardChanged;
            }, DispatcherPriority.Loaded);
        }


        public string ClipboardText
        {
            get { return _clipboardText; }
            set
            {
                if (_clipboardText == value) return;
                _clipboardText = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnClipboardChanged()
        {
            ClipboardText = Clipboard.ContainsText()
                ? Clipboard.GetText()
                : null;
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            _clipboardService?.Dispose();
        }
    }
}