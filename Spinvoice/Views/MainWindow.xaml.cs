using Spinvoice.Domain.Company;
using Spinvoice.Domain.Exchange;
using Spinvoice.Infrastructure.DataAccess;
using Spinvoice.Infrastructure.Pdf;
using Spinvoice.Services;
using Spinvoice.ViewModels;

namespace Spinvoice.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var documentStoreRepository = new DocumentStoreRepository();
            var companyDataAccess = new CompanyDataAccess(documentStoreRepository);
            var companyRepository = new CompanyRepository(companyDataAccess);
            var exchangeRateDataAccess = new ExchangeRateDataAccess(documentStoreRepository);
            var exchangeRatesLoader = new ExchangeRatesLoader(exchangeRateDataAccess);
            var exchangeRatesRepository = new ExchangeRatesRepository(exchangeRateDataAccess);
            var fileService = new FileService();
            var pdfParser = new PdfParser();

            DataContext = new AppViewModel(
                companyRepository,
                exchangeRatesRepository, 
                exchangeRatesLoader, 
                fileService,
                pdfParser, new AnalyzeInvoiceService(companyRepository));
        }
    }
}