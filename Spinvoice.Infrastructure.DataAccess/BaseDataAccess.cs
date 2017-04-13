using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Raven.Abstractions.Data;
using Raven.Client;
using Spinvoice.Domain.Company;

namespace Spinvoice.Infrastructure.DataAccess
{
    public abstract class BaseDataAccess<T> : IBaseDataAccess<T>
    {
        protected readonly IDocumentStore DocumentStore;

        protected BaseDataAccess(IDocumentStoreRepository documentStoreRepository)
        {
            DocumentStore = documentStoreRepository.DocumentStore;
        }

        public T[] GetAll()
        {
            using (var session = DocumentStore.OpenSession())
            {
                var entities = session
                    .Query<T>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                    .ToArray();
                return entities;
            }
        }

        public T Get(string id)
        {
            using (var session = DocumentStore.OpenSession())
            {
                var entity = session.Load<T>(id);
                return entity;
            }
        }

        public void AddOrUpdate(T entity)
        {
            using (var session = DocumentStore.OpenSession())
            {
                session.Store(entity);
                session.SaveChanges();
            }
        }

        public void AddOrUpdate(IEnumerable<T> entities)
        {
            using (var bulkInsert = DocumentStore.BulkInsert())
            {
                foreach (var entity in entities)
                {
                    bulkInsert.Store(entity);
                }
            }
        }

        public void DeleteAll()
        {
            var name = Raven.Client.Util.Inflector.Pluralize(typeof(T).Name);
            var indexName = $"Auto/{name}";

            WaitForStaleIndexes();
            if (DocumentStore.DatabaseCommands.GetIndexes(0, 128).All(i => i.Name != indexName))
            {
                return;
            }
            DocumentStore.DatabaseCommands.DeleteByIndex(indexName, new IndexQuery()).WaitForCompletion();
            WaitForStaleIndexes();
        }

        public void Dispose()
        {
            DocumentStore?.Dispose();
        }

        private void WaitForStaleIndexes()
        {
            while (DocumentStore.DatabaseCommands.GetStatistics().StaleIndexes.Length != 0)
            {
                Thread.Sleep(10);
            }
        }
    }
}