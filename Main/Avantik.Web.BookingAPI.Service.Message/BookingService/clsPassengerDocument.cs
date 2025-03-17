using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    public class PassengerDocument
    {
        public string passport_number { get; set; }
        public string document_type_rcd { get; set; }
        public string passport_issue_place { get; set; }
        public string passport_birth_place { get; set; }
        public string passport_issue_country_rcd { get; set; }
        public string country_code_long { get; set; }

        public DateTime passport_issue_date { get; set; }
        public DateTime passport_expiry_date { get; set; }
    }
}
