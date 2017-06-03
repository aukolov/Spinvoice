using System;
using System.Text;
using Spinvoice.Domain.Pdf;

namespace Spinvoice.Infrastructure.Pdf
{
    public class SentenceModelBuilder
    {
        private readonly StringBuilder _sb = new StringBuilder();

        public SentenceModelBuilder()
        {
            IsEmpty = true;
        }

        public double Left { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }

        public bool IsEmpty { get; private set; }

        public void Append(string text, double x, double y, double width, double height)
        {
            _sb.Append(text);
            if (IsEmpty)
            {
                Left = x;
                Top = y - height;
                Right = x + width;
                Bottom = y;

                IsEmpty = false;
            }
            else
            {
                Left = Math.Min(Left, x);
                Top = Math.Min(Top, y - height);
                Right = Math.Max(Right, x + width);
                Bottom = Math.Max(Bottom, y);
            }
        }

        public SentenceModel Build()
        {
            return new SentenceModel(_sb.ToString(), Left, Top, Right - Left, Bottom - Top);
        }
    }
}