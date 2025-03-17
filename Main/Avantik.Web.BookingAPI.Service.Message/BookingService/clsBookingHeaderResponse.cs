using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    public class BookingHeaderResponse : BookingHeader
    {
        public string error_code { get; set; }
        public string error_message { get; set; }

        //Response property
        public Guid booking_id { get; set; }
        public string record_locator { get; set; }
    }
}
