using Spinvoice.Domain.Pdf;

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

        public IPdfAnalysisStrategy CompanyInvoiceStrategy { get; set; }
        public IPdfAnalysisStrategy InvoiceNumberStrategy { get; set; }
    }
}