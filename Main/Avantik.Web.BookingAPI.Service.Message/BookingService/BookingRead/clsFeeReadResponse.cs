using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    public class FeeReadResponse : Fee
    {
        public string display_name { get; set; }
    }
}
