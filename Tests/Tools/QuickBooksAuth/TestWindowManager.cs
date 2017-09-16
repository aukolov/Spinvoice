using System;
using System.Windows;
using Spinvoice.Domain.UI;

namespace QuickBooksAuth
{
    internal class TestWindowManager : IWindowManager
    {
        public void ShowWindow<T>(T viewModel)
        {
            throw new NotImplementedException();
        }

        public void Close(object viewModel)
        {
            Application.Current.MainWindow.Close();
        }

        public void CloseDialog(object viewModel, bool? dialogResult)
        {
            throw new NotImplementedException();
        }

        public bool? ShowDialog<T>(T viewModel)
        {
            throw new NotImplementedException();
        }
    }
}