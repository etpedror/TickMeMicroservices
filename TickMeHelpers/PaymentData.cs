using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TickMeHelpers
{
    [Serializable]
    public class PaymentData
    {
        [Required]
        [Display(Name = "Card Number")]
        [CreditCard]
        [JsonProperty(PropertyName = "number")]
        public string CardNumber { get; set; }

        [Required]
        [Display(Name = "Name on Card")]
        [JsonProperty(PropertyName = "name")]
        public string NameOnCard { get; set; }

        [Required]
        [Display(Name = "Security Code")]
        [StringLength(3, MinimumLength = 3)]
        [JsonProperty(PropertyName = "securitycode")]
        public string SecurityCode { get; set; }

        [Required]
        [Display(Name = "Month")]
        [Range(1, 12, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [JsonProperty(PropertyName = "cardmonth")]
        public int ValidMonth { get; set; } = 1;

        [Required]
        [Display(Name = "Year")]
        [Range(18, 25, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [JsonProperty(PropertyName = "cardyear")]
        public int ValidYear { get; set; } = 2018;

        [Required]
        [Display(Name = "Value")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "£{0:N2}")]
        [JsonProperty(PropertyName = "value")]
        public decimal Value { get; set; } = -1;

        [Display(Name = "Transaction Data")]
        [JsonProperty(PropertyName = "transaction")]
        public string TransactionData { get; set; }

        [Display(Name = "Transaction Successful")]
        [JsonProperty(PropertyName = "success")]
        public bool TransactionSuccessful { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
