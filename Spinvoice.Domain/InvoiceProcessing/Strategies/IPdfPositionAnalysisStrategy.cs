using Spinvoice.Domain.Accounting;

namespace Spinvoice.Domain.InvoiceProcessing.Strategies
{
    public interface IPdfPositionAnalysisStrategy : IStrategy<RawPosition, Position[]>
    {
    }
}