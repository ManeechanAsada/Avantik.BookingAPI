using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    public class Tax
    {
        public Guid passenger_id { get; set; }
        public Guid booking_segment_id { get; set; }
        
        public string tax_rcd { get; set; }
        public string tax_currency_rcd { get; set; }
        public string sales_currency_rcd { get; set; }

        public decimal sales_amount { get; set; }
        public decimal sales_amount_incl { get; set; }
        public decimal tax_amount { get; set; }
        public decimal tax_amount_incl { get; set; }
    }
}
