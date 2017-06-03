namespace Spinvoice.Domain.Pdf
{
    public class SentenceModel
    {
        public SentenceModel(string text, double left, double top, double width, double height)
        {
            Text = text;
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public string Text { get; }

        public double Left { get; }
        public double Top { get; }
        public double Width { get; }
        public double Height { get; }
    }
}