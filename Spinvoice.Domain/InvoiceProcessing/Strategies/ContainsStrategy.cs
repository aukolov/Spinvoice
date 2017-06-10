using System.Linq;
using Spinvoice.Domain.Pdf;

namespace Spinvoice.Domain.InvoiceProcessing.Strategies
{
    public class ContainsStrategy : IStringPdfAnalysisStrategy
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public string Value { get; set; }

        public string GetValue(PdfModel pdfModel)
        {
            return !string.IsNullOrEmpty(Value) && Contains(pdfModel, Value) ? Value : null;
        }

        public bool Train(PdfModel pdfModel, string value)
        {
            if (Contains(pdfModel, value))
            {
                Value = value;
                return true;
            }
            return false;
        }

        private static bool Contains(PdfModel pdfModel, string value)
        {
            return pdfModel.Sentences.Any(s => s.Text == value);
        }

        public override string ToString()
        {
            return $"ContainsStrategy: {Value}";
        }

    }
}