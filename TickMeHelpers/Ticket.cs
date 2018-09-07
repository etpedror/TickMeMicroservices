using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TickMeHelpers
{
    [Serializable]
    public class Ticket
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty(PropertyName = "eventid")]
        public Guid EventId { get; set; } = Guid.Empty;

        [JsonProperty(PropertyName = "userid")]
        public Guid UserId { get; set; } = Guid.Empty;

        [JsonProperty(PropertyName = "serialnumber")]
        public int SerialNumber { get; set; } = -1;

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "£{0:N2}")]
        [JsonProperty(PropertyName = "pricepaid")]
        public decimal PricePaid { get; set; } = -1;

        [JsonProperty(PropertyName = "paymentdata")]
        public string PaymentData { get; set; } = String.Empty;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
