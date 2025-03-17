using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    [DataContract]
    public class BookingSaveRequest
    {
        //Authentication information.
        [DataMember]
        public string AgencyCode { get; set; }
        [DataMember]
        public string UserLogon { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public BookingHeader BookingHeader { get; set; }
        [DataMember]
        public IList<FlightSegment> BookingSegments { get; set; }
        [DataMember]
        public IList<Passenger> Passengers { get; set; }
        [DataMember]
        public IList<Payment> Payments { get; set; }
        [DataMember]
        public IList<Mapping> Mappings { get; set; }
        [DataMember]
        public IList<Tax> Taxes { get; set; }
        [DataMember]
        public IList<Fee> Fees { get; set; }
        [DataMember]
        public IList<Remark> Remarks { get; set; }
        [DataMember]
        public IList<Service> Services { get; set; }
    }
}
