using System.Reflection;
using Autofac;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Invoice;
using Spinvoice.QuickBooks.Reporting;
using Module = Autofac.Module;

namespace Spinvoice.QuickBooks
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces().AsSelf()
                .SingleInstance();
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("ViewModel"))
                .AsImplementedInterfaces().AsSelf()
                .InstancePerDependency();
            builder.RegisterType<ExternalConnection>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<ExternalAuthService>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<ExternalInvoiceUpdater>().AsImplementedInterfaces().AsSelf().InstancePerDependency();
            builder.RegisterType<ExternalInvoiceAndBillService>().AsImplementedInterfaces().AsSelf().InstancePerDependency();
            builder.RegisterType<ExternalBillCrudService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<InventoryValuationReportService>().AsImplementedInterfaces().SingleInstance();
        }
    }
}