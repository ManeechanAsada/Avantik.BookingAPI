using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    [DataContract]
    public class BookingReadRequest
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
    }
}
