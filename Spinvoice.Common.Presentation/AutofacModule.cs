using Autofac;

namespace Spinvoice.Common.Presentation
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WindowManager>().AsImplementedInterfaces().AsSelf().SingleInstance();
        }
    }
}