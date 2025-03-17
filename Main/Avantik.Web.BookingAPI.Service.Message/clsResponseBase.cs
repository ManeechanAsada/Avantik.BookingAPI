using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Avantik.Web.BookingAPI.Service.Message
{
    [DataContract]
    public abstract class ResponseBase
    {
        [DataMember]
        public string ErrorCode { get; set; }
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public string Message { get; set; }

    }
}
