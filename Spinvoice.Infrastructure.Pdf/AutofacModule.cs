using Autofac;

namespace Spinvoice.Infrastructure.Pdf
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BricksToSentensesTranslator>().AsImplementedInterfaces().AsSelf().InstancePerDependency();
            builder.RegisterType<PdfImageExtractor>().AsImplementedInterfaces().AsSelf().InstancePerDependency();
            builder.RegisterType<ImageBasedPageParser>().AsImplementedInterfaces().AsSelf().InstancePerDependency();
            builder.RegisterType<TextBasedPageParser>().AsImplementedInterfaces().AsSelf().InstancePerDependency();
            builder.RegisterType<PdfParser>().AsImplementedInterfaces().AsSelf().InstancePerDependency();
        }
    }
}