using iTextSharp.text.pdf.parser;

namespace Spinvoice.Infrastructure.Pdf
{
    internal interface IBrick
    {
        string Text { get; }
        double X { get; }
        double Y { get; }
        double Width { get; }
        double Height { get; }
    }

    internal class TextRenderInfoBrick : IBrick
    {
        public TextRenderInfoBrick(TextRenderInfo info, bool replaceZeros, double maxY)
        {
            var baseRectange = info.GetBaseline().GetBoundingRectange();
            var accentRectange = info.GetAscentLine().GetBoundingRectange();

            var text = info.PdfString.ToString();
            if (replaceZeros)
            {
                text = text.Replace("\0", "");
            }

            Text = text;
            X = baseRectange.X;
            Y = maxY - baseRectange.Y;
            Width = baseRectange.Width;
            Height = accentRectange.Y - baseRectange.Y;
        }

        public string Text { get; }
        public double X { get; }
        public double Y { get; }
        public double Width { get; }
        public double Height { get; }
    }
}