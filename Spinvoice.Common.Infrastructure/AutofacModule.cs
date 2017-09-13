using Autofac;
using Spinvoice.Common.Infrastructure.Logging;

namespace Spinvoice.Common.Infrastructure
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LogConfigurator>().AsImplementedInterfaces().AsSelf().SingleInstance();
        }
    }
}