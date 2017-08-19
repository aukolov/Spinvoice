using System;
using Autofac;
using NUnit.Framework;
using Spinvoice.Common.Domain.Pdf;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.InvoiceProcessing;
using Spinvoice.Domain.Pdf;
using Spinvoice.Infrastructure.DataAccess;
using Spinvoice.Infrastructure.Pdf;
using Spinvoice.IntegrationTests.Mocks;

namespace Spinvoice.IntegrationTests
{
    [TestFixture]
    public class AnalyzeInvoiceTests
    {
        private CompanyRepository _companyRepository;
        private AnalyzeInvoiceService _analyzeInvoiceService;
        private TrainStrategyService _trainStrategyService;
        private IPdfParser _pdfParser;

        [SetUp]
        public void Setup()
        {
            _companyRepository = new CompanyRepository(new CompanyDataAccessMock());
            _analyzeInvoiceService = new AnalyzeInvoiceService(_companyRepository);
            _trainStrategyService = new TrainStrategyService();

            var container = TestContainer.GetBuilder().Build();
            _pdfParser = container.Resolve<IPdfParser>();
        }

        private static object[] GetTestData()
        {
            return TestInputProvider.GetTestData(nameof(AnalyzeInvoiceTests));
        }

        [Test]
        [TestCaseSource(nameof(GetTestData))]
        public void Test(string testName)
        {
            Company company = null;
            foreach (var input in TestInputProvider.GetInput(testName, "learn"))
            {
                var rawInvoice = JsonUtils.Deserialize<RawInvoice>(input.JsonPath);
                using (_companyRepository.GetByNameForUpdateOrCreate(rawInvoice.CompanyName, out company))
                {
                    _trainStrategyService.Train(company, rawInvoice, _pdfParser.Parse(input.PdfPath));
                }
            }

            Console.WriteLine($@"Company: {company}");

            foreach (var input in TestInputProvider.GetInput(testName, "test"))
            {
                var invoice = new Invoice();
                _analyzeInvoiceService.Analyze(_pdfParser.Parse(input.PdfPath), invoice);
                var json = JsonUtils.Deserialize<ParsedInvoice>(input.JsonPath);
                AssertInvoice(invoice, json, input.JsonPath);
            }
        }

        private static void AssertInvoice(Invoice invoice, ParsedInvoice deserialize, string jsonPath)
        {
            Assert.AreEqual(deserialize.CompanyName, invoice.CompanyName, jsonPath);
            Assert.AreEqual(deserialize.Date, invoice.Date, jsonPath);
            Assert.AreEqual(deserialize.InvoiceNumber, invoice.InvoiceNumber, jsonPath);
            Assert.AreEqual(deserialize.NetAmount, invoice.NetAmount, jsonPath);
        }
    }
}
