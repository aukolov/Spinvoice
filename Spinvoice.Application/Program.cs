using System;
using System.Threading;
using System.Windows;

namespace Spinvoice.Application
{
    public static class Program
    {
        public static void Run()
        {
            using (var mutex = new Mutex(false, @"Global\" + "DA64BE31 - 3564 - 4A84 - 89A3 - F664CCCF47B1"))
            {
                if (!mutex.WaitOne(0, false))
                {
                    MessageBox.Show(
                        @"Spinvoice application is already running, only one instance of application is allowed to run at the same time.

Use already running application or close it and try again.",
                        "Spinvoice is already running...",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                var app = new App
                {
                    StartupUri = new Uri("pack://application:,,,/Spinvoice.Application;component/Views/MainWindow.xaml")
                };
                app.InitializeComponent();
                app.Run();
            }
        }
    }
}