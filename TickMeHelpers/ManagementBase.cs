using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TickMeHelpers
{
    public class ManagementBase<T>
    {
        protected readonly string _endpointUri = "";
        protected readonly string _primaryKey = "";
        protected readonly string _databaseName = "";
        protected readonly string _collectionName = "";
        protected readonly IConfiguration _configuration = null;

        protected Uri CollectionLink { get; } = null;

        protected async Task<DocumentClient> GetClient()
        {
            return _client ?? (_client = await CosmosHelper.GetClient(_endpointUri, _primaryKey, _databaseName, _collectionName));
        }
        private DocumentClient _client = null;

        public ManagementBase(IConfiguration configuration, string collectionName)
        {
            _configuration = configuration;
            _endpointUri = configuration["DataStore-EndpointUri"];
            _primaryKey = configuration["DataStore-PrimaryKey"];
            _databaseName = configuration["DataStore-DatabaseName"];
            _collectionName = collectionName;
            CollectionLink = UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName);
        }

        public async Task Upsert(T evnt)
        {
            var cosmos = await GetClient();
            await cosmos.UpsertDocumentAsync(CollectionLink, evnt);
        }

        public async Task Create(T evnt)
        {
            var cosmos = await GetClient();
            await cosmos.CreateDocumentAsync(CollectionLink, evnt);
        }

        public async Task Update(T evnt)
        {
            var cosmos = await GetClient();
            await cosmos.UpsertDocumentAsync(CollectionLink, evnt);
        }

        public async Task<T> Get(Guid id)
        {
            var cosmos = await GetClient();
            var result = await cosmos.ReadDocumentAsync<T>(UriFactory.CreateDocumentUri(_databaseName, _collectionName, id.ToString()));
            return result;
        }

        public async Task<List<T>> List()
        {
            var result = new List<T>();
            var cosmos = await GetClient();
            IQueryable<T> eventQuerySql = cosmos.CreateDocumentQuery<T>(CollectionLink);
            result = eventQuerySql.ToList();
            return result;
        }

        public async Task Delete(Guid id)
        {
            var cosmos = await GetClient();
            await cosmos.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseName, _collectionName, id.ToString()));
        }
    }
}
