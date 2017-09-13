using Autofac;

namespace Spinvoice.Server.Services
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileParseService>().AsImplementedInterfaces().SingleInstance();
        }
    }
}