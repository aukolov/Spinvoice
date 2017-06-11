// ReSharper disable once CheckNamespace
namespace Spinvoice.Domain.Pdf
{
    public interface IStrategy<in TRaw, out TResult>
    {
        TResult GetValue(PdfModel pdfModel);
        bool Train(PdfModel pdfModel, TRaw value);
    }
}