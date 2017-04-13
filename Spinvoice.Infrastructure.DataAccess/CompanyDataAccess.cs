using Spinvoice.Domain.Company;

namespace Spinvoice.Infrastructure.DataAccess
{
    public class CompanyDataAccess : BaseDataAccess<Company>, ICompanyDataAccess
    {
        public CompanyDataAccess(IDocumentStoreRepository documentStoreRepository) 
            : base(documentStoreRepository)
        {
        }
    }
}
