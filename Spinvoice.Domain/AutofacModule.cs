using Autofac;
using Spinvoice.Domain.InvoiceProcessing;

namespace Spinvoice.Domain
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AnalyzeInvoiceService>().AsImplementedInterfaces().InstancePerDependency().AsSelf();
            builder.RegisterType<TrainStrategyService>().AsImplementedInterfaces().InstancePerDependency().AsSelf();
        }
    }
}