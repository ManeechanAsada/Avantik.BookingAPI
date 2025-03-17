using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    public class Mapping
    {
        public Guid passenger_id { get; set; }
        public Guid booking_segment_id { get; set; }

        public DateTime not_valid_before_date { get; set; }
        public DateTime not_valid_after_date { get; set; }

        public Int16 refund_with_charge_hours { get; set; }
        public Int16 refund_not_possible_hours { get; set; }

        public short piece_allowance { get; set; }

        public string fare_code { get; set; }   
        public string seat_number { get; set; }
        public string currency_rcd { get; set; }
        public string endorsement_text { get; set; }
        public string restriction_text { get; set; }

        public decimal fare_amount { get; set; }
        public decimal fare_amount_incl { get; set; }
        public decimal fare_vat { get; set; }
        public decimal net_total { get; set; }
        //Optinal
        public decimal commission_amount { get; set; }
        //Optinal
        public decimal commission_amount_incl { get; set; }
        //Optinal
        public decimal commission_percentage { get; set; }
        //Optinal
        public decimal public_fare_amount { get; set; }
        //Optinal
        public decimal public_fare_amount_incl { get; set; }
        public decimal refund_charge { get; set; }
        public decimal baggage_weight { get; set; }
        public double redemption_points { get; set; }

        public byte e_ticket_flag { get; set; }
        public byte refundable_flag { get; set; }
        public byte transferable_fare_flag { get; set; }
        public byte through_fare_flag { get; set; }
        public byte it_fare_flag { get; set; }
        public byte duty_travel_flag { get; set; }
        public byte standby_flag { get; set; }
        public byte exclude_pricing_flag { get; set; }
    }
}
