using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;
using Avantik.Web.BookingAPI.Service.Message.BookingService;

namespace Avantik.Web.BookingAPI.Service.Contract
{
    [ServiceContract(Namespace = "Avantik.Web.BookingAPI.BookingService")]
    public interface IBookingService
    {
        [OperationContract()]
        BookingSaveResponse BookingSave(BookingSaveRequest BookingSaveRequest);
        [OperationContract()]
        BookingCancelResponse BookingCancel(BookingCancelRequest BookingCancelRequest);
        [OperationContract()]
        BookingItemsAddResponse BookingItemsAdd(BookingItemsAddRequest BookingItemsAddRequest);
        [OperationContract()]
        BookingReadResponse BookingRead(BookingReadRequest BookingReadRequest);
    }
}
