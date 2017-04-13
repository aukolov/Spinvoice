using Raven.Client;
using Raven.Client.Embedded;

namespace Spinvoice.Infrastructure.DataAccess
{
    public class DocumentStoreRepository : IDocumentStoreRepository
    {
        private EmbeddableDocumentStore _documentStore;

        public IDocumentStore DocumentStore
        {
            get
            {
                if (_documentStore == null)
                {
                    _documentStore = new EmbeddableDocumentStore()
                    {
                        DataDirectory = "Data",
                        UseEmbeddedHttpServer = false
                    };
                    _documentStore.Initialize();
                }
                return _documentStore;
            }
        }
    }
}