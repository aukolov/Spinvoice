﻿using System;
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
            var filePath = @"C:\Projects\my\sibil\08-10\1603865-13029.pdf";
            var pdfParser = new PdfParser();
            var pdfModel = pdfParser.Parse(filePath);
            Console.WriteLine(pdfModel.GetText());
        }
    }
}