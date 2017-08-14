using System;
using System.ComponentModel;
using Spinvoice.Application.Services;

namespace Spinvoice.Application.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = Bootstrapper.Init();
            Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            var disposable = DataContext as IDisposable;
            disposable?.Dispose();
        }
    }
}