using Raven.Client;
using Raven.Client.Embedded;

namespace Spinvoice.Infrastructure.DataAccess
{
    public class DocumentStoreRepository : IDocumentStoreRepository
    {
        private EmbeddableDocumentStore _documentStore;
        private readonly string _dataDirectory;

        public DocumentStoreRepository(string dataDirectory)
        {
            _dataDirectory = dataDirectory;
        }

        public IDocumentStore DocumentStore
        {
            get
            {
                if (_documentStore == null)
                {
                    _documentStore = new EmbeddableDocumentStore
                    {
                        DataDirectory = _dataDirectory,
                        UseEmbeddedHttpServer = false
                    };
                    _documentStore.Initialize();
                }
                return _documentStore;
            }
        }
    }
}