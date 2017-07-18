﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using NLog;

namespace Spinvoice.Preview
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
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
            MessageBox.Show(Application.Current.MainWindow,
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