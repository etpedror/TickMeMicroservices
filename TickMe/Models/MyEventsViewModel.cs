using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TickMeHelpers;

namespace TickMe.Models
{
    public class MyEventsViewModel
    {
        public Ticket Ticket = null;

        public string EventName = default(string);

        [Display(Name = "Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy hh:mm tt}")]
        public DateTime StartMoment { get; set; } = default(DateTime);

        [Display(Name = "Duration")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:%h\\:mm}")]
        public TimeSpan Duration { get; set; } = TimeSpan.FromHours(1);

        public string PaymentDate = default(string);
    }
}
