using System;
using System.Collections.Generic;
using System.Windows;
using Spinvoice.Application.ViewModels.Exchange;
using Spinvoice.Application.ViewModels.QuickBooks;
using Spinvoice.Application.Views.Exchange;
using Spinvoice.Application.Views.QuickBooks;
using Spinvoice.Domain.UI;
using Spinvoice.QuickBooks.ViewModels;

namespace Spinvoice.Application.Services
{
    public class WindowManager : IWindowManager
    {
        private readonly Dictionary<Type, Func<Window>> _windowFactories = new Dictionary<Type, Func<Window>>
            {
                {typeof(IExchangeRatesViewModel),() => new ExchangeRatesWindow()},
                {typeof(IQuickBooksConnectViewModel),() => new QuickBooksConnectWindow()},
                {typeof(IAccountsChartViewModel),() => new AccountsChartWindow()}
            };
        private readonly Dictionary<object, Window> _viewModelWindows = new Dictionary<object, Window>();

        public void ShowWindow<T>(T viewModel)
        {
            Window window;
            if (!_viewModelWindows.TryGetValue(viewModel, out window))
            {
                window = CreateWindow(viewModel);
                window.Show();
            }
            else
            {
                window.BringIntoView();
            }
        }

        public bool? ShowDialog<T>(T viewModel)
        {
            if (_viewModelWindows.ContainsKey(viewModel))
            {
                throw new InvalidOperationException("Dialog is already shown.");
            }
            var window = CreateWindow(viewModel);
            window.ShowDialog();
            return window.DialogResult;
        }

        private Window CreateWindow<T>(T viewModel)
        {
            var viewFactory = _windowFactories[typeof(T)];
            var window = viewFactory();
            window.DataContext = viewModel;
            window.Owner = System.Windows.Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _viewModelWindows.Add(viewModel, window);
            window.Closed += OnClosed;
            return window;
        }

        private void OnClosed(object sender, EventArgs e)
        {
            var window = (Window)sender;
            window.Closed -= OnClosed;
            _viewModelWindows.Remove(window.DataContext);
        }

        public void Close(object viewModel)
        {
            Window window;
            if (!_viewModelWindows.TryGetValue(viewModel, out window)) return;

            window.Close();
        }

        public void CloseDialog(object viewModel, bool? dialogResult)
        {
            Window window;
            if (!_viewModelWindows.TryGetValue(viewModel, out window)) return;

            window.DialogResult = dialogResult;
            window.Close();
        }
    }
}