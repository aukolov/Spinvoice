using System;
using System.Collections.Generic;
using System.Windows;
using Spinvoice.Application.ViewModels.Exchange;
using Spinvoice.Application.ViewModels.QuickBooks;
using Spinvoice.Application.Views.Exchange;
using Spinvoice.Application.Views.QuickBooks;
using Spinvoice.Common.Presentation;
using Spinvoice.QuickBooks.ViewModels;
using Spinvoice.QuickBooks.Views;

namespace Spinvoice.Application.Services
{
    internal class WindowFactoryProvider : IWindowFactoryProvider
    {
        private readonly Dictionary<Type, Func<Window>> _windowFactories = new Dictionary<Type, Func<Window>>
        {
            {typeof(IExchangeRatesViewModel),() => new ExchangeRatesWindow()},
            {typeof(IQuickBooksConnectViewModel),() => new QuickBooksConnectWindow()},
            {typeof(IAccountsChartViewModel),() => new AccountsChartWindow()}
        };

        public Func<Window> GetFactory<T>()
        {
            return _windowFactories[typeof(T)];
        }
    }
}