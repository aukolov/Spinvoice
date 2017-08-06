using Autofac;
using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.InvoiceProcessing;
using Spinvoice.Domain.Pdf;
using Spinvoice.Infrastructure.DataAccess;
using Spinvoice.IntegrationTests.Mocks;

namespace Spinvoice.IntegrationTests
{
    [TestFixture]
    public partial class AnalyzeInvoicePositionsTests
    {
        private IPdfParser _pdfParser;
        private CompanyRepository _companyRepository;
        private AnalyzeInvoiceService _analyzeInvoiceService;
        private TrainStrategyService _trainStrategyService;

        [SetUp]
        public void Setup()
        {
            var loggingConfiguration = new LoggingConfiguration();
            var consoleTarget = new ConsoleTarget("console target");
            loggingConfiguration.AddTarget(consoleTarget);
            loggingConfiguration.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, consoleTarget));
            LogManager.Configuration = loggingConfiguration;
            LogManager.EnableLogging();
            LogManager.ReconfigExistingLoggers();

            var container = TestContainer.GetBuilder().Build();
            _pdfParser = container.Resolve<IPdfParser>();

            _companyRepository = new CompanyRepository(new CompanyDataAccessMock());
            _analyzeInvoiceService = new AnalyzeInvoiceService(_companyRepository);
            _trainStrategyService = new TrainStrategyService();
        }

        [Test]
        [TestCaseSource(nameof(GetTestData))]
        public void Test(TestCase testCase)
        {
            Company company;
            using (_companyRepository.GetByNameForUpdateOrCreate(testCase.RawInvoice.CompanyName, out company))
            {
                var pdfModel = _pdfParser.Parse(TestInputProvider.GetTestPath(
                    nameof(AnalyzeInvoicePositionsTests), testCase.Name, "learn.pdf"));
                _trainStrategyService.Train(company, testCase.RawInvoice, pdfModel);
            }

            var testPdfPath = TestInputProvider.GetTestPath(
                nameof(AnalyzeInvoicePositionsTests), testCase.Name, testCase.TestFileName);
            var invoice = new Invoice();
            _analyzeInvoiceService.Analyze(_pdfParser.Parse(testPdfPath), invoice);

            for (var i = 0; i < testCase.ExpectedPositions.Length; i++)
            {
                var expectedPosition = testCase.ExpectedPositions[i];
                Assert.IsTrue(invoice.Positions.Count > i);
                var actualPosition = invoice.Positions[i];

                Assert.AreEqual(expectedPosition.Name, actualPosition.Name, $"Name mismatch. Index: {i}");
                Assert.AreEqual(expectedPosition.Quantity, actualPosition.Quantity, $"Quantity mismatch. Index: {i}");
                Assert.AreEqual(expectedPosition.Amount, actualPosition.Amount, $"Amount mismatch. Index: {i}");
            }
            Assert.AreEqual(testCase.ExpectedPositions.Length, invoice.Positions.Count);
        }

        public class TestCase
        {
            public string Name { get; }
            public RawInvoice RawInvoice { get; }
            public string TestFileName { get; }
            public Position[] ExpectedPositions { get; }

            public TestCase(
                string name,
                RawInvoice rawInvoice,
                string testFileName,
                Position[] expectedPositions)
            {
                Name = name;
                RawInvoice = rawInvoice;
                TestFileName = testFileName;
                ExpectedPositions = expectedPositions;
            }

            public override string ToString()
            {
                return $"{Name} {TestFileName}";
            }
        }

    }
}