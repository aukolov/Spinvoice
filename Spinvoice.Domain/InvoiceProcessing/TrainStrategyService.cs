using System.Collections.Generic;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.InvoiceProcessing.Strategies;
using Spinvoice.Domain.Pdf;

namespace Spinvoice.Domain.InvoiceProcessing
{
    public class TrainStrategyService
    {
        public void Train(Company.Company company, RawInvoice rawInvoice, PdfModel pdfModel)
        {
            company.CompanyInvoiceStrategy = (IStringPdfAnalysisStrategy)TrainStrategy(
                company.CompanyInvoiceStrategy,
                pdfModel,
                rawInvoice.CompanyName,
                GetCompanyStrategies());

            company.InvoiceNumberStrategy = (IStringPdfAnalysisStrategy)TrainStrategy(
                company.InvoiceNumberStrategy,
                pdfModel,
                rawInvoice.InvoiceNumber,
                GetStrategies());

            company.InvoiceDateStrategy = (IStringPdfAnalysisStrategy)TrainStrategy(
                company.InvoiceDateStrategy,
                pdfModel,
                rawInvoice.Date,
                GetStrategies());

            company.InvoiceNetAmountStrategy = (IStringPdfAnalysisStrategy)TrainStrategy(
                company.InvoiceNetAmountStrategy,
                pdfModel,
                rawInvoice.NetAmount,
                GetStrategies());

            company.PositionStrategy = (IPdfPositionAnalysisStrategy)TrainStrategy(
                company.PositionStrategy,
                pdfModel,
                rawInvoice.FirstPosition,
                GetPositionStrategies());
        }

        private static IStrategy<TRaw, TResult> TrainStrategy<TRaw, TResult>(
            IStrategy<TRaw, TResult> strategy,
            PdfModel pdfModel,
            TRaw value,
            IEnumerable<IStrategy<TRaw, TResult>> candidateStrategies)
        {
            if (value == null)
            {
                return strategy;
            }
            if (strategy == null)
            {
                foreach (var candidateStrategy in candidateStrategies)
                {
                    if (candidateStrategy.Train(pdfModel, value))
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

        private static IEnumerable<IStringPdfAnalysisStrategy> GetCompanyStrategies()
        {
            yield return new NextTokenStrategy();
            yield return new ContainsStrategy();
            yield return new InsideTokensStrategy();
        }

        private static IEnumerable<IStringPdfAnalysisStrategy> GetStrategies()
        {
            yield return new NextTokenStrategy();
            yield return new InsideTokensStrategy();
        }

        private static IEnumerable<IPdfPositionAnalysisStrategy> GetPositionStrategies()
        {
            yield return new UnderAmountPositionStrategy();
        }
    }
}