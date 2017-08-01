using Spinvoice.Domain.Accounting;

namespace Spinvoice.Infrastructure.DataAccess
{
    public class AccountsChartDataAccess : BaseDataAccess<AccountsChart>, IAccountsChartDataAccess
    {
        public AccountsChartDataAccess(IDocumentStoreContainer documentStoreContainer) 
            : base(documentStoreContainer)
        {
        }
    }
}