using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    [DataContract]
    public class BookingItemsAddResponse : ResponseBase
    {
        [DataMember]
        public string record_locator { get; set; }
        [DataMember]
        public Guid booking_id { get; set; }
        [DataMember]
        public IList<PaymentResponse> Payments { get; set; }
        [DataMember]
        public IList<RemarkResponse> Remarks { get; set; }
        [DataMember]
        public IList<FeeResponse> Fees { get; set; }
    }
}
