using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using NServiceBus.Timeout.Core;

namespace NServiceBus.DocumentDB.Timeouts
{
    public class TimeoutPersister : IPersistTimeouts
    {
        public const string CollectionName = "Timeouts";

        public DocumentClient DocumentClient { get; set; }

        public string TimeoutsCollectionLink { get; set; }

        public IEnumerable<Tuple<string, DateTime>> GetNextChunk(DateTime startSlice, out DateTime nextTimeToRunQuery)
        {
            // TODO DateTime queries aren't supported by DocumentDB. See http://social.msdn.microsoft.com/Forums/en-US/ed6de210-5a8f-450f-8c14-2e55dcf8f774/datetime-comparison-makes-an-exception-error-in-linq?forum=AzureDocumentDB
            // There are a couple of ways to bypass / implement this, currently leaving this broken
            // until ADB goes out of Preview

            var now = DateTime.UtcNow;
            var timeouts = DocumentClient.CreateDocumentQuery<Timeout>(TimeoutsCollectionLink)
                .Where(x => x.Time > startSlice && x.Time <= now)
                .Select(x => new Tuple<string, DateTime>(x.Id, x.Time))
                .ToArray();

            nextTimeToRunQuery = DocumentClient.CreateDocumentQuery<Timeout>(TimeoutsCollectionLink)
                .Where(x => x.Time >= now)
                .Select(x => x.Time)
                .FirstOrDefault();

            return timeouts;
        }

        public void Add(TimeoutData timeout)
        {
            DocumentClient.CreateDocumentAsync(TimeoutsCollectionLink, new Timeout(timeout)).Wait();
        }

        public bool TryRemove(string timeoutId, out TimeoutData timeoutData)
        {
            var timeout = DocumentClient.CreateDocumentQuery<Timeout>(TimeoutsCollectionLink)
                .Where(x => x.Id == timeoutId)
                .AsEnumerable()
                .FirstOrDefault();

            if (timeout != null)
            {
                timeoutData = new TimeoutData
                {
                    Destination = timeout.Destination,
                    Headers = timeout.Headers,
                    Id = timeoutId,
                    OwningTimeoutManager = timeout.OwningTimeoutManager,
                    SagaId = timeout.SagaId,
                    State = timeout.State,
                    Time = timeout.Time,
                };
                DocumentClient.DeleteDocumentAsync(timeout.SelfLink).Wait();
                return true;
            }

            timeoutData = null;
            return false;
        }

        public async void RemoveTimeoutBy(Guid sagaId)
        {
            // DocumentDB doesn't currently support set-based operations, so we have to do this manually
            foreach (var timeout in DocumentClient.CreateDocumentQuery<Timeout>(TimeoutsCollectionLink)
                .Where(x => x.SagaId == sagaId))
            {
                await DocumentClient.DeleteDocumentAsync(timeout.SelfLink);
            }
        }
    }
}
