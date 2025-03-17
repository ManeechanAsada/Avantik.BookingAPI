using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    public class EFT
    {
        public string bank_name { get; set; }
        public string bank_code { get; set; }
        public string bank_iban { get; set; }
    }
}
