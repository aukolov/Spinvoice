using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace Spinvoice.Infrastructure.DataAccess
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var dataAccess = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(dataAccess)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces().AsSelf()
                .SingleInstance();
            builder.RegisterAssemblyTypes(dataAccess)
                .Where(t => t.Name.EndsWith("DataAccess"))
                .AsImplementedInterfaces().AsSelf()
                .SingleInstance();

            builder.RegisterType<DocumentStoreContainer>().AsImplementedInterfaces().SingleInstance();
        }
    }
}