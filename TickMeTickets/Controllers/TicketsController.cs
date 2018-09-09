using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TickMeHelpers;
using TickMeHelpers.ApiModels;

namespace TickMeTickets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private TicketManagement TicketManager
        {
            get
            {
                if(_ticketManager == null)
                {
                    _ticketManager = new TicketManagement(_configuration);
                }
                return _ticketManager;
            }
        }
        private TicketManagement _ticketManager = null;

        private IConfiguration _configuration;
        public TicketsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET api/tickets
        [HttpGet]
        public string Get()
        {
            //var response = new HttpResponseMessage(HttpStatusCode.OK)
            //{
            //    Content = new StringContent("OK")
            //};
            //return response;
            return "OK";
        }
        // POST api/tickets
        [HttpPost]
        public async Task<string> Post([FromBody] string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Bad Request 1";
            }
            var buyModel = JsonConvert.DeserializeObject<TicketBuyModel>(value);
            //var buyModel = value;
            if(buyModel == null|| buyModel.EventId == Guid.Empty || buyModel.UserId == Guid.Empty || string.IsNullOrWhiteSpace(buyModel.PaymentData))
            {
                return "Bad Request 2";
            }
            var paymentData = JsonConvert.DeserializeObject<PaymentData>(buyModel.PaymentData);
            if(paymentData == null)
            {
                return "Bad Request 3";
            }
            try
            {
                var manager = TicketManager;
                var ticket = await manager.IssueEventTicket(buyModel.EventId, buyModel.UserId, paymentData.Value, paymentData.TransactionData);
                return ticket.ToString();
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}