using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    public class Passenger
    {
        public Guid passenger_id { get; set; }
        //optional use only when want to defind guardian of passenger type CHD
        public Guid guardian_passenger_id { get; set; }
        
        public DateTime date_of_birth { get; set; }

        public string client_number { get; set; }

        public PassengerDocument Document { get; set; }

        public string passenger_type_rcd { get; set; }
        public string lastname { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string title_rcd { get; set; }
        public string member_number { get; set; }
        public string member_airline_rcd { get; set; }
        public string member_level_rcd { get; set; }
        public string gender_type_rcd { get; set; }
        public string nationality_rcd { get; set; }
        public string residence_country_rcd { get; set; }
        public string redress_number { get; set; }

        public byte vip_flag { get; set; }
        
        public decimal passenger_weight { get; set; }

        public string known_traveler_number { get; set; }

        //EDWDEV-81
        public string pnr_lastname { get; set; }

        public string pnr_firstname { get; set; }



    }
}
