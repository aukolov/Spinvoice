using System.Linq;
using System.Threading;
using Raven.Client;
using Raven.Client.Embedded;
using Spinvoice.Domain.Company;

namespace Spinvoice.Infrastructure.DataAccess
{
    public class CompanyDataAccess : ICompanyDataAccess
    {
        private readonly IDocumentStore _documentStore;

        public CompanyDataAccess()
        {
            _documentStore = new EmbeddableDocumentStore()
            {
                DataDirectory = "Data",
                UseEmbeddedHttpServer = true
            };
            _documentStore.Initialize();
        }
        public Company[] GetAll()
        {
            using (var session = _documentStore.OpenSession())
            {
                var companies = session
                    .Query<Company>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                    .ToArray();
                return companies;
            }
        }

        public Company Get(string id)
        {
            using (var session = _documentStore.OpenSession())
            {
                var company = session.Load<Company>(id);
                return company;
            }
        }

        public void Add(Company company)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(company);
                session.SaveChanges();
            }
        }

        public void DeleteAll()
        {
            using (var session = _documentStore.OpenSession())
            {
                var companies = session.Query<Company>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                    .ToArray();
                foreach (var company in companies)
                {
                    session.Delete(company);
                }
                session.SaveChanges();
            }
        }

        public void Dispose()
        {
            _documentStore?.Dispose();
        }
    }
}
