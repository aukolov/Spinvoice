using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.Pdf;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.Domain.Company
{
    public class Company
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Side Side { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
        public bool IsEuropeanUnion { get; set; }
        public string VatNumber { get; set; }

        public IStringPdfAnalysisStrategy CompanyInvoiceStrategy { get; set; }
        public IStringPdfAnalysisStrategy InvoiceNumberStrategy { get; set; }
        public IStringPdfAnalysisStrategy InvoiceDateStrategy { get; set; }
        public IStringPdfAnalysisStrategy InvoiceNetAmountStrategy { get; set; }
        public IPdfPositionAnalysisStrategy PositionStrategy { get; set; }
        public string ExternalId { get; set; }

        public override string ToString()
        {
            return $@"Company {Name}";
        }
    }
}