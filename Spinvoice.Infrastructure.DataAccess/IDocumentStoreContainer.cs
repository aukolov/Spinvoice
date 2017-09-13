using System;
using Raven.Client;

namespace Spinvoice.Infrastructure.DataAccess
{
    public interface IDocumentStoreContainer : IDisposable
    {
        IDocumentStore DocumentStore { get; }
    }
}