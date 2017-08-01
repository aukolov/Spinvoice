using Raven.Client;

namespace Spinvoice.Infrastructure.DataAccess
{
    public interface IDocumentStoreContainer
    {
        IDocumentStore DocumentStore { get; }
    }
}