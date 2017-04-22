namespace Spinvoice.Domain.Pdf
{
    public interface IPdfAnalysisStrategy
    {
        string GetValue(PdfModel pdfModel);
        bool Train(PdfModel pdfModel, string value);
    }
}