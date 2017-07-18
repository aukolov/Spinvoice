using System;
using NUnit.Framework;
using Spinvoice.Infrastructure.Pdf;

namespace Spinvoice.Tests.Infrastructure.Pdf
{
    [TestFixture]
    //[Ignore("Local run only")]
    public class PdfTest
    {
        [Test]
        public void ParsePdf()
        {
            var filePath = @"C:\Users\aukolov\Downloads\Samsung_\Samsung 25.10.pdf";
            var pdfParser = new PdfParser();
            var pdfModel = pdfParser.Parse(filePath);
            Console.WriteLine(pdfModel.GetText());
        }
    }
}