using System;
using NUnit.Framework;
using Spinvoice.Domain;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.Company;
using Spinvoice.Infrastructure.Pdf;
using Spinvoice.IntegrationTests.Mocks;
using Spinvoice.Services;

namespace Spinvoice.IntegrationTests
{
    [TestFixture]
    public class AnalyzeInvoiceServiceTests
    {
        private CompanyRepository _companyRepository;
        private AnalyzeInvoiceService _service;
        private PdfParser _pdfParser;

        [SetUp]
        public void Setup()
        {
            _companyRepository = new CompanyRepository(new CompanyDataAccessMock());
            _service = new AnalyzeInvoiceService(_companyRepository);
            _pdfParser = new PdfParser();
        }

        private static object[] GetTestData()
        {
            return TestInputProvider.GetTestData(nameof(AnalyzeInvoiceServiceTests));
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
                    _service.Learn(company, rawInvoice, _pdfParser.Parse(input.PdfPath));
                }
            }

            Console.WriteLine($@"Company: {company}");

            foreach (var input in TestInputProvider.GetInput(testName, "test"))
            {
                var invoice = new Invoice();
                _service.Analyze(_pdfParser.Parse(input.PdfPath), invoice);
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
