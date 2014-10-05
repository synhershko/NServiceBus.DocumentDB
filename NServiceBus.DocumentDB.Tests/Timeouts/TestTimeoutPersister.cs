using System;
using NServiceBus.DocumentDB.Timeouts;
using NServiceBus.Timeout.Core;
using NUnit.Framework;

namespace NServiceBus.DocumentDB.Tests.Timeouts
{
    public class TestTimeoutPersister : TestAgainstDocumentDB
    {
        [Test, Ignore("DateTime querying isn't implemented properly yet; check comments in TimeoutPersister")]
        public async void Can_get_timeout_chunks()
        {
            var collection = await client.EnsureCollectionExists(database.SelfLink, TimeoutPersister.CollectionName);
            var persister = new TimeoutPersister
            {
                DocumentClient = client,
                TimeoutsCollectionLink = collection.SelfLink,
            };

            var sagaId = Guid.NewGuid();
            persister.Add(new TimeoutData
            {
                OwningTimeoutManager = string.Empty,
                Time = DateTime.UtcNow.AddMinutes(-120),
                SagaId = sagaId
            });

            DateTime next;
            var chukn = persister.GetNextChunk(DateTime.UtcNow.AddYears(-10), out next);
        }
    }
}
