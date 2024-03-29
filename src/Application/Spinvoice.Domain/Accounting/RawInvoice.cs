﻿namespace Spinvoice.Domain.Accounting
{
    public class RawInvoice
    {
        public string CompanyName { get; set; }
        public string InvoiceNumber { get; set; }
        public string Date { get; set; }
        public string NetAmount { get; set; }
        public RawPosition FirstPosition { get; set; } = new RawPosition();
    }
}