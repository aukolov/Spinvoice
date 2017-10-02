using System.Globalization;
using System.Threading;
using Autofac;

namespace QuickBooksTool
{
    public static class ToolBootstrapper
    {
        public static IContainer Init()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<Spinvoice.QuickBooks.AutofacModule>();
            containerBuilder.RegisterModule<Spinvoice.Common.Presentation.AutofacModule>();
            containerBuilder.RegisterModule<AutofacModule>();
            var container = containerBuilder.Build();

            return container;
        }
    }
}
