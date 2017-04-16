namespace Spinvoice.Domain.Pdf
{
    public interface IPdfAnalysisStrategy
    {
        string GetValue(PdfModel pdfModel);
        void Train(PdfModel pdfModel, string value);
    }
}