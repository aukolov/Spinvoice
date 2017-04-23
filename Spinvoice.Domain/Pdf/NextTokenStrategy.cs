namespace Spinvoice.Domain.Pdf
{
    public class NextTokenStrategy : IPdfAnalysisStrategy
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public string PreviousText { get; set; }

        public string GetValue(PdfModel pdfModel)
        {
            if (string.IsNullOrEmpty(PreviousText))
            {
                return null;
            }
            var isNext = false;
            foreach (var blockModel in pdfModel.BlockModels)
            {
                foreach (var sentense in blockModel.Sentences)
                {
                    if (isNext)
                    {
                        return sentense.Trim();
                    }
                    if (sentense.Trim() == PreviousText.Trim())
                    {
                        isNext = true;
                    }
                }
            }
            return null;
        }

        public bool Train(PdfModel pdfModel, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            string candidate = null;
            foreach (var blockModel in pdfModel.BlockModels)
            {
                for (var i = 0; i < blockModel.Sentences.Count; i++)
                {
                    if (blockModel.Sentences[i].Trim() == value.Trim()
                        && !TextUtils.IsNumber(candidate)
                        && TextUtils.IsNonTrivialString(candidate))
                    {
                        PreviousText = candidate;
                        if (i > 0)
                        {
                            return true;
                        }
                    }
                    candidate = blockModel.Sentences[i];
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