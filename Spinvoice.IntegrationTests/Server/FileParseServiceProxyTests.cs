using System;
using System.Linq;
using NUnit.Framework;
using Spinvoice.Application.Services;
using Spinvoice.Utils;

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
                    nameof(FileParseServiceProxyTests), nameof(ParsesFile), "test.pdf");
                var pdfModel = fileParseServiceProxy.Parse(filePath);
                Assert.IsNotNull(pdfModel);
                Assert.IsTrue(pdfModel.Sentences.Any());
                pdfModel.Sentences.ForEach(model => Console.WriteLine(model.Text));
                Console.WriteLine(pdfModel.Sentences.Last().Text);
            }
        }
    }
}