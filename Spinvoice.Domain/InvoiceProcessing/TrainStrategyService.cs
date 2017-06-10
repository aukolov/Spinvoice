using System.Collections.Generic;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.Pdf;

namespace Spinvoice.Domain.InvoiceProcessing
{
    public class TrainStrategyService
    {
        public void Train(Company.Company company, RawInvoice rawInvoice, PdfModel pdfModel)
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