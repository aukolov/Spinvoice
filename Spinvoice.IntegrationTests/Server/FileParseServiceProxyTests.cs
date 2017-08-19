using NUnit.Framework;
using Spinvoice.Application.Services;

namespace Spinvoice.IntegrationTests.Server
{
    [TestFixture]
    public class FileParseServiceProxyTests
    {
        [Test]
        public void CalculatesSum()
        {
            using (var serverManager = new ServerManager())
            {
                serverManager.Start();

                var fileParseServiceProxy = new FileParseServiceProxy();
                var sum = fileParseServiceProxy.Sum(10, 25);
                Assert.AreEqual(35, sum);
            }
        }

        [Test]
        public void ParsesFile()
        {
            using (var serverManager = new ServerManager())
            {
                serverManager.Start();

                var fileParseServiceProxy = new FileParseServiceProxy();

                var filePath = TestInputProvider.GetTestPath(
                    nameof(AnalyzeInvoicePositionsTests), nameof(ParsesFile), "test.pdf");
                //fileParseServiceProxy.Parse(filePath);
            }
        }
    }
}