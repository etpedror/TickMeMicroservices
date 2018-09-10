# TickMe
A sample ASP.NET Core application to sell tickets that serves as a base for a Containers Workshop.
This is the second part of the tutorial.
You can find the first part [here](https://github.com/etpedror/TickMe), and the final one [here](https://github.com/etpedror/TickMeContainers)
## OVERVIEW
TickMe is a ticket selling website that is growing in popularity. While the current monolithic application still works, to cope with higher loads in an efficient manner, it was decided to go for a microservices architecture and to use containers. 

### CONVERTING INTO A MICROSERVICES APPLICATION

The purpose of this part is to divide the application into a web front-end and two microservices, that will take care of the payments and ticket creation.
This allows for the application to offload those resource consuming and slow operations and makes the application more modular. In this sample application, the gains are not immediately evident, and it serves only as an educational piece.
The first step is to create the payment service and the ticket service will follow.

---
#### THE PAYMENT SERVICE

Open Visual Studio 2017 and the application that was downloaded from GiHub previously.
1. Right-click on the solution and select __Add__ > __New Project__.
2. Select ASP.NET Core Web Application and Choose API in the following screen. __DO NOT SELECT DOCKER SUPPORT AT THIS TIME__.
3. Once the Project is created, add a new class to the root of the project, name it _`RawRequestBodyFormatter`_ and past the following code:
```c#
    /// <summary>
    /// Formatter that allows content of type text/plain and application/octet stream
    /// or no content type to be parsed to raw data. Allows for a single input parameter
    /// in the form of:
    /// 
    /// public string RawString([FromBody] string data)
    /// public byte[] RawData([FromBody] byte[] data)
    /// </summary>
    public class RawRequestBodyFormatter : InputFormatter
    {
        public RawRequestBodyFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/octet-stream"));
        }

        /// <summary>
        /// Allow text/plain, application/octet-stream and no content type to
        /// be processed
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Boolean CanRead(InputFormatterContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var contentType = context.HttpContext.Request.ContentType;
            if (string.IsNullOrEmpty(contentType) || contentType == "text/plain" ||
                contentType == "application/octet-stream")
                return true;

            return false;
        }

        /// <summary>
        /// Handle text/plain or no content type for string results
        /// Handle application/octet-stream for byte[] results
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;
            var contentType = context.HttpContext.Request.ContentType;


            if (string.IsNullOrEmpty(contentType) || contentType == "text/plain")
            {
                using (var reader = new StreamReader(request.Body))
                {
                    var content = await reader.ReadToEndAsync();
                    return await InputFormatterResult.SuccessAsync(content);
                }
            }
            if (contentType == "application/octet-stream")
            {
                using (var ms = new MemoryStream(2048))
                {
                    await request.Body.CopyToAsync(ms);
                    var content = ms.ToArray();
                    return await InputFormatterResult.SuccessAsync(content);
                }
            }

            return await InputFormatterResult.FailureAsync();
        }
    }
 ```
This class will allow for the Payment service to receive raw JSON in the requests and decode it properly.
4. On the Startup.cs file, on the ConfigureServices method, add the following:

            services.AddMvc(o => o.InputFormatters.Insert(0, new RawRequestBodyFormatter()));
            
This will tell our middleware how to handle RawRequests.
5. You will also need to edit your _appsettings.json_ file and add the following:
```javascript
"KeyVault": {
    "Vault": "<Name of the Azure Key Vault that was created previously (just the name)>",
    "ClientId": "<The ApplicationID of the Application that was registered previously>",
    "ClientSecret": "<The ClientSecret defined previously>"
  }
```
6. Right-click on dependencies, choose __Manage NuGet Packages__, and fook for and add the following packages: 
    - Newtonsoft.Json and 
    - Microsoft.Extensions.Configuration.AzureKeyVault
7. Right-click on dependencies, choose __Add Reference__, and add a reference to the _TickMeHelpers_ project
8. Open the _program.cs_ file and make sure it looks like this:
```c#
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var builtConfig = config.Build();

                    config.AddAzureKeyVault(
                        $"https://{builtConfig["KeyVault:Vault"]}.vault.azure.net/",
                        builtConfig["KeyVault:ClientId"],
                        builtConfig["KeyVault:ClientSecret"]);
                })
                .UseStartup<Startup>();
    }
```
This will allow our service to use Azure Key Vault as a configuration source.
9. Open the _Controllers_ folder and delete the _ValuesControler_ file.
10.	Right Click on that same folder and choose __Add__. Select __Controller__ and __API Controller – Empty__.
11.	Name it _`PaymentController`_, and make sure that the file is like the following:
```c#
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
```
Your payment service is now created.

#### THE TICKET SERVICE

1. Repeat steps 1-9 of the Payment Service Setup.
2. Right Click on that same folder and choose __Add__. Select __Controller__ and __API Controller – Empty__.
3.	Name it _`TicketsController`_, and make sure that the file is like the following:
```c#
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
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
            Response.StatusCode = (int)HttpStatusCode.OK;
            return "{'status':'Service Up'}";
        }
        // POST api/tickets
        [HttpPost]
        public async Task<string> Post([FromBody] string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return "{'error':'Bad Request'}";
            }
            var buyModel = JsonConvert.DeserializeObject<TicketBuyModel>(value);
            //var buyModel = value;
            if(buyModel == null|| buyModel.EventId == Guid.Empty || buyModel.UserId == Guid.Empty || string.IsNullOrWhiteSpace(buyModel.PaymentData))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return "{'error':'Bad Request'}";
            }
            var paymentData = JsonConvert.DeserializeObject<PaymentData>(buyModel.PaymentData);
            if(paymentData == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return "{'error':'Bad Request'}";
            }
            try
            {
                var manager = TicketManager;
                var ticket = await manager.IssueEventTicket(buyModel.EventId, buyModel.UserId, paymentData.Value, paymentData.TransactionData);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return ticket.ToString();
            }
            catch
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return "{'error':'Server couldn't process ticket'}";
            }
        }
    }
}
```
Your Ticket service is now set up.

#### CHANGES TO THE TICKME WEB APP

The TickMe webapp also needs to be changed to work make rest requests instead of handling the operations itself. To do that follow the next steps.
Add the following values to the _appsettings.json_ file:
```javascript
  "TicketApiUrl": "http://localhost:50082/api/tickets",
  "PaymentApiUrl": http://localhost:50081/api/payment
```
You will also need to change the _EventController_ and add two methods:
```c#
public async Task<Ticket> IssueTicket(TicketBuyModel buyModel)
{
  using (var client = new HttpClient())
  {
    var request = new HttpRequestMessage
    {
      RequestUri = new Uri(Configuration["TicketApiUrl"]),
      Method = HttpMethod.Post
    };
    request.Content = new StringContent(JsonConvert.SerializeObject(buyModel));
    request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

    var result = client.SendAsync(request).Result;
    result.EnsureSuccessStatusCode();
    return JsonConvert.DeserializeObject<Ticket>(await result.Content.ReadAsStringAsync());
  }
}

public async Task<PaymentData> MakePayment(PaymentData paymentData)
{
  using (var client = new HttpClient())
  {
    var request = new HttpRequestMessage
    {
      RequestUri = new Uri(Configuration["PaymentApiUrl"]),
      Method = HttpMethod.Post
    };
    request.Content = new StringContent(JsonConvert.SerializeObject(paymentData));
    request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
 
    var result = client.SendAsync(request).Result;
    result.EnsureSuccessStatusCode();
    return JsonConvert.DeserializeObject<PaymentData>(await result.Content.ReadAsStringAsync());
  }
}
```

Additionaly, change the method _public async Task<ActionResult> Buy(IFormCollection collection)_ so it looks like this:
```c#
// POST: Event/Buy
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<ActionResult> Buy(IFormCollection collection)
{
  try
  {
    var buyModel = await PrepareTicketBuyViewModel(Guid.Parse(collection["evnt.Id"]));
    if (buyModel.evnt == null || buyModel.evnt.TotalAvailableTickets < 1)
    {
      return View("Error");
    }
    var paymentData = new PaymentData()
    {
      CardNumber = collection["paymentData.CardNumber"],
      NameOnCard = collection["paymentData.NameOnCard"],
      ValidMonth = Int32.Parse(collection["paymentData.ValidMonth"]),
      ValidYear = Int32.Parse(collection["paymentData.ValidYear"]),
      SecurityCode = collection["paymentData.SecurityCode"],
      Value = buyModel.evnt.Price
    };
    paymentData = await MakePayment(paymentData);

    buyModel.paymentData = paymentData;
    if (buyModel.user.Id == Guid.Empty)
    {
      buyModel.user = await UserManager.GetUserByAuthId(buyModel.user);
    }
    if (paymentData.TransactionSuccessful)
    {
      var ticketBuyModel = new TicketBuyModel()
      {
        EventId = buyModel.evnt.Id,
        UserId = buyModel.user.Id,
        PaymentData = paymentData.ToString()
      };
      var ticket = await IssueTicket(ticketBuyModel);
      return RedirectToAction("ViewTicket", new { id = ticket.Id });
    }
    return View("Error");
  }
  catch
  {
    return View("Error");
  }
}
```
The changes are now complete, and you have just modularized your application. 
If you were unable to follow all the steps, you can access the desired code at https://github.com/etpedror/TickMeMicroservices.

The next step is to bring containers in, and you can follow that [here](https://github.com/etpedror/TickMeContainers)!
