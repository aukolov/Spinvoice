namespace Spinvoice.Common.Domain.Pdf
{
    public class SentenceModel
    {
        public SentenceModel(
            string text,
            double left,
            double bottom,
            double width,
            double height)
        {
            Text = text;
            Left = left;
            Bottom = bottom;
            Width = width;
            Height = height;
        }

        public string Text { get; }
        public int PageIndex { get; set; }

        public double Left { get; }
        public double Bottom { get; }
        public double Right => Left + Width;
        public double Top => Bottom - Height;
        public double MidX => Left + Width / 2;
        public double Width { get; }
        public double Height { get; }
    }
}