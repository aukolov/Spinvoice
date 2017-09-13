using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NLog;
using Raven.Abstractions.Data;
using Raven.Client;
using Spinvoice.Domain.Company;

namespace Spinvoice.Infrastructure.DataAccess
{
    public abstract class BaseDataAccess<T> : IBaseDataAccess<T>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected readonly IDocumentStore DocumentStore;

        protected BaseDataAccess(IDocumentStoreContainer documentStoreContainer)
        {
            DocumentStore = documentStoreContainer.DocumentStore;
        }

        public T[] GetAll()
        {
            _logger.Info("Start getting all documents.");
            T[] entities;
            using (var session = DocumentStore.OpenSession())
            {
                entities = session
                    .Query<T>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfNow(TimeSpan.FromMinutes(2)))
                    .Take(1024)
                    .ToArray();
            }
            _logger.Info("Got {0} documents.", entities.Length);
            return entities;
        }

        public T Get(string id)
        {
            _logger.Info("Start getting document by id {0}.", id);
            T entity;
            using (var session = DocumentStore.OpenSession())
            {
                entity = session.Load<T>(id);
            }
            _logger.Info("Document found: {0}.", id != null);
            return entity;
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
            var indexName = $"Auto/{Raven.Client.Util.Inflector.Pluralize(typeof(T).Name)}";

            _logger.Info("Waiting for stale indexes.");
            WaitForStaleIndexes();
            _logger.Info("Start deliting all items.");
            if (DocumentStore.DatabaseCommands.GetIndexes(0, 128).All(i => i.Name != indexName))
            {
                return;
            }
            DocumentStore.DatabaseCommands.DeleteByIndex(indexName, new IndexQuery()).WaitForCompletion();
            _logger.Info("Waiting for stale indexes.");
            WaitForStaleIndexes();
            _logger.Info("All items deleted.");
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