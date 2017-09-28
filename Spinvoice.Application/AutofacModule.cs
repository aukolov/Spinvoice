using Autofac;
using Spinvoice.Application.Services;

namespace Spinvoice.Application
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("ViewModel"))
                .AsImplementedInterfaces().AsSelf()
                .InstancePerDependency()
                .ExternallyOwned();

            builder.RegisterType<ClipboardService>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<ExchangeRatesLoader>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<FileService>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<WindowFactoryProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<TaskSchedulerProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ServerManager>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ApplicationNameProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<FileParseServiceProxy>().AsImplementedInterfaces().SingleInstance();
        }
    }
}