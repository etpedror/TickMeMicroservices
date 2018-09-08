using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TickMeHelpers;

namespace TickMePayments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        // POST api/values
        [HttpPost]
        public HttpResponseMessage Post([FromBody] string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                var responseError = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Bad Request")
                };
                return responseError;
            }
            var paymentData = JsonConvert.DeserializeObject<PaymentData>(value);
            paymentData = PaymentManagement.ProcessPayment(paymentData);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(paymentData.ToString())
            };
            return response;
        }
    }
}