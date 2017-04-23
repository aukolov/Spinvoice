using System;
using System.IO;
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
                        DataDirectory = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                            "Spinvoice",
                            "data"),
                        UseEmbeddedHttpServer = false
                    };
                    _documentStore.Initialize();
                }
                return _documentStore;
            }
        }
    }
}