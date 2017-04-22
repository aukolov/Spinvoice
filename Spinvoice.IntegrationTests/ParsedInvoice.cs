using System;

namespace Spinvoice.IntegrationTests
{
    public class ParsedInvoice
    {
        public string CompanyName { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? Date { get; set; }
        public decimal? NetAmount { get; set; }

    }
}