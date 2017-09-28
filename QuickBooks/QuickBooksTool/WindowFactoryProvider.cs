using System;
using System.Collections.Generic;
using System.Windows;
using Spinvoice.Common.Presentation;
using Spinvoice.QuickBooks.ViewModels;
using Spinvoice.QuickBooks.Views;

namespace QuickBooksTool
{
    public class WindowFactoryProvider : IWindowFactoryProvider
    {
        private readonly Dictionary<Type, Func<Window>> _windowFactories = new Dictionary<Type, Func<Window>>
        {
            {typeof(IQuickBooksConnectViewModel),() => new QuickBooksConnectWindow()},
        };

        public Func<Window> GetFactory<T>()
        {
            return _windowFactories[typeof(T)];
        }
    }
}