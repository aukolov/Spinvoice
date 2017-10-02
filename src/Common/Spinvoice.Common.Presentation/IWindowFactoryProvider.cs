using System;
using System.Windows;

namespace Spinvoice.Common.Presentation
{
    public interface IWindowFactoryProvider
    {
        Func<Window> GetFactory<T>();
    }
}