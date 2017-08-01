using Raven.Client;
using Raven.Client.Embedded;

namespace Spinvoice.Infrastructure.DataAccess
{
    public class DocumentStoreContainer : IDocumentStoreContainer
    {
        private EmbeddableDocumentStore _documentStore;
        private readonly string _dataDirectory;

        public DocumentStoreContainer(IDataDirectoryProvider dataDirectoryProvider)
        {
            _dataDirectory = dataDirectoryProvider.Path;
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