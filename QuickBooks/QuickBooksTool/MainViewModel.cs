using System;
using System.Windows;
using System.Windows.Input;
using Spinvoice.Common.Presentation;
using Spinvoice.QuickBooks.ViewModels;
using Spinvoice.QuickBooks.Views;
using Spinvoice.Utils;

namespace QuickBooksTool
{
    public class MainViewModel : IMainViewModel
    {
        private readonly IWindowManager _windowManager;
        private readonly Func<IQuickBooksConnectViewModel> _connectViewModelFactory;

        public MainViewModel(
            IWindowManager windowManager,
            Func<IQuickBooksConnectViewModel> connectViewModelFactory)
        {
            _windowManager = windowManager;
            _connectViewModelFactory = connectViewModelFactory;
            ConnectCommand = new RelayCommand(ShowConnectDialog);
        }

        private void ShowConnectDialog()
        {
            var connectViewModel = _connectViewModelFactory();
            _windowManager.ShowDialog(connectViewModel);
        }

        public ICommand ConnectCommand { get; }
    }
}