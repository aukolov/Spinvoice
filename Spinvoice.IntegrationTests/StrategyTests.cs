using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Spinvoice.Domain;
using Spinvoice.Domain.Company;
using Spinvoice.Infrastructure.Pdf;
using Spinvoice.IntegrationTests.Mocks;
using Spinvoice.Services;

namespace Spinvoice.IntegrationTests
{
    [TestFixture()]
    public class StrategyTests
    {
        private CompanyRepository _companyRepository;
        private AnalyzeInvoiceService _service;
        private PdfParser _pdfParser;

        private static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented,
                new JsonSerializerSettings { Culture = CultureInfo.InvariantCulture });
        }

        //[Test]
        //public void JsonDemo()
        //{
        //    var rawInvoice = new RawInvoice
        //    {
        //        CompanyName = "DIGI-KEY",
        //        Date = "14-SEP-2016",
        //        InvoiceNumber = "54863984",
        //        NetAmount = "1182.60"
        //    };
        //    var parsedInvoice = new ParsedInvoice
        //    {
        //        CompanyName = "DIGI-KEY",
        //        Date = new DateTime(2016, 9, 14),
        //        InvoiceNumber = "54863984",
        //        NetAmount = 1182.60m
        //    };
        //    Console.WriteLine(Serialize(rawInvoice));
        //    Console.WriteLine(Serialize(parsedInvoice));
        //}

        [SetUp]
        public void Setup()
        {
            _companyRepository = new CompanyRepository(new CompanyDataAccessMock());
            _service = new AnalyzeInvoiceService(_companyRepository);
            _pdfParser = new PdfParser();
        }

        private static string GetPath(string testName, string fileName)
        {
            return $@"C:\Projects\my\Spinvoice.TestResources\{testName}\{fileName}";
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

        [Test]
        public void TestDigiKey()
        {
            var testName = "test001";
            var rawInvoice = Deserialize<RawInvoice>(GetPath(testName, "1.json"));
            Company company;
            using (_companyRepository.GetByNameForUpdateOrCreate(rawInvoice.CompanyName, out company))
            {
                _service.Learn(company, rawInvoice, _pdfParser.Parse(GetPath(testName, "1.pdf")));
            }

            var invoice = new Invoice();
            _service.Analyze(_pdfParser.Parse(GetPath(testName, "2.pdf")), invoice);

            var parsedInvoice = Deserialize<ParsedInvoice>(GetPath(testName, "2.json"));
            Assert.AreEqual(parsedInvoice.CompanyName, invoice.CompanyName);
            Assert.AreEqual(parsedInvoice.Date, invoice.Date);
            Assert.AreEqual(parsedInvoice.InvoiceNumber, invoice.InvoiceNumber);
            Assert.AreEqual(parsedInvoice.NetAmount, invoice.NetAmount);
        }
    }
}
