using Spinvoice.Domain.Accounting;

// ReSharper disable once CheckNamespace
namespace Spinvoice.Domain.Pdf
{
    public interface IPdfPositionAnalysisStrategy : IStrategy<RawPosition, Position[]>
    {
    }
}