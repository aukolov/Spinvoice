using Autofac;
using Spinvoice.IntegrationTests.Mocks;

namespace Spinvoice.IntegrationTests
{
    public static class TestContainer
    {
        public static ContainerBuilder GetBuilder()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<Infrastructure.Pdf.AutofacModule>();
            containerBuilder.RegisterType<TesseractDataPathProviderMock>().AsImplementedInterfaces().SingleInstance();
            return containerBuilder;
        }
    }
}