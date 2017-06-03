namespace Spinvoice.Domain.Pdf
{
    public class SentenceModel
    {
        public string Text { get; }

        public SentenceModel(string text)
        {
            Text = text;
        }
    }
}