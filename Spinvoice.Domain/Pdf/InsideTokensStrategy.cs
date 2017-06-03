namespace Spinvoice.Domain.Pdf
{
    public class InsideTokensStrategy : IPdfAnalysisStrategy
    {
        public string PreviousText { get; set; }

        public string GetValue(PdfModel pdfModel)
        {
            if (string.IsNullOrEmpty(PreviousText))
            {
                return null;
            }
            var searchText = PreviousText + " ";
            foreach (var sentence in pdfModel.Sentences)
            {
                if (sentence.Text.StartsWith(searchText))
                {
                    return sentence.Text.Substring(searchText.Length).Trim();
                }
            }
            return null;
        }

        public bool Train(PdfModel pdfModel, string value)
        {
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

                PreviousText = candidate;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return $"InsideTokensStrategy: {PreviousText}";
        }
    }
}