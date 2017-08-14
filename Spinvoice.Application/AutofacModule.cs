using System.Reflection;
using Autofac;
using Spinvoice.Application.Services;
using Module = Autofac.Module;

namespace Spinvoice.Application
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("ViewModel"))
                .AsImplementedInterfaces().AsSelf()
                .InstancePerDependency()
                .ExternallyOwned();

            builder.RegisterType<ClipboardService>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<ExchangeRatesLoader>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<FileService>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<LogConfigurator>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<WindowManager>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<TaskSchedulerProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ServerApplicationService>().AsImplementedInterfaces().SingleInstance();
        }
    }
}