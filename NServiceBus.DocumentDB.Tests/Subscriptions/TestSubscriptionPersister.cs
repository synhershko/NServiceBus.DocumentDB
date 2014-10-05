using System.Linq;
using NServiceBus.DocumentDB.Subscriptions;
using NServiceBus.Unicast.Subscriptions;
using NUnit.Framework;

namespace NServiceBus.DocumentDB.Tests.Subscriptions
{
    public class TestSubscriptionPersister : TestAgainstDocumentDB
    {
        [Test]
        public async void Can_subscribe_and_unsubscribe()
        {
            var collection = await client.EnsureCollectionExists(database.SelfLink, SubscriptionPersister.CollectionName);
            var persister = new SubscriptionPersister
            {
                DocumentClient = client,
                SubscriptionsCollectionLink = collection.SelfLink,
            };

            var address = new Address("test", "machine");
            persister.Subscribe(address, new[] { new MessageType(typeof(MockMessage)), });

            var subscribers = persister.GetSubscriberAddressesForMessage(new[] { new MessageType(typeof(MockMessage)), })
                .ToArray();
            CollectionAssert.Contains(subscribers, address);

            persister.Unsubscribe(address, new[] { new MessageType(typeof(MockMessage)), });
            subscribers = persister.GetSubscriberAddressesForMessage(new[] { new MessageType(typeof(MockMessage)), })
                .ToArray();
            CollectionAssert.DoesNotContain(subscribers, address);
        }
    }
}
