using System;
using System.Collections.Generic;
using System.Windows;
using Spinvoice.ViewModels.Exchange;
using Spinvoice.Views.Exchange;

namespace Spinvoice.Services
{
    public class WindowManager
    {
        private readonly Dictionary<Type, Func<Window>> _windowFactories = new Dictionary<Type, Func<Window>>
            {
                {typeof(ExchangeRatesViewModel),() => new ExchangeRatesWindow()}
            };
        private readonly Dictionary<object, Window> _viewModelWindows = new Dictionary<object, Window>();

        public WindowManager()
        {
        }

        public void ShowWindow<T>(T viewModel)
        {
            Window window;
            if (!_viewModelWindows.TryGetValue(viewModel, out window))
            {
                var viewFactory = _windowFactories[typeof(T)];
                window = viewFactory();
                window.DataContext = viewModel;
                window.Owner = Application.Current.MainWindow;
                window.Show();
                window.Closed += OnClosed;
                _viewModelWindows.Add(viewModel, window);
            }
            else
            {
                window.BringIntoView();
            }
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
            if (_viewModelWindows.TryGetValue(viewModel, out window))
            {
                window.Close();
            }
        }
    }
}