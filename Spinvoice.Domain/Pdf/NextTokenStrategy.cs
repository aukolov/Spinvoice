namespace Spinvoice.Domain.Pdf
{
    public class NextTokenStrategy : IPdfAnalysisStrategy
    {
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

        public void Train(PdfModel pdfModel, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            string previousText = null;
            foreach (var blockModel in pdfModel.BlockModels)
            {
                for (var i = 0; i < blockModel.Sentences.Count; i++)
                {
                    if (blockModel.Sentences[i].Trim() == value.Trim()
                        && !IsNumber(previousText)
                        && IsNonTrivialString(previousText))
                    {
                        PreviousText = previousText;
                        if (i > 0)
                        {
                            return;
                        }
                    }
                    previousText = blockModel.Sentences[i];
                }
            }
        }

        private static bool IsNumber(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }
            decimal d;
            return decimal.TryParse(text, out d);
        }

        private static bool IsNonTrivialString(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            return text.Trim().Length > 0;
        }
    }
}