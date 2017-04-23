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
                if (sentence.StartsWith(searchText))
                {
                    return sentence.Substring(searchText.Length).Trim();
                }
            }
            return null;
        }

        public bool Train(PdfModel pdfModel, string value)
        {
            value = " " + value;
            foreach (var sentence in pdfModel.Sentences)
            {
                if (!sentence.EndsWith(value)) continue;
                var candidate = sentence.Substring(0, sentence.Length - value.Length).Trim();

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