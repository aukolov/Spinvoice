using System;
using System.ComponentModel;

namespace Spinvoice.QuickBooks.ViewModels
{
    public interface IQuickBooksConnectViewModel : INotifyPropertyChanged
    {
        string Url { get; set; }
        void OnNavigated(Uri uri);
    }
}