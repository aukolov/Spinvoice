using System.Windows;
using System.Windows.Threading;

namespace QuickBooksTool
{
    public partial class App
    {
        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Something went wrong... " + e);
            e.Handled = true;
        }
    }
}
