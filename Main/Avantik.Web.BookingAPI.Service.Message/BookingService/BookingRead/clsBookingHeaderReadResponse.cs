using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    public class BookingHeaderReadResponse : BookingHeader
    {
         public Guid booking_id { get; set; }
         public string record_locator { get; set; }
    }
}
