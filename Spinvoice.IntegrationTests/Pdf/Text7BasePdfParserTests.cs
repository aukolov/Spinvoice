using NUnit.Framework;
using Spinvoice.Server.Infrastructure.Pdf.Text7;

namespace Spinvoice.IntegrationTests.Pdf
{
    [TestFixture]
    public class Text7BasedPageParserTests
    {
        [Test]
        public void ParsesPdfPage()
        {
            var pdfPath = @"C:\Projects\my\Spinvoice.TestResources\AnalyzeInvoiceTests\Yageo\learn001.pdf";
            var parser = new Text7BasedPageParser();
            parser.Parse(pdfPath, 1);
        }
    }
}