using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.InvoiceProcessing;
using Spinvoice.Domain.Pdf;
using Spinvoice.Infrastructure.Pdf;
using Spinvoice.IntegrationTests.Mocks;

namespace Spinvoice.IntegrationTests
{
    [TestFixture]
    public class AnalyzeInvoicePositionsTests
    {
        private static readonly PdfParser PdfParser = new PdfParser();

        private CompanyRepository _companyRepository;
        private AnalyzeInvoiceService _analyzeInvoiceService;
        private TrainStrategyService _trainStrategyService;

        [SetUp]
        public void Setup()
        {
            _companyRepository = new CompanyRepository(new CompanyDataAccessMock());
            _analyzeInvoiceService = new AnalyzeInvoiceService(_companyRepository);
            _trainStrategyService = new TrainStrategyService();
        }

        private static object[] GetTestData()
        {
            return GetTestCases().Cast<object>().ToArray();
        }

        private static IEnumerable<TestCase> GetTestCases()
        {
            var testPath = TestInputProvider.GetTestPath(nameof(AnalyzeInvoicePositionsTests), "DigiKey");
            var learnPdfModel = PdfParser.Parse(Path.Combine(testPath, "learn.pdf"));
            var rawInvoice = new RawInvoice
            {
                CompanyName = "DIGI-KEY",
                FirstPosition =
                {
                    Name = "296-11382-5-ND",
                    Quantity = "450",
                    Amount = "737.10"
                }

            };
            var expectedPositions = new[]
            {
                new Position("A107016CT-ND", 1020, 173.40m),
                new Position("*5552126-1-SI-ND", 15, 234m),
                new Position("A102198CT-ND", 120, 11.50m),
                new Position("1276-3364-1-ND", 27, 44.96m),
                new Position("1276-1092-1-ND", 54, 34.23m),
                new Position("311-10.0KCRTR-ND", 5000, 9m),
                new Position("399-8336-1-ND", 6, 5.94m),
                new Position("P150KDATR-ND", 5000, 160m),
                new Position("P150KDACT-ND", 26, 13.78m),
                new Position("311-196KCRTR-ND", 5000, 9m),
                new Position("1276-1187-1-ND", 10, 0.31m),
                new Position("1276-3391-1-ND", 41, 28.86m),
                new Position("311-2058-1-ND", 82, 63.63m),
                new Position("311-30.1KCRTR-ND", 5000, 9m),
                new Position("311-1880-1-ND", 4, 1.12m),
                new Position("311-40.2KCRTR-ND", 5000, 9m),
                new Position("311-475CRTR-ND", 5000, 9m),
                new Position("311-5.49CRTR-ND", 5000, 13m),
                new Position("311-56.2KCRTR-ND", 5000, 9m),
                new Position("311-576KCRTR-ND", 5000, 9m),
                new Position("732-10095-ND", 15, 40.17m),
                new Position("AT24CM01-SHD-B-ND", 70, 122.19m),
                new Position("ATTINY85V-10SU-ND", 35, 63.32m),
                new Position("CAT9555YI-T2CT-ND", 200, 224.60m),
                new Position("Z2607-ND", 50, 131.84m),
                new Position("240-2543-1-ND", 500, 58m),
                new Position("INA163UA-ND", 25, 169.33m),
                new Position("LMP7721MA/NOPB-ND", 300, 695.12m),
                new Position("MCP1727-3302E/SN-ND", 21, 17.22m),
                new Position("445-1542-2-ND", 4000, 280m),
                new Position("*296-4654-1-ND", 500, 81m),
                new Position("TC427CPA-ND", 50, 55m)
            };
            yield return new TestCase(
                testPath,
                learnPdfModel,
                rawInvoice,
                expectedPositions);
        }

        [Test]
        [TestCaseSource(nameof(GetTestData))]
        public void Test(TestCase testCase)
        {
            Company company;
            using (_companyRepository.GetByNameForUpdateOrCreate(testCase.RawInvoice.CompanyName, out company))
            {
                _trainStrategyService.Train(company, testCase.RawInvoice, testCase.LearnPdfModel);
            }

            var testPdfPath = TestInputProvider.GetInput(testCase.Name, "test").Single().PdfPath;
            var invoice = new Invoice();
            _analyzeInvoiceService.Analyze(PdfParser.Parse(testPdfPath), invoice);

            for (var i = 0; i < testCase.ExpectedPositions.Length; i++)
            {
                var expectedPosition = testCase.ExpectedPositions[i];
                Assert.IsTrue(invoice.Positions.Count > i);
                var actualPosition = invoice.Positions[i];

                Assert.AreEqual(expectedPosition.Name, actualPosition.Name);
                Assert.AreEqual(expectedPosition.Quantity, actualPosition.Quantity);
                Assert.AreEqual(expectedPosition.Amount, actualPosition.Amount);
            }
            Assert.AreEqual(testCase.ExpectedPositions.Length, invoice.Positions.Count);
        }

        public class TestCase
        {
            public string Name { get; }
            public PdfModel LearnPdfModel { get; }
            public RawInvoice RawInvoice { get; }
            public Position[] ExpectedPositions { get; }

            public TestCase(
                string name,
                PdfModel learnPdfModel,
                RawInvoice rawInvoice,
                Position[] expectedPositions)
            {
                Name = name;
                LearnPdfModel = learnPdfModel;
                RawInvoice = rawInvoice;
                ExpectedPositions = expectedPositions;
            }
        }
    }
}