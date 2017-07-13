using NLog;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.Pdf;
using Spinvoice.Utils;

namespace Spinvoice.Domain.InvoiceProcessing
{
    public class AnalyzeInvoiceService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly ICompanyRepository _companyRepository;

        public AnalyzeInvoiceService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public void Analyze(PdfModel pdfModel, Invoice invoice)
        {
            Logger.Info("Start analyzing invoice.");
            var company = FindCompany(pdfModel);
            if (company == null)
            {
                return;
            }

            invoice.ApplyCompany(company);
            TrySetInvoiceNumber(pdfModel, invoice, company.InvoiceNumberStrategy);
            TrySetInvoiceDate(pdfModel, invoice, company.InvoiceDateStrategy);
            TrySetInvoiceNetAmount(pdfModel, invoice, company.InvoiceNetAmountStrategy);
            AnalyzePositions(pdfModel, invoice, company);
        }

        public void AnalyzePositions(PdfModel pdfModel, Invoice invoice, Company.Company company)
        {
            TrySetPositions(pdfModel, invoice, company.PositionStrategy);
        }

        private Company.Company FindCompany(PdfModel pdfModel)
        {
            Logger.Info("Matching company.");
            var companies = _companyRepository.GetAll();
            Logger.Info($"Companies known: {companies.Length}.");
            foreach (var company in companies)
            {
                Logger.Info($"Matching company '{company.Name}'.");
                if (company.CompanyInvoiceStrategy == null)
                {
                    Logger.Info("Company strategy is empty.");
                    continue;
                }

                var value = company.CompanyInvoiceStrategy.GetValue(pdfModel);
                Logger.Info($"Candidate value: {value}.");
                if (value == company.Name && value != null)
                {
                    Logger.Info("Match found.");
                    return company;
                }
            }
            Logger.Info("Company not found.");
            return null;
        }

        private static void TrySetInvoiceNumber(PdfModel pdfModel, Invoice invoice, IStringPdfAnalysisStrategy strategy)
        {
            var invoiceNumber = strategy?.GetValue(pdfModel);
            if (invoiceNumber != null)
            {
                invoice.InvoiceNumber = invoiceNumber.Trim();
            }
        }

        private static void TrySetInvoiceDate(PdfModel pdfModel, Invoice invoice, IStringPdfAnalysisStrategy strategy)
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

        private static void TrySetInvoiceNetAmount(PdfModel pdfModel, Invoice invoice, IStringPdfAnalysisStrategy strategy)
        {
            var stringInvoiceNetAmount = strategy?.GetValue(pdfModel);
            if (stringInvoiceNetAmount != null)
            {
                invoice.NetAmount = AmountParser.Parse(stringInvoiceNetAmount.Trim());
            }
        }

        private void TrySetPositions(
            PdfModel pdfModel,
            Invoice invoice,
            IPdfPositionAnalysisStrategy strategy)
        {
            var positions = strategy?.GetValue(pdfModel);
            if (positions == null) return;

            invoice.Positions.Clear();
            invoice.Positions.AddRange(positions);
        }

    }
}