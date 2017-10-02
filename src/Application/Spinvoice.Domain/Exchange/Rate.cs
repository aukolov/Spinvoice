using System;

namespace Spinvoice.Domain.Exchange
{
    public class Rate
    {
        public string Currency { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
    }
}