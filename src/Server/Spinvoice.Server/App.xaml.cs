using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Threading;
using Autofac;
using Autofac.Integration.Wcf;
using NLog;
using Spinvoice.Server.Services;

namespace Spinvoice.Server
{
    public partial class App
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ServiceHost _serviceHost;
        private readonly IContainer _container;

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            try
            {
                _container = ServerBootstrapper.Init();

                _serviceHost = new ServiceHost(typeof(FileParseService));

                _serviceHost.AddDependencyInjectionBehavior<IFileParseService>(_container);

                _serviceHost.Open();
            }
            catch (Exception e)
            {
                Logger.Error(e, "Exception while starting the server." + e);
                _serviceHost?.Abort();
                Current.Shutdown(-1);
            }

            Logger.Info("Started.");
        }

        private void OnExit(object sender, ExitEventArgs exitEventArgs)
        {
            Logger.Info("Shutting down.");
            _serviceHost?.Close();
            ((IDisposable)_serviceHost)?.Dispose();
            _container?.Dispose();
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Logger.Error((Exception)unhandledExceptionEventArgs.ExceptionObject,
                "Unhandled exception. {0}", unhandledExceptionEventArgs.IsTerminating ? "The application will be terminated." : "");
            MessageBox.Show(Current.MainWindow,
                $"Something went wrong...\r\n{unhandledExceptionEventArgs.ExceptionObject}");
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Error(e.Exception, "Unhandled exception.");
            MessageBox.Show(Current.MainWindow,
                $"Something went wrong in dispatcher...\r\n{e.Exception}");
            e.Handled = true;
        }
    }
}
