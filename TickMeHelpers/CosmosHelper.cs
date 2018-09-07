using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TickMeHelpers
{
    public static class CosmosHelper
    {
        public static async Task<DocumentClient> GetClient(string endpointUri, string primaryKey, string databaseName, string collectionName)
        {
            var _client = new DocumentClient(new Uri(endpointUri), primaryKey);
            await _client.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseName });
            await _client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(databaseName), new DocumentCollection { Id = collectionName });
            return _client;
        }
    }
}
