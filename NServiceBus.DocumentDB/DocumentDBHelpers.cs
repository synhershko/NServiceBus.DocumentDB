using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace NServiceBus.DocumentDB
{
    public static class DocumentDBHelpers
    {
        public static async Task<Database> EnsureDatabaseExists(this DocumentClient client, string dbName)
        {
            var database = client.CreateDatabaseQuery()
                .Where(db => db.Id == dbName)
                .AsEnumerable()
                .FirstOrDefault();

            return database ?? await client.CreateDatabaseAsync(new Database { Id = dbName });
        }

        public static async Task<DocumentCollection> EnsureCollectionExists(this DocumentClient client, string databaseLink, string collectionId)
        {
            var collection = client.CreateDocumentCollectionQuery(databaseLink)
                .Where(c => c.Id == collectionId)
                .AsEnumerable()
                .FirstOrDefault();

            return collection ?? await client.CreateDocumentCollectionAsync(databaseLink, new DocumentCollection { Id = collectionId });
        }
    }
}