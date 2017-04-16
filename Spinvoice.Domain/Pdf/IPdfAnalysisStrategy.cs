namespace Spinvoice.Domain.Pdf
{
    public interface IPdfAnalysisStrategy
    {
        string GetValue(PdfModel pdfModel);
        void Study(PdfModel pdfModel, string value);
    }
}