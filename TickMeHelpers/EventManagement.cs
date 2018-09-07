using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TickMeHelpers
{
    public class EventManagement : ManagementBase<Event>
    {
        public EventManagement(IConfiguration configuration):base(configuration, "events")
        {
        }
        
        public async Task<List<Event>> GetFilteredListOfEvents(Event filter)
        {
            var result = new List<Event>();
            var cosmos = await GetClient();
            IQueryable<Event> eventQuerySql = cosmos.CreateDocumentQuery<Event>(CollectionLink)
                .Where(e =>
                   (filter.Id == default(Guid) || e.Id == filter.Id) &&
                   (filter.Title == default(string) || e.Title == filter.Title) &&
                   (filter.Price == -1 || e.Price <= filter.Price) &&
                   (filter.Venue == default(string) || e.Venue == filter.Venue)
                );
            result = eventQuerySql.ToList();
            return result;
        }


    }
}
