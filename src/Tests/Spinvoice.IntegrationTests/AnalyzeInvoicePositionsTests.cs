using Autofac;
using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.InvoiceProcessing;
using Spinvoice.Infrastructure.DataAccess;
using Spinvoice.Infrastructure.Pdf;
using Spinvoice.IntegrationTests.Mocks;

namespace Spinvoice.IntegrationTests
{
    [TestFixture]
    public class AnalyzeInvoicePositionsTests
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
            loggingConfiguration.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, consoleTarget));
            LogManager.Configuration = loggingConfiguration;
            LogManager.EnableLogging();
            LogManager.ReconfigExistingLoggers();

            var container = TestContainer.GetBuilder().Build();
            _pdfParser = container.Resolve<IPdfParser>();

            _companyRepository = new CompanyRepository(new CompanyDataAccessMock());
            _analyzeInvoiceService = new AnalyzeInvoiceService(_companyRepository);
            _trainStrategyService = new TrainStrategyService();
        }

        private static object[] GetTestData()
        {
            return TestInputProvider.GetTestData(nameof(AnalyzeInvoicePositionsTests));
        }

        [Test]
        [TestCaseSource(nameof(GetTestData))]
        public void Test(string testName)
        {
            foreach (var input in TestInputProvider.GetInput(testName, "learn"))
            {
                var rawInvoice = JsonUtils.Deserialize<RawInvoice>(input.JsonPath);
                Company company;
                using (_companyRepository.GetByNameForUpdateOrCreate(rawInvoice.CompanyName, out company))
                {
                    _trainStrategyService.Train(company, rawInvoice, _pdfParser.Parse(input.PdfPath));
                }
            }

            foreach (var input in TestInputProvider.GetInput(testName, "test"))
            {
                var invoice = new Invoice();
                _analyzeInvoiceService.Analyze(_pdfParser.Parse(input.PdfPath), invoice);
                var expectedPositions = JsonUtils.Deserialize<Position[]>(input.JsonPath);

                for (var i = 0; i < expectedPositions.Length; i++)
                {
                    var expectedPosition = expectedPositions[i];
                    Assert.IsTrue(invoice.Positions.Count > i, $"Not enough positions: {i}/{expectedPositions.Length}.");
                    var actualPosition = invoice.Positions[i];

                    Assert.AreEqual(expectedPosition.Name, actualPosition.Name, $"Name mismatch. Index: {i}");
                    Assert.AreEqual(expectedPosition.Quantity, actualPosition.Quantity,
                        $"Quantity mismatch. Index: {i}");
                    Assert.AreEqual(expectedPosition.Amount, actualPosition.Amount, $"Amount mismatch. Index: {i}");
                }

                Assert.AreEqual(expectedPositions.Length, invoice.Positions.Count);
            }
        }
    }
}