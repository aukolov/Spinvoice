using Autofac;
using Spinvoice.IntegrationTests.Mocks;
// ReSharper disable RedundantNameQualifier

namespace Spinvoice.IntegrationTests
{
    public static class TestContainer
    {
        public static ContainerBuilder GetBuilder()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<Spinvoice.Infrastructure.Pdf.AutofacModule>();
            containerBuilder.RegisterModule<Spinvoice.Server.Infrastructure.Pdf.Text7.AutofacModule>();
            containerBuilder.RegisterType<TesseractDataPathProviderMock>().AsImplementedInterfaces().SingleInstance();
            return containerBuilder;
        }
    }
}