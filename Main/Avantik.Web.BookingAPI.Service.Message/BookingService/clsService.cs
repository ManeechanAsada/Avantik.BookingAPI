using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    public class Service
    {
        public Guid passenger_segment_service_id { get; set; }
        public Guid passenger_id { get; set; }
        public Guid booking_segment_id { get; set; }

        public short number_of_units { get; set; }

        public string special_service_rcd { get; set; }
        public string service_text { get; set; }
        //The value should be SS or NN
        public string special_service_status_rcd { get; set; }
        public string unit_rcd { get; set; }
        
        
    }
}
