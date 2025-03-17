using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    [DataContract]
    public class BookingCancelResponse : ResponseBase
    {
        [DataMember]
        public Guid booking_id { get; set; }
    }
}
