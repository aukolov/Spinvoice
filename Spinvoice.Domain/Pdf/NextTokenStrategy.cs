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
            foreach (var blockModel in pdfModel.BlockModels)
            {
                for (var i = 0; i < blockModel.Sentences.Count - 1; i++)
                {
                    if (blockModel.Sentences[i].Trim() == PreviousText.Trim())
                    {
                        return blockModel.Sentences[i + 1];
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
            foreach (var blockModel in pdfModel.BlockModels)
            {
                for (var i = 1; i < blockModel.Sentences.Count; i++)
                {
                    if (blockModel.Sentences[i].Trim() == value.Trim()
                        && !IsNumber(blockModel.Sentences[i - 1]))
                    {
                        PreviousText = blockModel.Sentences[i - 1];
                        return;
                    }
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
    }
}