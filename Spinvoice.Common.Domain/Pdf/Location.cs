namespace Spinvoice.Common.Domain.Pdf
{
    public class Location
    {
        public Location(int page, int block, int sentence)
        {
            Page = page;
            Block = block;
            Sentence = sentence;
        }

        public int Page { get; private set; }
        public int Block { get; private set; }
        public int Sentence { get; private set; }
    }
}