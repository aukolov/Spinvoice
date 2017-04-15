namespace Spinvoice.Domain.Company
{
    public class Company
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
        public bool IsEuropeanUnion { get; set; }
        public string VatNumber { get; set; }
    }
}