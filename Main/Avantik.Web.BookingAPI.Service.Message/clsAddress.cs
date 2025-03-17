using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avantik.Web.BookingAPI.Service.Message
{
    public class Address
    {
        public string address_line1 { get; set; }
        public string address_line2 { get; set; }
        public string street { get; set; }
        public string po_box { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string district { get; set; }
        public string province { get; set; }
        public string zip_code { get; set; }
        public string country_rcd { get; set; }
    }
}
