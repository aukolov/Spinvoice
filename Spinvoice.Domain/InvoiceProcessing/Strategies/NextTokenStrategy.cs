﻿// ReSharper disable CheckNamespace

using NLog;

namespace Spinvoice.Domain.Pdf
{
    public class NextTokenStrategy : IStringPdfAnalysisStrategy
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // ReSharper disable once MemberCanBePrivate.Global
        public string PreviousText { get; set; }

        public string GetValue(PdfModel pdfModel)
        {
            Logger.Info($"Start getting value with previous text '{PreviousText}'.");
            if (string.IsNullOrEmpty(PreviousText))
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
                        Logger.Info($"Found next: '{text}'.");
                        return text;
                    }
                    if (text == PreviousText.Trim())
                    {
                        Logger.Info($"Found preceeding text: '{text}'.");
                        isNext = true;
                    }
                }
            }
            return null;
        }

        public bool Train(PdfModel pdfModel, string value)
        {
            Logger.Info($"Start training with value '{value}'.");
            if (string.IsNullOrEmpty(value))
            {
                Logger.Info("Value is null or empty.");
                return false;
            }
            string candidate = null;
            foreach (var blockModel in pdfModel.BlockModels)
            {
                for (var i = 0; i < blockModel.Sentences.Count; i++)
                {
                    if (blockModel.Sentences[i].Text.Trim() == value.Trim()
                        && !TextUtils.IsNumber(candidate)
                        && TextUtils.IsNonTrivialString(candidate))
                    {
                        Logger.Info($"Previous text: '{candidate}'.");
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