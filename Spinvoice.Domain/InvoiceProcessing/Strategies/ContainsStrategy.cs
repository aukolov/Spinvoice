using System.Linq;
using NLog;

// ReSharper disable once CheckNamespace
namespace Spinvoice.Domain.Pdf
{
    public class ContainsStrategy : IStringPdfAnalysisStrategy
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        // ReSharper disable once MemberCanBePrivate.Global
        public string Value { get; set; }

        public string GetValue(PdfModel pdfModel)
        {
            Logger.Info($"Seraching for '{Value}'.");
            return !string.IsNullOrEmpty(Value) && Contains(pdfModel, Value) ? Value : null;
        }

        public bool Train(PdfModel pdfModel, string value)
        {
            Logger.Info($"Training with value '{value}'.");
            if (Contains(pdfModel, value))
            {
                Value = value;
                Logger.Info("Value found.");
                return true;
            }
            Logger.Info("Value not found.");
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