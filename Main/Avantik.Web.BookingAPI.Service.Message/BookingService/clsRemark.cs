using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    public class Remark
    {
        public DateTime timelimit_date_time { get; set; }
        public DateTime utc_timelimit_date_time { get; set; }

        public string remark_type_rcd { get; set; }
        public string remark_text { get; set; }
        public string nickname { get; set; }
    }
}
