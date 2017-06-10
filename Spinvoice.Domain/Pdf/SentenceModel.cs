namespace Spinvoice.Domain.Pdf
{
    public class SentenceModel
    {
        public SentenceModel(
            string text,
            int pageIndex,
            double left,
            double top,
            double width,
            double height)
        {
            Text = text;
            PageIndex = pageIndex;
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public string Text { get; }
        public int PageIndex { get; }

        public double Left { get; }
        public double Top { get; }
        public double Right => Left + Width;
        public double Bottom => Top + Height;
        public double MidX => Left + Width / 2;
        public double Width { get; }
        public double Height { get; }
    }
}