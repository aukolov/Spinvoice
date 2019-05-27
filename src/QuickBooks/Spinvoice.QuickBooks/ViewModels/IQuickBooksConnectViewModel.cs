using System;
using System.ComponentModel;

namespace Spinvoice.QuickBooks.ViewModels
{
    public interface IQuickBooksConnectViewModel : INotifyPropertyChanged
    {
        string Url { get; set; }
        void OnNavigating(Uri uri, out bool cancel);
    }
}