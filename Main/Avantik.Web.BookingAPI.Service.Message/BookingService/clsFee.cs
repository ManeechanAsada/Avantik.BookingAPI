using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    public class Fee
    {
        public Guid passenger_id { get; set; }
        public Guid booking_segment_id { get; set; }
        public Guid passenger_segment_service_id { get; set; }
        public Guid fee_id { get; set; }

     
        public string fee_rcd { get; set; }
        public string vendor_rcd { get; set; }
        public string od_origin_rcd { get; set; }
        public string od_destination_rcd { get; set; }
        public string currency_rcd { get; set; }
        public string charge_currency_rcd { get; set; }
        public string unit_rcd { get; set; }
        public string mpd_number { get; set; }
        public string comment { get; set; }
        public string external_reference { get; set; }

        public decimal fee_amount { get; set; }
        public decimal fee_amount_incl { get; set; }
        public decimal vat_percentage { get; set; }
        public decimal charge_amount { get; set; }
        public decimal charge_amount_incl { get; set; }
        public decimal weight_lbs { get; set; }
        public decimal weight_kgs { get; set; }
        public decimal number_of_units { get; set; }
        
        
    }
}
