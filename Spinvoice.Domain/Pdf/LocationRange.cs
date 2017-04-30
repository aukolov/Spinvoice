namespace Spinvoice.Domain.Pdf
{
    public class LocationRange
    {
        public LocationRange(Location start, Location end)
        {
            Start = start;
            End = end;
        }

        public Location Start { get; private set; }
        public Location End { get; private set; }
    }
}