using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    [DataContract]
    public class BookingCancelRequest
    {
        //Authentication information.
        [DataMember]
        public string AgencyCode { get; set; }
        [DataMember]
        public string UserLogon { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public Guid booking_id { get; set; }
        [DataMember]
        public bool IsVoidAllFees { get; set; }

        
    }
}
