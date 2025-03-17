using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    [DataContract]
    public class BookingSaveResponse : ResponseBase
    {
        //Error message for each node.
        [DataMember]
        public BookingHeaderResponse BookingHeader { get; set; }
        [DataMember]
        public IList<FlightSegmentResponse> BookingSegments { get; set; }
        [DataMember]
        public IList<PassengerResponse> Passengers { get; set; }
        [DataMember]
        public IList<PaymentResponse> Payments { get; set; }
        [DataMember]
        public IList<MappingResponse> Mappings { get; set; }
        [DataMember]
        public IList<TaxResponse> Taxes { get; set; }
        [DataMember]
        public IList<FeeResponse> Fees { get; set; }
        [DataMember]
        public IList<RemarkResponse> Remarks { get; set; }
        [DataMember]
        public IList<ServiceResponse> Services { get; set; }
    }
}
