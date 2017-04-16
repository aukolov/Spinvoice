namespace Spinvoice.Domain.Pdf
{
    public class NextTokenStrategy : IPdfAnalysisStrategy
    {
        public string PreviousText { get; set; }

        public string GetValue(PdfModel pdfModel)
        {
            if (PreviousText == null)
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

        public void Study(PdfModel pdfModel, string value)
        {
            if (value == null)
            {
                return;
            }
            foreach (var blockModel in pdfModel.BlockModels)
            {
                for (var i = 1; i < blockModel.Sentences.Count; i++)
                {
                    if (blockModel.Sentences[i].Trim() == value.Trim())
                    {
                        PreviousText = blockModel.Sentences[i - 1];
                        return;
                    }
                }
            }
        }
    }
}