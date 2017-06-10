using System.Collections.Generic;
using Spinvoice.Domain;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.Pdf;
using Spinvoice.Utils;

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
            if (company == null) return;

            invoice.ApplyCompany(company);
            TrySetInvoiceNumber(pdfModel, invoice, company.InvoiceNumberStrategy);
            TrySetInvoiceDate(pdfModel, invoice, company.InvoiceDateStrategy);
            TrySetInvoiceNetAmount(pdfModel, invoice, company.InvoiceNetAmountStrategy);
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
            company.CompanyInvoiceStrategy = TrainStrategy(company.CompanyInvoiceStrategy, 
                pdfModel, rawInvoice.CompanyName, GetCompanyStrategies());
            company.InvoiceNumberStrategy = TrainStrategy(company.InvoiceNumberStrategy, 
                pdfModel, rawInvoice.InvoiceNumber, GetStrategies());
            company.InvoiceDateStrategy = TrainStrategy(company.InvoiceDateStrategy, 
                pdfModel, rawInvoice.Date, GetStrategies());
            company.InvoiceNetAmountStrategy = TrainStrategy(company.InvoiceNetAmountStrategy, 
                pdfModel, rawInvoice.NetAmount, GetStrategies());
        }

        private static IPdfAnalysisStrategy TrainStrategy(
            IPdfAnalysisStrategy strategy, 
            PdfModel pdfModel, 
            string value, 
            IEnumerable<IPdfAnalysisStrategy> candidateStrategies)
        {
            if (string.IsNullOrEmpty(value))
            {
                return strategy;
            }
            if (strategy == null)
            {
                foreach (var candidateStrategy in candidateStrategies)
                {
                    if (candidateStrategy.Train(pdfModel, value)
                        && candidateStrategy.GetValue(pdfModel) == value)
                    {
                        return candidateStrategy;
                    }
                }
            }
            else
            {
                strategy.Train(pdfModel, value);
            }
            return strategy;
        }

        private static IEnumerable<IPdfAnalysisStrategy> GetCompanyStrategies()
        {
            yield return new NextTokenStrategy();
            yield return new ContainsStrategy();
            yield return new InsideTokensStrategy();
        }

        private static IEnumerable<IPdfAnalysisStrategy> GetStrategies()
        {
            yield return new NextTokenStrategy();
            yield return new InsideTokensStrategy();
        }
    }
}