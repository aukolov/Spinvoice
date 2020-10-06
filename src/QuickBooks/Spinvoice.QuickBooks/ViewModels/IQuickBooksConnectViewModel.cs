using System;
using System.ComponentModel;
using Spinvoice.Utils;

namespace Spinvoice.QuickBooks.ViewModels
{
    public interface IQuickBooksConnectViewModel : INotifyPropertyChanged
    {
        RelayCommand ApplyCommand { get; }
        RelayCommand OpenBrowserCommand { get; }
        string AuthKey { get; set; }
    }
}