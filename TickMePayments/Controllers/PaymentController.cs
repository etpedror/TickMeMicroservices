using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using TickMeHelpers;

namespace TickMePayments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        // GET api/payment
        [HttpGet]
        public string Get()
        {
            Response.StatusCode = (int)HttpStatusCode.OK;
            return "{'status':'Service Up'}";
        }

        // POST api/payment
        [HttpPost]
        public string Post([FromBody] string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return "{'error':'Bad Request'}";
            }
            var payData = JsonConvert.DeserializeObject<PaymentData>(value);
            if (payData == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return "{'error':'Bad Request'}";
            }           
            
            try
            {
                payData = PaymentManagement.ProcessPayment(payData);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return payData.ToString();
            }
            catch
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return "{'error':'Server couldn't process payment'}";
            }
        }
    }
}