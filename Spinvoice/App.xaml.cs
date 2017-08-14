using System;
using System.Windows;
using System.Windows.Threading;
using NLog;

namespace Spinvoice
{
    public partial class App
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            _logger.Error((Exception)unhandledExceptionEventArgs.ExceptionObject,
                "Unhandled exception. {0}", unhandledExceptionEventArgs.IsTerminating ? "The application will be terminated." : "");
            MessageBox.Show(Current.MainWindow,
                $"Something went wrong...\r\n{unhandledExceptionEventArgs.ExceptionObject}");
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.Error(e.Exception, "Unhandled exception.");
            MessageBox.Show(Current.MainWindow,
                $"Something went wrong in dispatcher...\r\n{e.Exception}");
            e.Handled = true;
        }
    }
}
