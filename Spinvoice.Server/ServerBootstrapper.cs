using Autofac;
using Spinvoice.Common.Infrastructure.Logging;
// ReSharper disable RedundantNameQualifier

namespace Spinvoice.Server
{
    public static class ServerBootstrapper
    {
        public static IContainer Init()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<Spinvoice.Common.Infrastructure.AutofacModule>();
            containerBuilder.RegisterModule<Spinvoice.Infrastructure.Pdf.AutofacModule>();
            containerBuilder.RegisterModule<Spinvoice.Server.Infrastructure.Pdf.Text7.AutofacModule>();
            containerBuilder.RegisterModule<Spinvoice.Server.Services.AutofacModule>();
            containerBuilder.RegisterModule<AutofacModule>();

            var container = containerBuilder.Build();
            container.Resolve<ILogConfigurator>().Configure();
            return container;
        }
    }
}