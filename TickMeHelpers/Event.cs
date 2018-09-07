using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TickMeHelpers
{
    [Serializable]
    public class Event
    {
        [Key]
        [Display(Name = "Id")]
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; } = Guid.Empty;

        [Required]
        [Display(Name = "Title")]
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; } = default(string);

        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy hh:mm tt}")]
        [JsonProperty(PropertyName = "startdate")]
        public DateTime StartMoment { get; set; } = default(DateTime);

        [Required]
        [Display(Name = "Duration")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:%h\\:mm}")]
        [JsonProperty(PropertyName = "duration")]
        public TimeSpan Duration { get; set; } = TimeSpan.FromHours(1);

        [Required]
        [Display(Name = "Venue")]
        [JsonProperty(PropertyName = "venue")]
        public string Venue { get; set; } = default(string);

        [Required]
        [Display(Name = "Price")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "£{0:N2}")]
        [JsonProperty(PropertyName = "price")]
        public decimal Price { get; set; } = -1;

        [Required]
        [Display(Name = "Venue Size")]
        [JsonProperty(PropertyName = "totalavailabletickets")]
        public int TotalAvailableTickets { get; set; } = 0;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
