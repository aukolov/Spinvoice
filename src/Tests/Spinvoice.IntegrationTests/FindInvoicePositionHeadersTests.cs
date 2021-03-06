﻿using System.Linq;
using Autofac;
using NUnit.Framework;
using Spinvoice.Common.Domain.Pdf;
using Spinvoice.Domain.InvoiceProcessing;
using Spinvoice.Domain.Pdf;
using Spinvoice.Infrastructure.DataAccess;
using Spinvoice.Infrastructure.Pdf;
using Spinvoice.IntegrationTests.Mocks;

namespace Spinvoice.IntegrationTests
{
    [TestFixture]
    public class FindInvoicePositionHeadersTests
    {
        private CompanyRepository _companyRepository;
        private IPdfParser _pdfParser;

        [SetUp]
        public void Setup()
        {
            _companyRepository = new CompanyRepository(new CompanyDataAccessMock());

            var container = TestContainer.GetBuilder().Build();
            _pdfParser = container.Resolve<IPdfParser>();
        }

        private static object[] GetTestData()
        {
            return TestInputProvider.GetTestData(nameof(FindInvoicePositionHeadersTests));
        }

        [Test]
        [TestCaseSource(nameof(GetTestData))]
        public void FindsHeaderLineInPdfModel(string testName)
        {
            foreach (var input in TestInputProvider.GetInput(testName, "test"))
            {
                var learnData = JsonUtils.Deserialize<LearnData>(input.JsonPath);
                var pdfModel = _pdfParser.Parse(input.PdfPath);
                var locationRanges = pdfModel.Find(learnData.HeaderLine).ToArray();

                for (var i = 0; i < learnData.LocationRanges.Length; i++)
                {
                    Assert.IsTrue(i < locationRanges.Length, $"Match #{i} is missing.");
                    AssertLocation(learnData.LocationRanges[i].Start, locationRanges[i].Start);
                    AssertLocation(learnData.LocationRanges[i].End, locationRanges[i].End);
                }
            }
        }

        private static void AssertLocation(Location location1, Location location2)
        {
            Assert.AreEqual(location1.Page, location2.Page, "Page");
            Assert.AreEqual(location1.Block, location2.Block, "Block");
            Assert.AreEqual(location1.Sentence, location2.Sentence, "Sentence");
        }

        private class LearnData
        {
            public string HeaderLine { get; set; }
            public LocationRange[] LocationRanges { get; set; }
        }

    }
}