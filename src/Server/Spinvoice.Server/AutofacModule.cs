using Autofac;

namespace Spinvoice.Server
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApplicationNameProvider>().AsImplementedInterfaces().SingleInstance();
        }
    }
}