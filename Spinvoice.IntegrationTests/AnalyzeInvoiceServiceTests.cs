using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using Spinvoice.Domain;
using Spinvoice.Domain.Company;
using Spinvoice.Infrastructure.Pdf;
using Spinvoice.IntegrationTests.Mocks;
using Spinvoice.Services;

namespace Spinvoice.IntegrationTests
{
    [TestFixture()]
    public class AnalyzeInvoiceServiceTests
    {
        private const string TestInputPath = @"C:\Projects\my\Spinvoice.TestResources";
        private CompanyRepository _companyRepository;
        private AnalyzeInvoiceService _service;
        private PdfParser _pdfParser;

        private static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented,
                new JsonSerializerSettings { Culture = CultureInfo.InvariantCulture });
        }

        [SetUp]
        public void Setup()
        {
            _companyRepository = new CompanyRepository(new CompanyDataAccessMock());
            _service = new AnalyzeInvoiceService(_companyRepository);
            _pdfParser = new PdfParser();
        }

        private static string GetPath(string testName, string fileName = "")
        {
            return $@"{TestInputPath}\{testName}\{fileName}";
        }

        private static T Deserialize<T>(string filePath)
        {
            var text = File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<T>(text, new JsonSerializerSettings
            {
                Culture = CultureInfo.InvariantCulture
            });
            return result;
        }

        private static IEnumerable<TestInput> GetInput(string testName, string category)
        {
            var fileNames = Directory.GetFiles(GetPath(testName)).Select(Path.GetFileName).ToArray();
            foreach (var pdf in fileNames.Where(s => s.StartsWith(category) && s.EndsWith(".pdf")))
            {
                var json = Path.ChangeExtension(pdf, ".json");
                yield return new TestInput(GetPath(testName, pdf), GetPath(testName, json));
            }
        }

        public static object[] GetTestData()
        {
            var data = Directory.GetDirectories(TestInputPath)
                .Select(Path.GetFileName)
                .Select(s => new object[] { s })
                .Cast<object>()
                .ToArray();
            return data;
        }

        [Test]
        [TestCaseSource(nameof(GetTestData))]
        public void Test(string testName)
        {
            Company company = null;
            foreach (var input in GetInput(testName, "learn"))
            {
                var rawInvoice = Deserialize<RawInvoice>(input.JsonPath);
                using (_companyRepository.GetByNameForUpdateOrCreate(rawInvoice.CompanyName, out company))
                {
                    _service.Learn(company, rawInvoice, _pdfParser.Parse(input.PdfPath));
                }
            }

            Console.WriteLine($@"Company: {company}");

            foreach (var input in GetInput(testName, "test"))
            {
                var invoice = new Invoice();
                _service.Analyze(_pdfParser.Parse(input.PdfPath), invoice);
                var json = Deserialize<ParsedInvoice>(input.JsonPath);
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

        private class TestInput
        {
            public string PdfPath { get; }
            public string JsonPath { get; }

            public TestInput(string pdfPath, string jsonPath)
            {
                PdfPath = pdfPath;
                JsonPath = jsonPath;
            }
        }
    }
}
