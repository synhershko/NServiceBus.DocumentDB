using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.DocumentDB;
using NServiceBus.DocumentDB.Subscriptions;
using NServiceBus.Unicast.Subscriptions;

namespace Playground
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var connParams = new ConnectionParameters
            {
                Url = "https://test.documents.azure.com:443/",
                AuthKey = "~your auth key~",
                DatabaseName = "test"// + Guid.NewGuid(),
            };

            // Default is Direct with TCP, we want to avoid possible issues with running from remote to Azure
            var connectionPolicy = new ConnectionPolicy
            {
                ConnectionMode = ConnectionMode.Gateway,
                ConnectionProtocol = Protocol.Https,
            };

            using (var client = new DocumentClient(new Uri(connParams.Url), connParams.AuthKey))
            {
                Do(client).Wait();
            }

            Console.ReadKey();
        }

        private async static Task DoTest(DocumentClient client)
        {
            var db = await client.EnsureDatabaseExists("test");
            var collection = await client.EnsureCollectionExists(db.SelfLink, "Timeouts2");

            var subscription = new Subscription()
            {

            };
            Document created = await client.CreateDocumentAsync(collection.SelfLink, subscription);
            Console.WriteLine("Created document: " + created);

            var subscriptions = client.CreateDocumentQuery<Subscription>(collection.SelfLink)
                //.Where(x => x.SubscriberAddress == client.ToString())
                .ToArray();

            Console.WriteLine("Retreived document: " + JsonConvert.SerializeObject(subscriptions.FirstOrDefault()));
        }

        private static async Task Do(DocumentClient client)
        {
            var db = await client.EnsureDatabaseExists("test");
            var collection = await client.EnsureCollectionExists(db.SelfLink, SubscriptionPersister.CollectionName);

            var persister = new SubscriptionPersister
            {
                DocumentClient = client,
                SubscriptionsCollectionLink = collection.SelfLink,
            };

            var address = new Address("test", "machine");
            persister.Subscribe(address, new[] {new MessageType(typeof (MockMessage)),});

            var subscribers = persister.GetSubscriberAddressesForMessage(new[] {new MessageType(typeof (MockMessage)),})
                .ToArray();

            //                var query = client.CreateDocumentQuery(collection.SelfLink, "SELECT * FROM Families f WHERE f.id = 'AndersenFamily'");
            //                var family = query.AsEnumerable().Single();
            //                Console.WriteLine("Time to perform simple query {0} ms", s.ElapsedMilliseconds);
            //
            //                s.Restart();
            //                Console.WriteLine("Andersen family:");
            //                Console.WriteLine(((JObject)family).ToString());
            //                Console.WriteLine("Time to convert and print {0} ms", s.ElapsedMilliseconds);
            //
            //                s.Restart();
            //                query = client.CreateDocumentQuery(collection.DocumentsLink, "SELECT * FROM c IN Families.children WHERE c.gender='male'");
            //                var child = query.AsEnumerable().FirstOrDefault();
        }
    }
}
