using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TickMeHelpers.ApiModels
{
    [Serializable]
    public class TicketBuyModel
    {
        [JsonProperty(PropertyName = "eventid")]
        public Guid EventId { get; set; } = Guid.Empty;

        [JsonProperty(PropertyName = "userid")]
        public Guid UserId { get; set; } = Guid.Empty;

        [JsonProperty(PropertyName = "paymentdata")]
        public string PaymentData { get; set; } = null;
    }
}
