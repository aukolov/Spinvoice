using System;
using System.Windows;
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
            _logger.Error((Exception)unhandledExceptionEventArgs.ExceptionObject, "Unhandled exception.");
            MessageBox.Show(Application.Current.MainWindow,
                $"Something went wrong...\r\n{unhandledExceptionEventArgs.ExceptionObject}");
        }
    }
}
