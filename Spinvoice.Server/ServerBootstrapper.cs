using Autofac;
using Spinvoice.Common.Infrastructure.Logging;

namespace Spinvoice.Server
{
    public static class ServerBootstrapper
    {
        public static IContainer Init()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<Common.Infrastructure.AutofacModule>();
            containerBuilder.RegisterModule<Infrastructure.Pdf.AutofacModule>();
            containerBuilder.RegisterModule<Server.Services.AutofacModule>();
            containerBuilder.RegisterModule<AutofacModule>();

            var container = containerBuilder.Build();
            container.Resolve<ILogConfigurator>().Configure();
            return container;
        }
    }
}