using System;
using System.ComponentModel;
using Autofac;
using Spinvoice.Application.Services;
using Spinvoice.Application.ViewModels;
using IContainer = Autofac.IContainer;

namespace Spinvoice.Application.Views
{
    public partial class MainWindow
    {
        private IContainer _container;

        public MainWindow()
        {
            InitializeComponent();

            _container = Bootstrapper.Init();
            DataContext = _container.Resolve<IAppViewModel>();
            Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            var disposable = DataContext as IDisposable;
            disposable?.Dispose();

            _container.Dispose();
        }
    }
}