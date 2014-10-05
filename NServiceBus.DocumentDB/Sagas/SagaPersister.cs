using System;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using NServiceBus.Saga;

namespace NServiceBus.DocumentDB.Sagas
{
    class SagaPersister : ISagaPersister
    {
        public const string CollectionName = "Sagas";

        public DocumentClient DocumentClient { get; set; }

        public string SagasCollectionLink { get; set; }

        public void Save(IContainSagaData saga)
        {
            // TODO we probably need to properly pass _id and _etag for OC to kick in; to do that we will need a wrapper
            // TODO class for Sagas inheritting from AbstractDocument
            DocumentClient.CreateDocumentAsync(SagasCollectionLink, saga, null, true).Wait();
        }

        public void Update(IContainSagaData saga)
        {
            // TODO need to properly support unique constraints
            // This can be done either using a method similar to that used with RavenDB with optimistic
            // concurrency and marker documents (although it does need x-document transactions to be supported
            // by ADB), or to use Pre-Triggers to do a constraint lookup and veto writes when needed.
            // Again, since this is just a preview we better wait and do neither.

            throw new NotImplementedException();
        }

        public TSagaData Get<TSagaData>(Guid sagaId) where TSagaData : IContainSagaData
        {
            return DocumentClient.CreateDocumentQuery<TSagaData>(SagasCollectionLink)
                .Where(x => x.Id == sagaId)
                .FirstOrDefault();
        }

        public TSagaData Get<TSagaData>(string propertyName, object propertyValue) where TSagaData : IContainSagaData
        {
            return DocumentClient.CreateDocumentQuery<TSagaData>(SagasCollectionLink,
                "SELECT * FROM " + SagasCollectionLink + " s " +
                // Too many assumptions here... (property name wasn't changed on serialization, it is a string, ...)
                "WHERE s." + propertyName + "=\"" + propertyValue + "\""
                )
                .FirstOrDefault();
        }

        public async void Complete(IContainSagaData saga)
        {
            var toDelete = DocumentClient.CreateDocumentQuery<Document>(SagasCollectionLink)
                .Where(x => x.Id == saga.Id.ToString())
                .FirstOrDefault();
            
            if (toDelete == null)
                return;

            await DocumentClient.DeleteDocumentAsync(toDelete.SelfLink);
            // TODO depending on our implementation for unique constraints, we need to delete them here within the same tx
        }
    }
}
