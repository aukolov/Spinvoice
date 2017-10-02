using System;
using System.Diagnostics;

namespace QuickBooksAuth
{
    public static class EntryPoint
    {
        [STAThread]
        private static void Main()
        {
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}