using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TickMeHelpers
{
    public class TicketManagement : ManagementBase<Ticket>
    {
        public TicketManagement(IConfiguration config):base(config,"ticket")
        {

        }

        public async Task<Ticket> IssueEventTicket(Event evnt, User user, string paymentData)
        {
            return await IssueEventTicket(evnt.Id, user.Id, evnt.Price, paymentData);
        }

        public async Task<Ticket> IssueEventTicket(Guid eventId, Guid userId, decimal pricePaid, string paymentData)
        {
            var eventManager = new EventManagement(_configuration);
            var evnt = await eventManager.Get(eventId);
            if(evnt == null || evnt.TotalAvailableTickets < 1)
            {
                return null;
            }
                        
            var cosmos = await GetClient();
            var lastSerialList = cosmos.CreateDocumentQuery<Ticket>(CollectionLink).
                                    Where(e => eventId == e.EventId).
                                    OrderByDescending(e => e.SerialNumber).ToList();
            var lastSerial = 0;
            if (lastSerialList.Count > 0)
            {
                lastSerial = lastSerialList.FirstOrDefault().SerialNumber;
            }
            evnt.TotalAvailableTickets = evnt.TotalAvailableTickets - 1;
            await eventManager.Update(evnt);
            var result = new Ticket()
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                UserId = userId,
                PricePaid = pricePaid,
                SerialNumber = lastSerial + 1,
                PaymentData = paymentData
            };
            await Upsert(result);
            return result;
        }

        public async Task<IEnumerable<Ticket>> GetUserTickets(Guid userId)
        {
            var cosmos = await GetClient();
            IQueryable<Ticket> query = cosmos.CreateDocumentQuery<Ticket>(CollectionLink).Where(e => userId == e.UserId);
            var res = query.ToList();
            return res;
        }
    }
}
