namespace Spinvoice.Domain.Accounting
{
    public class AccountsChart
    {
        public string Id { get; set; }
        public string AssetExternalAccountId { get; set; }
        public string ExpenseExternalAccountId { get; set; }
        public string IncomeExternalAccountId { get; set; }
        public string VatAccountId { get; set; }
        public string TransportationCostsAccountId { get; set; }

        public bool IsComplete => !string.IsNullOrEmpty(AssetExternalAccountId)
                                  && !string.IsNullOrEmpty(ExpenseExternalAccountId)
                                  && !string.IsNullOrEmpty(IncomeExternalAccountId)
                                  && !string.IsNullOrEmpty(VatAccountId)
                                  && !string.IsNullOrEmpty(TransportationCostsAccountId);
    }
}