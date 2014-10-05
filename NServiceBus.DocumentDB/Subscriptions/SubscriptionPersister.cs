using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using NServiceBus.Unicast.Subscriptions;
using NServiceBus.Unicast.Subscriptions.MessageDrivenSubscriptions;

namespace NServiceBus.DocumentDB.Subscriptions
{
    public class SubscriptionPersister : ISubscriptionStorage
    {
        public const string CollectionName = "Subscriptions";

        public DocumentClient DocumentClient { get; set; }

        public string SubscriptionsCollectionLink { get; set; }

        public void Subscribe(Address client, IEnumerable<MessageType> messageTypes)
        {
            bool found = true, updated = false;            
            var subscription = DocumentClient.CreateDocumentQuery<Subscription>(SubscriptionsCollectionLink)
                .Where(x => x.SubscriberAddress == client.ToString())
                .AsEnumerable()
                .FirstOrDefault();

            if (subscription == null)
            {
                found = false;
                subscription = new Subscription
                {
                    Subscriber = client,
                    SubscriberAddress = client.ToString(),
                };
            }

            foreach (var messageType in messageTypes)
            {
                updated = updated || subscription.MessageTypes.Add(Subscription.HashMessageType(messageType));
            }

            // TODO ensure optimistic concurrency
            if (found)
            {
                if (updated) // Avoid posting an update when nothing changed
                    DocumentClient.ReplaceDocumentAsync(subscription.SelfLink, subscription).Wait();
            }
            else
            {
                DocumentClient.CreateDocumentAsync(SubscriptionsCollectionLink, subscription).Wait();
            }
        }

        public void Unsubscribe(Address client, IEnumerable<MessageType> messageTypes)
        {
            var subscription = DocumentClient.CreateDocumentQuery<Subscription>(SubscriptionsCollectionLink)
                .Where(x => x.SubscriberAddress == client.ToString())
                .AsEnumerable()
                .FirstOrDefault();

            if (subscription == null)
                return;

            foreach (var messageType in messageTypes)
            {
                subscription.MessageTypes.Remove(Subscription.HashMessageType(messageType));
            }

            DocumentClient.ReplaceDocumentAsync(subscription.SelfLink, subscription).Wait(); // TODO ensure optimistic concurrency
        }

        public IEnumerable<Address> GetSubscriberAddressesForMessage(IEnumerable<MessageType> messageTypes)
        {
            if (!messageTypes.Any())
                return Enumerable.Empty<Address>();

            var query = new StringBuilder("SELECT VALUE s.Subscriber " + // Project only the subscriber address
                                          "FROM " + CollectionName + " s " + // Specifying the collection to pick from
                                          "JOIN t IN s.MessageTypes" + // Flattening the structure so we can query on MessageTypes
                                          " WHERE "); // Start building the filtering
            foreach (var messageType in messageTypes)
            {
                query.Append("t=\"");
                query.Append(Subscription.HashMessageType(messageType));
                query.Append("\" OR ");
            }
            query.Length -= 4;

            return DocumentClient
                .CreateDocumentQuery<Address>(SubscriptionsCollectionLink, query.ToString())
                .ToArray();
        }

        public void Init()
        {
        }
    }
}
