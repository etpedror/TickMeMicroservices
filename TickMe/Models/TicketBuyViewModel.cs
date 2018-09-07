using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickMeHelpers;

namespace TickMe.Models
{
    public class TicketBuyViewModel
    {
        public Event evnt;
        public User user;
        public Ticket ticket;
        public PaymentData paymentData;
    }
}
