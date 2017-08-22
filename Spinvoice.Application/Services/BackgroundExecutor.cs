using System;
using System.Windows.Threading;

namespace Spinvoice.Application.Services
{
    public class BackgroundExecutor
    {
        public static void Execute(Action action)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(action));
        }
    }
}