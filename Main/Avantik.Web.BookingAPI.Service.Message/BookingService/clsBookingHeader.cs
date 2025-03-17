using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    public class BookingHeader
    {
        public Telephone telephone { get; set; }

        public Address address { get; set; }

        public string agency_code { get; set; }
        public string currency_rcd { get; set; }
        public string booking_number { get; set; }
        public string language_rcd { get; set; }
        public string contact_name { get; set; }
        public string contact_email { get; set; }
        public string received_from { get; set; }
        public string phone_search { get; set; }
        public string comment { get; set; }
        public string title_rcd { get; set; }
        public string lastname { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string country_rcd { get; set; }
        public byte group_booking_flag { get; set; }
    }
}
