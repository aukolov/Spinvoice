using Spinvoice.Domain;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.Pdf;

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
            }
        }

        private Company FindCompany(PdfModel pdfModel)
        {
            var companies = _companyRepository.GetAll();
            foreach (var company in companies)
            {
                if (company.CompanyInvoiceStrategy != null)
                {
                    var value = company.CompanyInvoiceStrategy.GetValue(pdfModel);
                    if (value == company.Name)
                    {
                        return company;
                    }
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
    }
}