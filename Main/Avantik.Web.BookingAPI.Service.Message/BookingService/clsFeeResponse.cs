using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    public class FeeResponse : Fee
    {
        public string error_code { get; set; }
        public string error_message { get; set; }
        public string display_name { get; set; }
    }
}
