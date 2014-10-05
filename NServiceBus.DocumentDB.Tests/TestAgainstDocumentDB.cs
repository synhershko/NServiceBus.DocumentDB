using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;

namespace NServiceBus.DocumentDB.Tests
{
    public abstract class TestAgainstDocumentDB
    {
        protected readonly ConnectionParameters ConnectionParameters;

        protected Database database;
        protected DocumentClient client;

        protected TestAgainstDocumentDB()
        {
            ConnectionParameters = new ConnectionParameters
            {
                Url = "https://test.documents.azure.com:443/",
                AuthKey = "~ your auth key here ~",
                DatabaseName = "test"//"-" + Guid.NewGuid(),
            };
        }

        [SetUp]
        public void Setup()
        {
            var connectionPolicy = new ConnectionPolicy
            {
                ConnectionMode = ConnectionMode.Gateway,
                ConnectionProtocol = Protocol.Https,
            };

            client = new DocumentClient(new Uri(ConnectionParameters.Url), ConnectionParameters.AuthKey, connectionPolicy);

            PrepareDatabase().Wait();
        }

        private async Task PrepareDatabase()
        {
            database = await client.EnsureDatabaseExists(ConnectionParameters.DatabaseName);
        }

        [TearDown]
        public async void TearDown()
        {
            await client.DeleteDatabaseAsync(database.SelfLink);
            client.Dispose();
        }
    }
}
