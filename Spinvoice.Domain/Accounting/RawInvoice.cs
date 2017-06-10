namespace Spinvoice.Domain.Accounting
{
    public class RawInvoice
    {
        public string CompanyName { get; set; }
        public string InvoiceNumber { get; set; }
        public string Date { get; set; }
        public string NetAmount { get; set; }

        public RawPosition FirstPosition { get; } = new RawPosition();
    }

    public class RawPosition
    {
        public string Name { get; set; }
        public string Quantity { get; set; }
        public string Amount { get; set; }
    }
}