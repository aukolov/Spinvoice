using Spinvoice.Domain.Pdf;

namespace Spinvoice.Domain.InvoiceProcessing.Strategies
{
    public interface IStrategy<in TRaw, out TResult>
    {
        TResult GetValue(PdfModel pdfModel);
        bool Train(PdfModel pdfModel, TRaw value);
    }
}