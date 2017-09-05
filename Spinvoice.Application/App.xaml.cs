using System;
using System.Windows;
using System.Windows.Threading;
using NLog;

namespace Spinvoice.Application
{
    public partial class App
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
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
