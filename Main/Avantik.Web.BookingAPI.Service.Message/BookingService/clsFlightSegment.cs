using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    public class FlightSegment
    {
        public Guid booking_segment_id { get; set; }
        public Guid flight_connection_id { get; set; }
        public Guid flight_id { get; set; }

        public DateTime departure_date { get; set; }
        public int departure_time { get; set; }
        public int arrival_time { get; set; }
        public string airline_rcd { get; set; }
        public string flight_number { get; set; }
        public string origin_rcd { get; set; }
        public string destination_rcd { get; set; }
        public string od_origin_rcd { get; set; }
        public string od_destination_rcd { get; set; }
        public string booking_class_rcd { get; set; }
        public string boarding_class_rcd { get; set; }
        public string segment_status_rcd { get; set; }

        //Use for passenger count in the flight.
        public int number_of_units { get; set; }
    }
}
