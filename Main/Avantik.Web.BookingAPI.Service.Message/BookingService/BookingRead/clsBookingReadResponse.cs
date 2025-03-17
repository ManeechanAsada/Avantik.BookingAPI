using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    [DataContract]
    public class BookingReadResponse : ResponseBase
    {
        [DataMember]
        public BookingHeaderReadResponse BookingHeader { get; set; }
        [DataMember]
        public IList<FlightSegmentReadResponse> BookingSegments { get; set; }
        [DataMember]
        public IList<PassengerReadResponse> Passengers { get; set; }
        [DataMember]
        public IList<PaymentReadResponse> Payments { get; set; }
        [DataMember]
        public IList<MappingReadResponse> Mappings { get; set; }
        [DataMember]
        public IList<TaxReadResponse> Taxes { get; set; }
        [DataMember]
        public IList<FeeReadResponse> Fees { get; set; }
        [DataMember]
        public IList<RemarkReadResponse> Remarks { get; set; }
        [DataMember]
        public IList<ServiceReadResponse> Services { get; set; }
        [DataMember]
        public IList<QuoteReadResponse> Quotes { get; set; }

    }
}
