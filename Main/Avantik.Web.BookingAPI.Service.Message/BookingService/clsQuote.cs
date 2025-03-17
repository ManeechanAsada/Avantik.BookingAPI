using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    public class Quote
    {
        public Guid booking_segment_id { get; set; }
        public string passenger_type_rcd { get; set; }
        public string currency_rcd { get; set; }
        public string charge_type { get; set; }
        public string charge_name { get; set; }
        public decimal charge_amount { get; set; }
        public decimal total_amount { get; set; }
        public decimal tax_amount { get; set; }
        public int passenger_count { get; set; }
        public int sort_sequence { get; set; }
        public Guid create_by { get; set; }
        public DateTime create_date_time { get; set; }
        public Guid update_by { get; set; }
        public DateTime update_date_time { get; set; }
        public decimal redemption_points { get; set; }
        public decimal charge_amount_incl { get; set; }

    }
}
