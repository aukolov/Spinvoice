// ReSharper disable once CheckNamespace

using Spinvoice.Common.Domain.Pdf;

namespace Spinvoice.Domain.Pdf
{
    public interface IStrategy<in TRaw, out TResult>
    {
        TResult GetValue(PdfModel pdfModel);
        bool Train(PdfModel pdfModel, TRaw value);
    }
}