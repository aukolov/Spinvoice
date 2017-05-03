using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.Exchange;
using Spinvoice.Infrastructure.DataAccess;
using Spinvoice.Infrastructure.Pdf;
using Spinvoice.ViewModels;

namespace Spinvoice.Services
{
    public class Bootstrapper
    {
        public static AppViewModel Init()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            var logConfigurator = new LogConfigurator();
            logConfigurator.Configure();

            var dataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Spinvoice",
                "data");
            var documentStoreRepository = new DocumentStoreRepository(dataDirectory);
            var companyDataAccess = new CompanyDataAccess(documentStoreRepository);
            var companyRepository = new CompanyRepository(companyDataAccess);
            var exchangeRateDataAccess = new ExchangeRateDataAccess(documentStoreRepository);
            var exchangeRatesLoader = new ExchangeRatesLoader(exchangeRateDataAccess);
            var exchangeRatesRepository = new ExchangeRatesRepository(exchangeRateDataAccess);
            var appMetadataDataAccess = new AppMetadataDataAccess(documentStoreRepository);
            var appMetadataRepository = new AppMetadataRepository(appMetadataDataAccess);
            var fileService = new FileService();
            var pdfParser = new PdfParser();
            var analyzeInvoiceService = new AnalyzeInvoiceService(companyRepository);
            var windowManager = new WindowManager();

            return new AppViewModel(
                companyRepository,
                exchangeRatesRepository,
                appMetadataRepository,
                exchangeRatesLoader,
                fileService,
                pdfParser, 
                analyzeInvoiceService,
                windowManager);
        }
    }
}