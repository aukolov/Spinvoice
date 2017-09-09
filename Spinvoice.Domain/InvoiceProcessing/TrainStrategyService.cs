using System.Collections.Generic;
using NLog;
using Spinvoice.Common.Domain.Pdf;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.InvoiceProcessing.Strategies;
using Spinvoice.Domain.Pdf;

namespace Spinvoice.Domain.InvoiceProcessing
{
    public class TrainStrategyService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Train(Company.Company company, RawInvoice rawInvoice, PdfModel pdfModel)
        {
            Logger.Info($"Start training company {company.Name} with file {pdfModel.FileName}");

            Logger.Info("Training company invoice strategy.");
            company.CompanyInvoiceStrategy = (IStringPdfAnalysisStrategy)TrainStrategy(
                company.CompanyInvoiceStrategy,
                pdfModel,
                rawInvoice.CompanyName,
                GetCompanyStrategies());

            Logger.Info("Training invoice number strategy.");
            company.InvoiceNumberStrategy = (IStringPdfAnalysisStrategy)TrainStrategy(
                company.InvoiceNumberStrategy,
                pdfModel,
                rawInvoice.InvoiceNumber,
                GetStrategies());

            Logger.Info("Training company date strategy.");
            company.InvoiceDateStrategy = (IStringPdfAnalysisStrategy)TrainStrategy(
                company.InvoiceDateStrategy,
                pdfModel,
                rawInvoice.Date,
                GetStrategies());

            Logger.Info("Training invoice net amount strategy.");
            company.InvoiceNetAmountStrategy = (IStringPdfAnalysisStrategy)TrainStrategy(
                company.InvoiceNetAmountStrategy,
                pdfModel,
                rawInvoice.NetAmount,
                GetAmountStrategies());

            Train(company, rawInvoice.FirstPosition, pdfModel);
        }

        public void Train(Company.Company company, RawPosition rawPosition, PdfModel pdfModel)
        {
            Logger.Info("Training positions strategy.");
            company.PositionStrategy = (IPdfPositionAnalysisStrategy)TrainStrategy(
                company.PositionStrategy,
                pdfModel,
                rawPosition,
                GetPositionStrategies());
        }

        private static IStrategy<TRaw, TResult> TrainStrategy<TRaw, TResult>(
            IStrategy<TRaw, TResult> strategy,
            PdfModel pdfModel,
            TRaw value,
            IEnumerable<IStrategy<TRaw, TResult>> candidateStrategies)
        {
            var stringValue = value as string;
            if (value == null || stringValue != null && stringValue.Length == 0)
            {
                Logger.Info("Value is null or empty. Cannot train.");
                return strategy;
            }

            if (strategy == null)
            {
                Logger.Info("Current strategy is null.");
                foreach (var candidateStrategy in candidateStrategies)
                {
                    Logger.Info($"Trying {candidateStrategy.GetType().Name} strategy.");
                    if (candidateStrategy.Train(pdfModel, value))
                    {
                        Logger.Info("Training successful.");
                        return candidateStrategy;
                    }
                    Logger.Info("Training failed.");
                }
            }
            else
            {
                Logger.Info($"Current strategy is {strategy.GetType().Name}. Training again.");
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
            yield return new LocationStrategy();
            yield return new NextTokenStrategy();
            yield return new InsideTokensStrategy();
        }

        private static IEnumerable<IStringPdfAnalysisStrategy> GetAmountStrategies()
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