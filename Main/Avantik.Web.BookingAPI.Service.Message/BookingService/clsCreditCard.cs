using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    public class CreditCard
    {
        public string credit_card_number { get; set; }
        public string name_on_card { get; set; }
        public string cvv_code { get; set; }
        public string issue_number { get; set; }

        public int expiry_month { get; set; }
        public int expiry_year { get; set; }
        public int issue_month { get; set; }
        public int issue_year { get; set; }

        public Address Address { get; set; }
    }
}
