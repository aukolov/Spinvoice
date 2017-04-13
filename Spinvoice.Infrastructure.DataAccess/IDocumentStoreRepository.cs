using Raven.Client;

namespace Spinvoice.Infrastructure.DataAccess
{
    public interface IDocumentStoreRepository
    {
        IDocumentStore DocumentStore { get; }
    }
}