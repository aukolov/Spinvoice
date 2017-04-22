using Spinvoice.Domain;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.Pdf;
using Spinvoice.Domain.Utils;

namespace Spinvoice.Services
{
    public class AnalyzeInvoiceService
    {
        private readonly ICompanyRepository _companyRepository;

        public AnalyzeInvoiceService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public void Analyze(PdfModel pdfModel, Invoice invoice)
        {
            var company = FindCompany(pdfModel);
            if (company != null)
            {
                invoice.ApplyCompany(company);
                TrySetInvoiceNumber(pdfModel, invoice, company.InvoiceNumberStrategy);
                TrySetInvoiceDate(pdfModel, invoice, company.InvoiceDateStrategy);
                TrySetInvoiceNetAmount(pdfModel, invoice, company.InvoiceNetAmountStrategy);
            }
        }

        private Company FindCompany(PdfModel pdfModel)
        {
            var companies = _companyRepository.GetAll();
            foreach (var company in companies)
            {
                if (company.CompanyInvoiceStrategy == null)
                {
                    continue;
                }

                var value = company.CompanyInvoiceStrategy.GetValue(pdfModel);
                if (value == company.Name && value != null)
                {
                    return company;
                }
            }
            return null;
        }

        private static void TrySetInvoiceNumber(PdfModel pdfModel, Invoice invoice, IPdfAnalysisStrategy strategy)
        {
            var invoiceNumber = strategy?.GetValue(pdfModel);
            if (invoiceNumber != null)
            {
                invoice.InvoiceNumber = invoiceNumber.Trim();
            }
        }

        private static void TrySetInvoiceDate(PdfModel pdfModel, Invoice invoice, IPdfAnalysisStrategy strategy)
        {
            var stringInvoiceDate = strategy?.GetValue(pdfModel);
            if (stringInvoiceDate != null)
            {
                var invoiceDate = DateParser.TryParseDate(stringInvoiceDate.Trim());
                if (invoiceDate.HasValue)
                {
                    invoice.Date = invoiceDate.Value;
                }
            }
        }

        private static void TrySetInvoiceNetAmount(PdfModel pdfModel, Invoice invoice, IPdfAnalysisStrategy strategy)
        {
            var stringInvoiceNetAmount = strategy?.GetValue(pdfModel);
            if (stringInvoiceNetAmount != null)
            {
                invoice.NetAmount = AmountParser.Parse(stringInvoiceNetAmount.Trim());
            }
        }

        public void Learn(Company company, RawInvoice rawInvoice, PdfModel pdfModel)
        {
            company.CompanyInvoiceStrategy = TrainStrategy(company.CompanyInvoiceStrategy, pdfModel, rawInvoice.CompanyName);
            company.InvoiceNumberStrategy = TrainStrategy(company.InvoiceNumberStrategy, pdfModel, rawInvoice.InvoiceNumber);
            company.InvoiceDateStrategy = TrainStrategy(company.InvoiceDateStrategy, pdfModel, rawInvoice.Date);
            company.InvoiceNetAmountStrategy = TrainStrategy(company.InvoiceNetAmountStrategy, pdfModel, rawInvoice.NetAmount);
        }

        private static IPdfAnalysisStrategy TrainStrategy(IPdfAnalysisStrategy strategy, PdfModel pdfModel, string value)
        {
            if (strategy == null)
            {
                strategy = new NextTokenStrategy();
            }
            strategy.Train(pdfModel, value);
            return strategy;
        }
    }
}