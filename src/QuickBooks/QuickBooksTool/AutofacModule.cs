using Autofac;
using QuickBooksTool.DataAccess;

namespace QuickBooksTool
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MainViewModel>().AsImplementedInterfaces();
            builder.RegisterType<WindowFactoryProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<OAuthProfileDataAccess>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<AccountsChartRepository>().AsImplementedInterfaces().SingleInstance();
        }
    }
}