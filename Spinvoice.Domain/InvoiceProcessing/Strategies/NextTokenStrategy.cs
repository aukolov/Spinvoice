// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CheckNamespace

using NLog;
using Spinvoice.Common.Domain.Pdf;
using Spinvoice.Domain.InvoiceProcessing.Strategies;

namespace Spinvoice.Domain.Pdf
{
    public class NextTokenStrategy : IStringPdfAnalysisStrategy
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public string PreviousText { get; set; }

        public string GetValue(PdfModel pdfModel)
        {
            return GetValue(pdfModel, PreviousText);
        }

        private string GetValue(PdfModel pdfModel, string previousText)
        {
            Logger.Trace($"Start getting value with previous text '{previousText}'.");
            if (string.IsNullOrEmpty(previousText))
            {
                return null;
            }
            var isNext = false;
            foreach (var blockModel in pdfModel.BlockModels)
            {
                foreach (var sentense in blockModel.Sentences)
                {
                    var text = sentense.Text.Trim();
                    if (isNext)
                    {
                        Logger.Trace($"Found next: '{text}'.");
                        return text;
                    }
                    if (text == previousText.Trim())
                    {
                        Logger.Trace($"Found preceeding text: '{text}'.");
                        isNext = true;
                    }
                }
            }
            return null;
        }

        public bool Train(PdfModel pdfModel, string value)
        {
            Logger.Trace($"Start training with value '{value}'.");
            if (string.IsNullOrEmpty(value))
            {
                Logger.Trace("Value is null or empty.");
                return false;
            }
            value = value.Trim();
            string candidate = null;
            foreach (var blockModel in pdfModel.BlockModels)
            {
                for (var i = 0; i < blockModel.Sentences.Count; i++)
                {
                    if (blockModel.Sentences[i].Text.Trim() == value
                        && !TextUtils.IsNumber(candidate)
                        && TextUtils.IsNonTrivialString(candidate)
                        && GetValue(pdfModel, candidate) == value)
                    {
                        Logger.Trace($"Previous text: '{candidate}'.");
                        PreviousText = candidate;
                        if (i > 0)
                        {
                            return true;
                        }
                    }
                    candidate = blockModel.Sentences[i].Text;
                }
            }
            return PreviousText != null;
        }

        public override string ToString()
        {
            return $"NextTokenStrategy: {PreviousText}";
        }
    }
}