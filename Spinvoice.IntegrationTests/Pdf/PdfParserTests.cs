using System;
using Autofac;
using NUnit.Framework;
using Spinvoice.Common.Domain.Pdf;
using Spinvoice.Domain.Pdf;

namespace Spinvoice.IntegrationTests.Pdf
{
    [TestFixture]
    public class PdfParserTests
    {
        private IPdfParser _pdfParser;

        [SetUp]
        public void SetUp()
        {
            var container = TestContainer.GetBuilder().Build();
            _pdfParser = container.Resolve<IPdfParser>();
        }

        [Test]
        [TestCase("PdfWithScans")]
        public void ParsePdf(string testCaseName)
        {
            var filePath = TestInputProvider.GetTestPath(
                nameof(PdfParserTests),
                testCaseName,
                "test.pdf");

            var pdfModel = _pdfParser.Parse(filePath);
            Console.WriteLine(pdfModel.GetText());
        }
    }
}