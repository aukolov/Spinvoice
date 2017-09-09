using Autofac;

namespace Spinvoice.Server.Infrastructure.Pdf.Text7
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Text7BasedPageParser>().AsImplementedInterfaces().InstancePerDependency()
                .ExternallyOwned();
        }
    }
}