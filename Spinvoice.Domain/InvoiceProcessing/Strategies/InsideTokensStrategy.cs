using NLog;

// ReSharper disable once CheckNamespace
namespace Spinvoice.Domain.Pdf
{
    public class InsideTokensStrategy : IStringPdfAnalysisStrategy
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public string Text { get; set; }

        public string GetValue(PdfModel pdfModel)
        {
            Logger.Info("Start getting value with text '{0}'.", Text);
            if (string.IsNullOrEmpty(Text))
            {
                Logger.Warn("Text is null or empty.");
                return null;
            }
            var searchText = Text + " ";
            foreach (var sentence in pdfModel.Sentences)
            {
                if (sentence.Text.StartsWith(searchText))
                {
                    var result = sentence.Text.Substring(searchText.Length).Trim();
                    Logger.Info("Found text: '{0}'.", result);
                    return result;
                }
            }
            Logger.Info("Value not found.");
            return null;
        }

        public bool Train(PdfModel pdfModel, string value)
        {
            Logger.Info("Start training with text '{0}'.", value);
            value = " " + value;
            foreach (var sentence in pdfModel.Sentences)
            {
                if (!sentence.Text.EndsWith(value)) continue;
                var candidate = sentence.Text
                    .Substring(0, sentence.Text.Length - value.Length)
                    .Trim();

                if (TextUtils.IsNumber(candidate) 
                    || TextUtils.EndsWithNumber(candidate)
                    || !TextUtils.IsNonTrivialString(candidate)) continue;

                Logger.Info("Text found: '{0}'.", candidate);
                Text = candidate;
                return true;
            }

            Logger.Info("Text not found.");
            return false;
        }

        public override string ToString()
        {
            return $"InsideTokensStrategy: {Text}";
        }
    }
}