using System;

namespace Spinvoice
{
    public static class EntryPoint
    {
        [STAThread]
        private static void Main()
        {
            var app = new Application.App
            {
                StartupUri = new Uri("pack://application:,,,/Spinvoice.Application;component/Views/MainWindow.xaml")
            };
            app.InitializeComponent();
            app.Run();
        }
    }
}