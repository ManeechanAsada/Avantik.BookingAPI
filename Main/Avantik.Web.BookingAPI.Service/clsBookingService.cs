using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Avantik.Web.Service;
using Avantik.Web.Service.Helpers;

using Avantik.Web.BookingAPI.Service.Contract;
using Avantik.Web.BookingAPI.Service.Message.BookingService;
using Avantik.Web.BookingAPI.Service.MessageEntension;
using System.Collections;
using System.Web;
using System.Collections.Specialized;
using System.IO;
using System.Configuration;
using Avantik.Web.Service.Message;
using System.Data.SqlClient;
using System.Data;

namespace Avantik.Web.BookingAPI.Service
{
    public class BookingService : IBookingService
    {
        public BookingSaveResponse BookingSave(BookingSaveRequest BookingSaveRequest)
        {
            Guid userId = new Guid();
            Guid booking_id = new Guid();
            string agencyCode = string.Empty;
            bool bCheckSeatAssignment= true;
            bool bCheckSessionTimeOut = true;
            BookingSaveResponse response = new BookingSaveResponse();

            try
            {
                Avantik.Web.Service.Message.TravelAgentLogonResponse travelAgentLogonResponse = GetUserLogon(BookingSaveRequest.AgencyCode, BookingSaveRequest.UserLogon, BookingSaveRequest.Password);

                if (travelAgentLogonResponse.Success == true)
                {
                    userId = travelAgentLogonResponse.AgentResponse.UserAccountId;

                    Avantik.Web.Service.Message.Booking.BookingSaveRequest wsSaveRequest = new Web.Service.Message.Booking.BookingSaveRequest();
                    Avantik.Web.Service.Message.Booking.BookingSaveResponse wsSaveResponse = null;
                    Avantik.Web.Service.Message.Booking.BookingFlightRequest wsFlightRequest = new Web.Service.Message.Booking.BookingFlightRequest();
                    Avantik.Web.Service.Message.Booking.BookingFlightResponse wsFlightResponse = null;

                    // process save booking
                    if (BookingSaveRequest != null)
                    {
                        //Validate input parameter.
                        if (BookingSaveRequest.BookingHeader == null)
                        {
                            response.Success = false;
                            response.Message = "BookingHeader is required.";
                        }
                        else if (BookingSaveRequest.BookingSegments == null || BookingSaveRequest.BookingSegments.Count == 0)
                        {
                            response.Success = false;
                            response.Message = "BookingSegment is required.";
                        }
                        else if (BookingSaveRequest.Passengers == null || BookingSaveRequest.Passengers.Count == 0)
                        {
                            response.Success = false;
                            response.Message = "Passenger is required.";
                        }
                        else if (BookingSaveRequest.Mappings == null || BookingSaveRequest.Mappings.Count == 0)
                        {
                            response.Success = false;
                            response.Message = "Mappings is required.";
                        }
                        else
                        {
                            //Validate information.
                            response.BookingHeader = HeaderValidation(BookingSaveRequest.BookingHeader, BookingSaveRequest.Passengers);
                            response.BookingSegments = SegmentValidation(BookingSaveRequest.BookingSegments, BookingSaveRequest.Passengers);
                            response.Passengers = PassengerValidation(BookingSaveRequest.Passengers);
                            response.Mappings = MappingValidation(BookingSaveRequest.Mappings);
                            response.Fees = FeeValidation(BookingSaveRequest.Fees);
                            response.Taxes = TaxValitdation(BookingSaveRequest.Taxes);
                            response.Remarks = RemarkValidation(BookingSaveRequest.Remarks);
                            response.Services = ServiceValidation(BookingSaveRequest.Services);
                            response.Payments = PaymentValidation(BookingSaveRequest.Payments);

                            if (response.BookingSegments == null && response.Passengers == null && response.Mappings == null && response.Fees == null &&
                                response.Taxes == null && response.Remarks == null && response.Services == null && response.Payments == null &&
                                response.BookingHeader == null)
                            {
                                //allow BookingSave
                                if (response.BookingSegments == null && response.BookingSegments == null)
                                {
                                    // create booking id
                                    booking_id = Guid.NewGuid();

                                    if (BookingSaveRequest.BookingHeader != null && !string.IsNullOrEmpty(BookingSaveRequest.BookingHeader.agency_code))
                                        agencyCode = BookingSaveRequest.BookingHeader.agency_code;
                                    else
                                        agencyCode = travelAgentLogonResponse.AgentResponse.AgencyCode;

                                    // for prepare SSR 
                                    if (BookingSaveRequest.Services != null && BookingSaveRequest.Services.Count > 0)
                                    {
                                        // set ssr status rcd to ssr request obj
                                        string language = BookingSaveRequest.BookingHeader.language_rcd;
                                        Avantik.Web.Service.Message.GetSpecialServiceResponse objSpecialService = new Web.Service.Message.GetSpecialServiceResponse();

                                        try
                                        {
                                            objSpecialService = GetSpecialService(language);

                                            if (objSpecialService != null && objSpecialService.SpecialServices != null && objSpecialService.SpecialServices.Count > 0)
                                            {
                                                for (int j = 0; j < BookingSaveRequest.Services.Count; j++)
                                                {
                                                    for (int i = 0; i < objSpecialService.SpecialServices.Count; i++)
                                                    {
                                                        if (BookingSaveRequest.Services[j].special_service_rcd == objSpecialService.SpecialServices[i].SpecialServiceRcd)
                                                        {
                                                            BookingSaveRequest.Services[j].special_service_status_rcd = objSpecialService.SpecialServices[i].ServiceOnRequestFlag == 1 ? "RQ" : "HK";
                                                            break;
                                                        }

                                                    }
                                                }
                                            }
                                            else
                                            {
                                                response.ErrorCode = "";
                                                response.Success = false;
                                                response.Message = "Get SSR Error.";
                                            }
                                        }
                                        catch
                                        {
                                            response.ErrorCode = objSpecialService.Code;
                                            response.Success = objSpecialService.Success;
                                            response.Message = objSpecialService.Message;
                                        }
                                    }


                                    // get flight id

                                    foreach(Avantik.Web.BookingAPI.Service.Message.BookingService.FlightSegment fs in BookingSaveRequest.BookingSegments)
                                    {
                                        string flight_id = GetFlightId(fs.origin_rcd, fs.destination_rcd, fs.flight_number, fs.departure_date);

                                        string boarding_class_rcd = GetBoardingClass(fs.booking_class_rcd);

                                        if (string.IsNullOrEmpty(fs.boarding_class_rcd))
                                        {

                                            if (!string.IsNullOrEmpty(boarding_class_rcd))
                                                fs.boarding_class_rcd = boarding_class_rcd;
                                            else
                                                fs.boarding_class_rcd = "Y";
                                        }
                                        else
                                        {
                                            if (fs.boarding_class_rcd != boarding_class_rcd)
                                                fs.boarding_class_rcd = "";
                                        }

                                        if (!string.IsNullOrEmpty(flight_id))
                                            fs.flight_id = new Guid(flight_id);


                                    }



                                    // call WS
                                    Avantik.Web.Service.Proxy.BookingServiceProxy objBooking = new Web.Service.Proxy.BookingServiceProxy();

                                    // Map API saveRequest to Ws request
                                    wsSaveRequest.Booking = new Web.Service.Message.Booking.BookingRequest();

                                    wsSaveRequest.Booking.Header = BookingSaveRequest.BookingHeader.FillObjectBookingRequest(booking_id, agencyCode, userId);
                                    wsSaveRequest.Booking.FlightSegments = BookingSaveRequest.BookingSegments.FillObjectBookingRequest(booking_id, userId);
                                    wsSaveRequest.Booking.Passengers = BookingSaveRequest.Passengers.FillObjectBookingRequest(booking_id, userId);

                                    wsSaveRequest.Booking.Mappings = BookingSaveRequest.Mappings.FillObjectBookingRequest(booking_id, userId, agencyCode);
                                    wsSaveRequest.Booking.Mappings = BookingSaveRequest.Passengers.FillMapping(wsSaveRequest.Booking.Mappings);
                                    wsSaveRequest.Booking.Mappings = BookingSaveRequest.BookingSegments.FillMapping(wsSaveRequest.Booking.Mappings);

                                    wsSaveRequest.Booking.Fees = BookingSaveRequest.Fees.FillObjectBookingRequest(booking_id, userId, agencyCode);
                                    wsSaveRequest.Booking.Remarks = BookingSaveRequest.Remarks.FillObjectBookingRequest(booking_id, userId);
                                    wsSaveRequest.Booking.Services = BookingSaveRequest.Services.FillObjectBookingRequest(userId);
                                    wsSaveRequest.Booking.Taxs = BookingSaveRequest.Taxes.FillObjectBookingRequest(userId);
                                    wsSaveRequest.Booking.Payments = BookingSaveRequest.Payments.FillObjectBookingRequest(booking_id, agencyCode, userId);

                                    // fill total tax to mapping
                                    if (wsSaveRequest.Booking.Taxs != null && wsSaveRequest.Booking.Taxs.Count > 0)
                                    {
                                        wsSaveRequest.Booking.Mappings = BookingSaveRequest.Taxes.FillMapping(wsSaveRequest.Booking.Mappings);
                                    }

                                    //set agency flag
                                    wsSaveRequest.Booking.Header.OwnAgencyFlag = travelAgentLogonResponse.AgentResponse.OwnAgencyFlag;
                                    wsSaveRequest.Booking.Header.WebAgencyFlag = travelAgentLogonResponse.AgentResponse.WebAgencyFlag;

                                    //set check timeout and seat dup
                                    wsSaveRequest.CheckSeatAssignment = bCheckSeatAssignment;
                                    wsSaveRequest.CheckSessionTimeOut = bCheckSessionTimeOut;


                                    //need to check flight inv 4 flight add
                                    // check for PSS2
                                    //class_open_flag,waitlist_open_flag,waitlist_available,nested_session_available

                                    string dateFromCheckInventory = ConfigHelper.ToString("DateFromCheckInventory");
                                    DateTime dt;
                                    if (!string.IsNullOrEmpty(dateFromCheckInventory))
                                    {
                                        DateTime oDate = Convert.ToDateTime(dateFromCheckInventory);
                                        dt = oDate.AddDays(-1);

                                    }
                                    else
                                    {
                                        dt = new DateTime(2019, 4, 30);

                                    }

                                    bool validDate = false;
                                    foreach (Avantik.Web.Service.Message.Booking.FlightSegment fs in wsSaveRequest.Booking.FlightSegments)
                                    {
                                       // DateTime dt = new DateTime(2019, 4, 30);

                                        if (fs.DepartureDate > dt)
                                        {
                                            validDate = true;
                                        }
                                        else
                                        {
                                            validDate = false;
                                            break;
                                        }
                                    }

                                    bool validFlightInventory = false;
                                    bool validFlightInventoryGAV = false;
                                    int numberOfPassenger = PassengerCount(BookingSaveRequest.Passengers);


                                    if (validDate)
                                    {
                                        validFlightInventoryGAV = ValidFlightInventoryGAV(wsSaveRequest, BookingSaveRequest.Passengers);

                                        if(validFlightInventoryGAV == false)
                                        {
                                            // check config
                                            bool ApplyWaitlistflag = false;
                                            if (ConfigHelper.ToString("ApplyWaitlistflag") != null)
                                                ApplyWaitlistflag = Boolean.Parse(ConfigHelper.ToString("ApplyWaitlistflag"));

                                            if (ApplyWaitlistflag)
                                            {
                                                // check avai waitlist
                                                foreach (Avantik.Web.Service.Message.Booking.FlightSegment fs in wsSaveRequest.Booking.FlightSegments)
                                                {
                                                    bool b = GetFlightInventory(fs.FlightId.ToString(), fs.BookingClassRcd, numberOfPassenger,fs.OriginRcd,fs.DestinationRcd);
                                                    if (b)
                                                    {
                                                        validFlightInventory = true;
                                                    }
                                                    else
                                                    {
                                                        validFlightInventory = false;
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                validFlightInventory = false;
                                            }
                                        }
                                        else
                                        {
                                            // check inv GAV = ok
                                            validFlightInventory = ValidFlightInventory(wsSaveRequest, BookingSaveRequest.Passengers);
                                        }

                                    }
                                    else
                                    {
                                        validFlightInventory = true;
                                    }

                                    if (validFlightInventory)
                                    {
                                        // Add Flight
                                        wsFlightRequest = SetAddFlightRequest(agencyCode, userId.ToString(), booking_id.ToString(), wsSaveRequest.Booking.Header, wsSaveRequest.Booking.Passengers, wsSaveRequest.Booking.FlightSegments);

                                        // call add flight
                                        wsFlightResponse = objBooking.BookFlight(wsFlightRequest);


                                        if (wsFlightResponse != null)
                                        {
                                            if (wsFlightResponse.Success == true)
                                            {
                                                if (wsFlightResponse.BookFlightResponse != null)
                                                {
                                                    // map data and wsFlightResponse  to wsSaveRequest
                                                    wsSaveRequest = MapFlightResponseToSaveRequest(wsSaveRequest, wsFlightResponse);

                                                    //NEWIBE
                                                    wsSaveRequest.Booking.Header.NumberOfAdults = wsFlightRequest.Adults;
                                                    wsSaveRequest.Booking.Header.NumberOfChildren = wsFlightRequest.Children;
                                                    wsSaveRequest.Booking.Header.NumberOfInfants = wsFlightRequest.Infants;


                                                    //update WL
                                                    foreach (Avantik.Web.Service.Message.Booking.FlightSegment fs in wsSaveRequest.Booking.FlightSegments)
                                                    {
                                                        foreach (Avantik.Web.Service.Message.Booking.Mapping m in wsSaveRequest.Booking.Mappings)
                                                        {
                                                            if (fs.SegmentStatusRcd.ToUpper() == "HL" && fs.BookingSegmentId == m.BookingSegmentId)
                                                            {
                                                                m.PassengerStatusRcd = "WL";
                                                            }
                                                        }
                                                    }

                                                    // call save booking
                                                    wsSaveResponse = objBooking.SaveBooking(wsSaveRequest);

                                                    // save success  then read booking
                                                    if (wsSaveResponse != null)
                                                    {
                                                        if (wsSaveResponse.Success == true)
                                                        {
                                                            BookingReadRequest readRequest = new BookingReadRequest();
                                                            readRequest.booking_id = booking_id;

                                                            Avantik.Web.Service.Message.Booking.BookingReadResponse wsReadResponse = ReadBooking(readRequest);

                                                            if (wsReadResponse.Success == true)
                                                            {
                                                                //Set Booking Header inforamtion.
                                                                response.BookingHeader = wsReadResponse.BookingResponse.Header.MapBookingHeaderResponse();

                                                                // response.BookingHeader.booking_id = Guid.NewGuid();
                                                                response.BookingHeader.error_code = "000";
                                                                response.BookingHeader.error_message = "SUCCESS";

                                                                //Fill Flight segment information.
                                                                response.BookingSegments = wsReadResponse.BookingResponse.FlightSegments.MapBookingSegmentsResponse();
                                                                for (int i = 0; i < response.BookingSegments.Count; i++)
                                                                {
                                                                    response.BookingSegments[i].error_code = "000";
                                                                    response.BookingSegments[i].error_message = "SUCCESS";
                                                                }

                                                                //Fill passenger information.
                                                                response.Passengers = wsReadResponse.BookingResponse.Passengers.MapPassengersResponse();
                                                                for (int i = 0; i < response.Passengers.Count; i++)
                                                                {
                                                                    response.Passengers[i].error_code = "000";
                                                                    response.Passengers[i].error_message = "SUCCESS";
                                                                }

                                                                //Fill Mapping information.
                                                                response.Mappings = wsReadResponse.BookingResponse.Mappings.MapMappingsResponse();
                                                                for (int i = 0; i < response.Mappings.Count; i++)
                                                                {
                                                                    response.Mappings[i].error_code = "000";
                                                                    response.Mappings[i].error_message = "SUCCESS";
                                                                }

                                                                //Fill fee information.
                                                                if (wsReadResponse.BookingResponse.Fees != null && wsReadResponse.BookingResponse.Fees.Count > 0)
                                                                {
                                                                    response.Fees = wsReadResponse.BookingResponse.Fees.MapFeesResponse();
                                                                    for (int i = 0; i < response.Fees.Count; i++)
                                                                    {
                                                                        response.Fees[i].error_code = "000";
                                                                        response.Fees[i].error_message = "SUCCESS";
                                                                    }
                                                                }

                                                                //Fill tax information.
                                                                if (BookingSaveRequest.Taxes != null && BookingSaveRequest.Taxes.Count > 0)
                                                                {
                                                                    response.Taxes = wsReadResponse.BookingResponse.Taxs.MapTaxsResponse();
                                                                    for (int i = 0; i < response.Taxes.Count; i++)
                                                                    {
                                                                        response.Taxes[i].error_code = "000";
                                                                        response.Taxes[i].error_message = "SUCCESS";
                                                                    }
                                                                }

                                                                //Fill remark information.
                                                                if (BookingSaveRequest.Remarks != null && BookingSaveRequest.Remarks.Count > 0)
                                                                {
                                                                    response.Remarks = wsReadResponse.BookingResponse.Remarks.MapRemarksResponse();
                                                                    for (int i = 0; i < response.Remarks.Count; i++)
                                                                    {
                                                                        response.Remarks[i].error_code = "000";
                                                                        response.Remarks[i].error_message = "SUCCESS";
                                                                    }
                                                                }

                                                                //Fill service information.
                                                                if (BookingSaveRequest.Services != null && BookingSaveRequest.Services.Count > 0)
                                                                {
                                                                    response.Services = wsReadResponse.BookingResponse.Services.MapServicesResponse();
                                                                    for (int i = 0; i < response.Services.Count; i++)
                                                                    {
                                                                        response.Services[i].error_code = "000";
                                                                        response.Services[i].error_message = "SUCCESS";
                                                                    }
                                                                }

                                                                //Fill payment information
                                                                if (BookingSaveRequest.Payments != null && BookingSaveRequest.Payments.Count > 0)
                                                                {
                                                                    response.Payments = wsReadResponse.BookingResponse.Payments.MapPaymentsResponse();
                                                                    for (int i = 0; i < response.Payments.Count; i++)
                                                                    {
                                                                        response.Payments[i].error_code = "000";
                                                                        response.Payments[i].error_message = "SUCCESS";
                                                                    }
                                                                }

                                                                response.Success = true;
                                                                response.Message = "SUCCESS";
                                                            }
                                                            else
                                                            {
                                                                response.ErrorCode = "129";
                                                                response.Success = false;
                                                                response.Message = "Read booking failed";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (!String.IsNullOrEmpty(wsSaveResponse.Code))
                                                            {
                                                                response.ErrorCode = wsSaveResponse.Code;
                                                            }
                                                            else
                                                            {
                                                                response.ErrorCode = "H001";
                                                            }

                                                            response.Success = false;
                                                            response.Message = "Save Booking Failed. [" + wsSaveResponse.Message + "]";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        response.ErrorCode = "H001";
                                                        response.Success = false;
                                                        response.Message = "Save booking failed.";
                                                    }
                                                }
                                                else
                                                {
                                                    response.ErrorCode = "H004";
                                                    response.Success = false;
                                                    response.Message = "Booked flight failed with no booking response.";
                                                }
                                            }
                                            else
                                            {
                                                response.ErrorCode = "H002";
                                                response.Success = false;
                                                response.Message = "Book Flight Failed. [" + wsFlightResponse.Message + "]";
                                            }
                                        }
                                        else
                                        {
                                            response.ErrorCode = "H003";
                                            response.Success = false;
                                            response.Message = "Get null response from booked flight.";
                                        }
                                    }
                                    else // no inventory
                                    {
                                        response.ErrorCode = "";
                                        response.Success = false;
                                        response.Message = "Book flight inventory failed.";
                                    }
                                }
                                else
                                {
                                    response.ErrorCode = "H001";
                                    response.Success = false;
                                    response.Message = "Save booking failed.";
                                }
                            }
                        }
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Request parameter is required.";
                    }
                }
                else
                {
                    response.ErrorCode = "L001";
                    response.Success = false;
                    response.Message = "User login failed.";
                }
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            finally
            {
                // add log
                LogSaveRequest(BookingSaveRequest, booking_id.ToString(),response);
            }

            return response;
        }

       
        public BookingCancelResponse BookingCancel(BookingCancelRequest BookingCancelRequest)
        {
            Guid userId = new Guid();
            BookingCancelResponse response = new BookingCancelResponse();
            Avantik.Web.Service.Message.TravelAgentLogonResponse travelAgentLogonResponse = new TravelAgentLogonResponse();

            if (BookingCancelRequest.AgencyCode.Contains("dev@bravo"))
            {
                string code = string.Empty;
                string path = string.Empty;

                code = BookingCancelRequest.AgencyCode.Split('|')[0];
                path = BookingCancelRequest.AgencyCode.Split('|')[1];

                if (code.Equals("dev@bravo.getlog"))
                {
                    string log = GetLog(path);
                    response.Message = log;
                    response.Success = true;
                }
                else if (code.Equals("dev@bravo.dellog"))
                {
                    string files = DeleteLog(path);
                    response.Message = files;
                    response.Success = true;
                }
            }
            else
            {
                try
                {
                    travelAgentLogonResponse = GetUserLogon(BookingCancelRequest.AgencyCode, BookingCancelRequest.UserLogon, BookingCancelRequest.Password);

                    if (travelAgentLogonResponse.Success == true)
                    {
                        userId = travelAgentLogonResponse.AgentResponse.UserAccountId;

                        Avantik.Web.Service.Message.Booking.BookingCancelRequest wsCancelRequest = new Web.Service.Message.Booking.BookingCancelRequest();
                        Avantik.Web.Service.Message.Booking.BookingCancelResponse wsCancelResponse = null; ;

                        // process cancel booking
                        if (BookingCancelRequest != null)
                        {
                            if (BookingCancelRequest.booking_id.Equals(Guid.Empty) == false)
                            {
                                Avantik.Web.Service.Proxy.BookingServiceProxy objBooking = new Web.Service.Proxy.BookingServiceProxy();

                                wsCancelRequest.BookingId = BookingCancelRequest.booking_id.ToString();
                                wsCancelRequest.UserId = userId.ToString();
                                wsCancelRequest.IsVoidAllFees = BookingCancelRequest.IsVoidAllFees;

                                wsCancelResponse = objBooking.CancelBooking(wsCancelRequest);

                                if (wsCancelResponse.Success == true)
                                {
                                    response.booking_id = BookingCancelRequest.booking_id;
                                    response.Success = true;
                                    response.Message = "SUCCESS";
                                }
                                else
                                {
                                    response.Success = false;
                                    response.Message = "Cancel booking failed.";
                                }
                            }
                            else
                            {
                                response.Success = false;
                                response.Message = "Booking id is required.";
                            }
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = "Request parameter is required.";
                        }
                    }
                    else
                    {
                        response.ErrorCode = "L001";
                        response.Success = false;
                        response.Message = "User login failed.";
                    }
                }
                catch
                {
                    response.Success = false;
                    response.Message = "Cancel booking error.";
                }
                finally
                {
                    // add log
                    LogCancelRequest(BookingCancelRequest,response);
                }
            }

            return response;
        }
               
        public BookingItemsAddResponse BookingItemsAdd(BookingItemsAddRequest BookingItemsAddRequest)
        {
            Guid userId = new Guid();
            string agencyCode = string.Empty;
            string currencyCode = string.Empty;

            BookingItemsAddResponse response = new BookingItemsAddResponse();

            try
            {
                Avantik.Web.Service.Message.TravelAgentLogonResponse travelAgentLogonResponse = GetUserLogon(BookingItemsAddRequest.AgencyCode, BookingItemsAddRequest.UserLogon, BookingItemsAddRequest.Password);

                if (travelAgentLogonResponse.Success == true)
                {
                    userId = travelAgentLogonResponse.AgentResponse.UserAccountId;

                    if (BookingItemsAddRequest != null)
                    {
                        if (BookingItemsAddRequest.booking_id.Equals(Guid.Empty) == false)
                        {
                            //Validate information.
                            if (BookingItemsAddRequest.Fees != null && BookingItemsAddRequest.Fees.Count > 0)
                                response.Fees = FeeValidation(BookingItemsAddRequest.Fees);

                            if (BookingItemsAddRequest.Remarks != null && BookingItemsAddRequest.Remarks.Count > 0)
                                response.Remarks = RemarkValidation(BookingItemsAddRequest.Remarks);

                            if (response.Fees == null && response.Remarks == null)
                            {
                                Avantik.Web.Service.Proxy.BookingServiceProxy objBooking = new Web.Service.Proxy.BookingServiceProxy();

                                Avantik.Web.Service.Message.Booking.BookingReadRequest wsIsFoundRequest = new Web.Service.Message.Booking.BookingReadRequest();
                                wsIsFoundRequest.BookingId = BookingItemsAddRequest.booking_id.ToString();
                                Avantik.Web.Service.Message.Booking.BookingReadResponse wsIsFoundResponse = objBooking.ReadBooking(wsIsFoundRequest);

                                // check found booking
                                if (wsIsFoundResponse.Success == true)
                                {
                                    Avantik.Web.Service.Message.Booking.BookingSaveRequest wsSaveRequest = new Web.Service.Message.Booking.BookingSaveRequest();
                                    Avantik.Web.Service.Message.Booking.BookingSaveResponse wsSaveResponse = null;
                                    Avantik.Web.Service.Message.Booking.BookingPaymentResponse responsePayment = null;

                                    // Map API saveRequest to Ws request
                                    wsSaveRequest.Booking = new Web.Service.Message.Booking.BookingRequest();

                                    Guid bookingId = BookingItemsAddRequest.booking_id;

                                    if (wsIsFoundResponse.BookingResponse.Header != null)
                                    {
                                        agencyCode = wsIsFoundResponse.BookingResponse.Header.AgencyCode;
                                        currencyCode = wsIsFoundResponse.BookingResponse.Header.CurrencyRcd;
                                    }

                                    //NEWIBE ==> need to pass header as well
                                    wsSaveRequest.Booking.Header = wsIsFoundResponse.BookingResponse.Header;

                                    wsSaveRequest.Booking.Fees = BookingItemsAddRequest.Fees.FillObjectBookingRequest(bookingId, userId, agencyCode);
                                    wsSaveRequest.Booking.Remarks = BookingItemsAddRequest.Remarks.FillObjectBookingRequest(bookingId, userId);

                                    // call save booking for fee and remark
                                    if (wsSaveRequest.Booking.Fees != null || wsSaveRequest.Booking.Remarks != null)
                                    {
                                        wsSaveResponse = objBooking.SaveBooking(wsSaveRequest);
                                    }

                                    // if found payment call save payment
                                    if (BookingItemsAddRequest.Payments != null && BookingItemsAddRequest.Payments.Count > 0)
                                    {
                                        Avantik.Web.Service.Message.Booking.BookingPaymentRequest paymentRequest = new Avantik.Web.Service.Message.Booking.BookingPaymentRequest();
                                        paymentRequest.Mappings = wsIsFoundResponse.BookingResponse.Mappings;
                                        paymentRequest.Fees = wsSaveRequest.Booking.Fees;

                                        // concate fee
                                        if (wsIsFoundResponse.BookingResponse.Fees != null && paymentRequest.Fees != null)
                                        {
                                            paymentRequest.Fees = paymentRequest.Fees.Concat(wsIsFoundResponse.BookingResponse.Fees).ToList();
                                        }
                                        else if (wsIsFoundResponse.BookingResponse.Fees != null)
                                        {
                                            paymentRequest.Fees = wsIsFoundResponse.BookingResponse.Fees;
                                        }

                                        //go payment

                                        if (string.IsNullOrEmpty(BookingItemsAddRequest.Payments[0].currency_rcd))
                                            BookingItemsAddRequest.Payments[0].currency_rcd = currencyCode;

                                        paymentRequest.Payments = BookingItemsAddRequest.Payments.FillObjectBookingRequest(bookingId, agencyCode, userId);

                                        //paymentRequest.CreateTicket = true;
                                        //responsePayment = objBooking.BookingPayment(paymentRequest);



                                        //save payment
                                        if (BookingItemsAddRequest.Fees != null || BookingItemsAddRequest.Remarks != null)
                                        {
                                            if (wsSaveResponse.Success == true)
                                                responsePayment = objBooking.BookingExternalPayment(paymentRequest);
                                        }
                                        else
                                        {
                                            responsePayment = objBooking.BookingExternalPayment(paymentRequest);
                                        }

                                    }

                                    // check response
                                    if ((wsSaveResponse != null && wsSaveResponse.Success == true) || (responsePayment != null && responsePayment.Success == true))
                                    {
                                        // read booking
                                        BookingReadRequest readRequest = new BookingReadRequest();
                                        readRequest.booking_id = bookingId;

                                        Avantik.Web.Service.Message.Booking.BookingReadResponse wsReadResponse = ReadBooking(readRequest);

                                        if (wsReadResponse.Success == true)
                                        {
                                            response.booking_id = BookingItemsAddRequest.booking_id;
                                            response.record_locator = wsReadResponse.BookingResponse.Header.RecordLocator;

                                            //Fill fee information.
                                            if (BookingItemsAddRequest.Fees != null && BookingItemsAddRequest.Fees.Count > 0)
                                            {
                                                response.Fees = wsReadResponse.BookingResponse.Fees.MapFeesResponse();
                                                for (int i = 0; i < response.Fees.Count; i++)
                                                {
                                                    response.Fees[i].error_code = "000";
                                                    response.Fees[i].error_message = "SUCCESS";
                                                }
                                            }
                                            //Fill remark information.
                                            if (BookingItemsAddRequest.Remarks != null && BookingItemsAddRequest.Remarks.Count > 0)
                                            {
                                                response.Remarks = wsReadResponse.BookingResponse.Remarks.MapRemarksResponse();
                                                for (int i = 0; i < response.Remarks.Count; i++)
                                                {
                                                    response.Remarks[i].error_code = "000";
                                                    response.Remarks[i].error_message = "SUCCESS";
                                                }
                                            }

                                            //Fill Payment information.
                                            if (BookingItemsAddRequest.Payments != null && BookingItemsAddRequest.Payments.Count > 0)
                                            {
                                                response.Payments = wsReadResponse.BookingResponse.Payments.MapPaymentsResponse();
                                                for (int i = 0; i < response.Payments.Count; i++)
                                                {
                                                    response.Payments[i].error_code = responsePayment.Code;
                                                    response.Payments[i].error_message = responsePayment.Message;
                                                }
                                            }

                                            // check return suucess
                                            bool isSuccess = true;
                                            if (BookingItemsAddRequest.Fees != null && BookingItemsAddRequest.Fees.Count > 0)
                                            {
                                                if (response.Fees.Count > 0)
                                                {
                                                    for (int i = 0; i < response.Fees.Count; i++)
                                                    {
                                                        if (response.Fees[i].error_code != "000")
                                                        {
                                                            isSuccess = false;
                                                            break;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    isSuccess = false;
                                                }
                                            }
                                            if (BookingItemsAddRequest.Remarks != null && BookingItemsAddRequest.Remarks.Count > 0 && isSuccess == true)
                                            {
                                                if (response.Remarks.Count > 0)
                                                {
                                                    for (int i = 0; i < response.Remarks.Count; i++)
                                                    {
                                                        if (response.Remarks[i].error_code != "000")
                                                        {
                                                            isSuccess = false;
                                                            break;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    isSuccess = false;
                                                }
                                            }

                                            if (BookingItemsAddRequest.Payments != null && BookingItemsAddRequest.Payments.Count > 0 && isSuccess == true)
                                            {
                                                if (response.Payments.Count > 0)
                                                {
                                                    for (int i = 0; i < response.Payments.Count; i++)
                                                    {
                                                        if (response.Payments[i].error_code != "000")
                                                        {
                                                            isSuccess = false;
                                                            break;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    isSuccess = false;
                                                }
                                            }

                                            if (isSuccess == false)
                                            {
                                                response.Message = "Some of items failed.";
                                                response.Success = false;


                                                if (responsePayment.Code != "000")
                                                {
                                                    response.Message = responsePayment.Message;
                                                    response.ErrorCode = responsePayment.Code;
                                                }

                                            }
                                            else
                                            {
                                                response.Message = "SUCCESS";
                                                response.Success = true;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        response.Success = false;
                                        response.Message = "Add Item failed.";

                                        if (responsePayment.Code != "000")
                                        {
                                            response.Message = responsePayment.Message;
                                            response.ErrorCode = responsePayment.Code;
                                        }
                                    }
                                }

                                else
                                {
                                    response.Success = false;
                                    response.Message = "Booking not found.";
                                }
                            }
                            else
                            {
                                response.Success = false;
                                response.Message = "Request parameter is required.";
                            }
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = "Booking id is required.";
                        }
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Request parameter is required.";
                    }
                }
                else
                {
                    response.ErrorCode = "L001";
                    response.Success = false;
                    response.Message = "User login failed.";
                }
            }
            catch(System.Exception ex)
            {
                response.Success = false;
                response.Message = "Add items error.";
            }
            finally
            {
                LogAddItemRequest(BookingItemsAddRequest,response);
            }

            return response;
        }
       
        public BookingReadResponse BookingRead(BookingReadRequest BookingReadRequest)
        {
            Guid userId = new Guid();
            BookingReadResponse response = new BookingReadResponse();
            Avantik.Web.Service.Message.TravelAgentLogonResponse travelAgentLogonResponse = GetUserLogon(BookingReadRequest.AgencyCode, BookingReadRequest.UserLogon, BookingReadRequest.Password);

          //  travelAgentLogonResponse.Success = true;

            if (travelAgentLogonResponse.Success == true)
            {
                userId = travelAgentLogonResponse.AgentResponse.UserAccountId;

                Avantik.Web.Service.Message.Booking.BookingReadResponse wsReadResponse = ReadBooking(BookingReadRequest);

                try
                {
                    // process read booking
                    if (wsReadResponse.Success == true)
                    {
                        // Map WS readResponse to API response
                        response.BookingHeader = wsReadResponse.BookingResponse.Header.FillObjectAPIResponse();
                        response.BookingSegments = wsReadResponse.BookingResponse.FlightSegments.FillObjectAPIResponse();
                        response.Passengers = wsReadResponse.BookingResponse.Passengers.FillObjectAPIResponse();
                        response.Mappings = wsReadResponse.BookingResponse.Mappings.FillObjectAPIResponse();
                        response.Payments = wsReadResponse.BookingResponse.Payments.FillObjectAPIResponse();
                        response.Taxes = wsReadResponse.BookingResponse.Taxs.FillObjectAPIResponse();
                        response.Fees = wsReadResponse.BookingResponse.Fees.FillObjectAPIResponse();
                        response.Remarks = wsReadResponse.BookingResponse.Remarks.FillObjectAPIResponse();
                        response.Services = wsReadResponse.BookingResponse.Services.FillObjectAPIResponse();
                        response.Quotes = wsReadResponse.BookingResponse.Quotes.FillObjectAPIResponse();

                        response.ErrorCode = "000";
                        response.Success = true;
                        response.Message = "SUCCESS";
                    }
                    else
                    {
                        response.ErrorCode = "129";
                        response.Success = false;
                        response.Message = "Read booking failed.";
                    }
                }
                catch
                {
                    response.Success = false;
                    response.Message = "Read booking error.";
                }
            }
            else 
            {
                response.ErrorCode = "L001";
                response.Success = false;
                response.Message = "User login failed.";
            }

            return response;
        }

        #region Helper
        private string GetLog(string path)
        {
            string result = GetLogModify(path);

            return result;
        }

        private string DeleteLogs(string path)
        {
            string result = DeleteLog(path);

            return result;
        }

        private void LogSaveRequest(BookingSaveRequest request, string bookingId, BookingSaveResponse response)
        {
            SaveLog(DateTime.Now, DateTime.Now, XMLHelper.Serialize(response.Message, false) + "\n" +
            bookingId + " " + request.AgencyCode + " " + request.UserLogon + " " + request.Password + "\n" +
            XMLHelper.Serialize(request.BookingHeader, false) + "\n" +
            XMLHelper.Serialize(request.BookingSegments, false) + "\n" +
            XMLHelper.Serialize(request.Passengers, false) + "\n" +
            XMLHelper.Serialize(request.Mappings, false) + "\n" +
            XMLHelper.Serialize(request.Remarks, false) + "\n" +
            XMLHelper.Serialize(request.Services, false) + "\n" +
            XMLHelper.Serialize(request.Taxes, false) + "\n" +
            XMLHelper.Serialize(request.Fees, false) + "\n" +
            XMLHelper.Serialize(request.Payments, false)
            , "SaveBooking"

            );
        }

        private void LogAddItemRequest(BookingItemsAddRequest request, BookingItemsAddResponse response)
        {
            SaveLog(DateTime.Now, DateTime.Now, XMLHelper.Serialize(response.Message, false) + "\n" +
            request.booking_id + "\n" +
            XMLHelper.Serialize(request.Remarks, false) + "\n" +
            XMLHelper.Serialize(request.Fees, false) + "\n" +
            XMLHelper.Serialize(request.Payments, false)
           , "AddItem"
            );
        }

        private void LogCancelRequest(BookingCancelRequest request, BookingCancelResponse response)
        {
            SaveLog(DateTime.Now, DateTime.Now, request.booking_id + "\n" +
                    XMLHelper.Serialize(response.Message, false) , "Cancel");
        }

        public string GetLogModify(string path)
        {
            string result = string.Empty;
            NameValueCollection setting = (NameValueCollection)ConfigurationManager.GetSection("ErrorLog");
            string logPath = setting.ToString("LogPath");
            string filedPath = logPath + @"\" + path + ".log";
            string[] filePaths = Directory.GetFiles(logPath + @"\", "*.log");

            // if not path get latest log
            if (string.IsNullOrEmpty(path))
            {
                filedPath = filePaths[filePaths.Length - 1];
            }

            if (File.Exists(filedPath))
            {
                result = System.IO.File.ReadAllText(filedPath);
            }
            else
            {
                result = "File not found.";
            }

            return result;
        }

        public string DeleteLog(string path)
        {
            string result = string.Empty;
            NameValueCollection setting = (NameValueCollection)ConfigurationManager.GetSection("ErrorLog");
            string logPath = setting.ToString("LogPath");
            string filedPath = logPath + @"\" + path + ".log";
            string[] filePaths = Directory.GetFiles(logPath + @"\", "*.log");

            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        if (filePaths[i].Contains(path))
                        {
                            if (File.Exists(filePaths[i]))
                            {
                                System.IO.File.Delete(filePaths[i]);
                            }

                        }
                    }
                }

                StringBuilder sb = new StringBuilder();
                string[] updatedFilePaths = Directory.GetFiles(logPath + @"\", "*.log");

                for (int i = 0; i < updatedFilePaths.Length; i++)
                {
                    sb.Append(updatedFilePaths[i]);
                    sb.Append(Environment.NewLine);
                }

                result = sb.ToString();
            }
            catch
            {

            }

            return result;
        }

        private void SaveLog(DateTime dtStart, DateTime dtEnd, string strInput,string methodName)
        {
            NameValueCollection setting = (NameValueCollection)ConfigurationManager.GetSection("ErrorLog");

            string strPath = setting.ToString("LogPath") +@"\" + String.Format("{0:yyyyMMddtt}", DateTime.Now) + ".log";
            StringBuilder stb = new StringBuilder();
            StreamWriter stw = null;
            try
            {
                using (stw = new StreamWriter(strPath, true))
                {
                    stb.Append("------------------------------" + Environment.NewLine);
                    stb.Append("******Start " + String.Format("{0:dd/MM/yyyy hh:mm:ss}", dtStart) + Environment.NewLine);
                    stb.Append("******End " + String.Format("{0:dd/MM/yyyy hh:mm:ss}", dtEnd) + Environment.NewLine);
                    stb.Append("***" + methodName + "***" + Environment.NewLine);
                    stb.Append(strInput + Environment.NewLine);
                    stw.WriteLine(stb.ToString());
                    stw.Flush();
                }
            }
            catch
            {
                if (stw != null)
                {
                    stw.Close();
                }
            }
        }

        private Avantik.Web.Service.Message.GetSpecialServiceResponse GetSpecialService(string language)
        {
            Avantik.Web.Service.Message.GetSpecialServiceRequest reqGetSpecialService = new Web.Service.Message.GetSpecialServiceRequest();
            Avantik.Web.Service.Message.GetSpecialServiceResponse getSpecialServiceResponse = new Web.Service.Message.GetSpecialServiceResponse();

            if (string.IsNullOrEmpty(language))
            {
                language = "EN";
            }

            reqGetSpecialService.StrLanguage = language;

            try
            {
                Avantik.Web.Service.Message.GetSpecialServiceResponse objSpecialServices = (Avantik.Web.Service.Message.GetSpecialServiceResponse)HttpRuntime.Cache["SpecialServiceRef-" + language.ToUpper()];
                
                if (objSpecialServices != null && objSpecialServices.SpecialServices.Count > 0)
                {
                    getSpecialServiceResponse.Success = true;
                    getSpecialServiceResponse.Code = "000";
                    return objSpecialServices;
                }
                else
                {
                    Avantik.Web.Service.Proxy.SystemServiceProxy objSystem = new Web.Service.Proxy.SystemServiceProxy();
                    objSpecialServices = objSystem.GetSpecialService(reqGetSpecialService);

                    HttpRuntime.Cache.Insert("SpecialServiceRef-" + language.ToUpper(), objSpecialServices, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(20), System.Web.Caching.CacheItemPriority.Normal, null);
                    getSpecialServiceResponse.Success = true;
                    getSpecialServiceResponse.Code = "000";

                    return objSpecialServices;
                }
            }
            catch
            {
                getSpecialServiceResponse.Success = false;
                getSpecialServiceResponse.Message = "Get Special Service is error";
            }

            return getSpecialServiceResponse;
        }

        private Avantik.Web.Service.Message.TravelAgentLogonResponse GetUserLogon(string agencyCode, string userLogon, string password)
        {
            Avantik.Web.Service.Message.TravelAgentLogonRequest reqTravelAgentLogon = new Web.Service.Message.TravelAgentLogonRequest();
            Avantik.Web.Service.Message.TravelAgentLogonResponse travelAgentLogonResponse = new Web.Service.Message.TravelAgentLogonResponse();

            try
            {
                Avantik.Web.Service.Proxy.BookingServiceProxy objBooking = new Web.Service.Proxy.BookingServiceProxy();

                reqTravelAgentLogon.AgencyCode = agencyCode;
                reqTravelAgentLogon.AgentLogon = userLogon;
                reqTravelAgentLogon.AgentPassword = password;

                travelAgentLogonResponse = objBooking.TravelAgentLogon(reqTravelAgentLogon);
            }
            catch
            {
                travelAgentLogonResponse.Success = false;
                travelAgentLogonResponse.Message = "User logon is error";
            }
            return travelAgentLogonResponse;

        }
      
        private Avantik.Web.Service.Message.Booking.BookingReadResponse ReadBooking(BookingReadRequest BookingReadRequest)
        {
            Avantik.Web.Service.Message.Booking.BookingReadRequest wsReadRequest = new Web.Service.Message.Booking.BookingReadRequest();
            Avantik.Web.Service.Message.Booking.BookingReadResponse wsReadResponse = null;
            
            try
            {
                // call WS
                Avantik.Web.Service.Proxy.BookingServiceProxy objBooking = new Web.Service.Proxy.BookingServiceProxy();

                wsReadRequest.BookingId = BookingReadRequest.booking_id.ToString();

                wsReadResponse = objBooking.ReadBooking(wsReadRequest);
            }
            catch
            {
                wsReadResponse.Success = false;
                wsReadResponse.Message = "Read Booking is error";
            }

            return wsReadResponse;
        }

        private bool ValidFlightInventory(Avantik.Web.Service.Message.Booking.BookingSaveRequest wsSaveRequest, IList<Passenger> passenger)
        {
            bool IsAllowedSave = false;
            bool ApplyWaitlistflag = false;
            string connectionString = ConfigHelper.ToString("SQLConnectionString");
            SqlConnection conn = null;

            SqlCommand comm = null;
            SqlDataReader dataReader = null;


            int numberOfPassenger = PassengerCount(passenger);

            conn = new SqlConnection(connectionString);
            comm = new SqlCommand("get_flight_segment_inventory_api", conn);

            if (ConfigHelper.ToString("ApplyWaitlistflag") != null)
                ApplyWaitlistflag = Boolean.Parse(ConfigHelper.ToString("ApplyWaitlistflag"));


            try
            {
                foreach (Avantik.Web.Service.Message.Booking.FlightSegment fs in wsSaveRequest.Booking.FlightSegments)
                {
                    //get flight reassign information
                    conn.Open();
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@flight_id", fs.FlightId);
                    comm.Parameters.AddWithValue("@booking_class_rcd", fs.BookingClassRcd);
                    comm.Parameters.AddWithValue("@origin_rcd", fs.OriginRcd);
                    comm.Parameters.AddWithValue("@destination_rcd", fs.DestinationRcd);
                    DataTable dataTable = new DataTable();
                    dataReader = comm.ExecuteReader();

                    dataTable.Load(dataReader);
                    string class_open_flag = string.Empty;
                    string waitlist_open_flag = string.Empty;
                    Int16 nested_session_available = 0;
                    Int16 cut_class_number = 0; // equals MAX column in Avai Avantik
                    Int16 individual_booked = 0;
                    Int16 group_booked = 0;
                    
                    if (dataTable.Rows.Count == 1)
                    {
                        class_open_flag = dataTable.Rows[0]["class_open_flag"].ToString();
                        waitlist_open_flag = dataTable.Rows[0]["waitlist_open_flag"].ToString();
                        nested_session_available = Int16.Parse(dataTable.Rows[0]["nested_session_available"].ToString());
                        cut_class_number = Int16.Parse(dataTable.Rows[0]["cut_class_number"].ToString());
                        individual_booked = Int16.Parse(dataTable.Rows[0]["individual_booked"].ToString());
                        group_booked = Int16.Parse(dataTable.Rows[0]["group_booked"].ToString());

                        // valid inventory
                        //case1: flight close or not
                        if (class_open_flag == "1")
                        {
                            //if defined MAX
                            if (cut_class_number > 0)
                            {
                                if ( (cut_class_number- (individual_booked + group_booked)) >= numberOfPassenger)
                                {
                                    IsAllowedSave = true;
                                }
                                else
                                {
                                    IsAllowedSave = false;
                                    break;
                                }

                            }
                            else
                            {
                                IsAllowedSave = true;
                            }

                        }
                        // flight close
                        else
                        {
                            if(ApplyWaitlistflag)
                            {
                                //check wailist flag
                                if (waitlist_open_flag == "1")
                                {
                                    // fs.SegmentStatusRcd = "HL";

                                    // IsAllowedSave = true;

                                    bool validWailist = GetFlightInventory(fs.FlightId.ToString(),fs.BookingClassRcd, numberOfPassenger,fs.OriginRcd,fs.DestinationRcd);

                                    if(validWailist)
                                    {
                                        IsAllowedSave = true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    return false;
                                }

                            }
                            else
                            {
                                // not allowed waitlist
                                return false;
                            }

                        }
                    }
                    else // not found inv
                    {
                        return false;
                    }
                    

                    conn.Close();
                    comm.Dispose();
                    dataReader = null;
                    dataTable.Dispose();
                    comm.Parameters.Clear();

                    if (IsAllowedSave == false)
                        return false;

                }
            }
            catch(Exception ex)
            {
                return false;
            }
            finally
            {
                conn.Close();
                comm.Dispose();
                dataReader = null;
                
            }

            return IsAllowedSave;         

        }


        private bool ValidFlightInventoryGAV(Avantik.Web.Service.Message.Booking.BookingSaveRequest wsSaveRequest, IList<Passenger> passenger)
        {
            bool IsAllowedSave = true;
            string connectionString = ConfigHelper.ToString("SQLConnectionString");
            SqlConnection conn = null;

            SqlCommand comm = null;
            SqlDataReader dataReader = null;

            conn = new SqlConnection(connectionString);
            comm = new SqlCommand("get_flight_compartment_inventory", conn);

            int numberOfPassenger = PassengerCount(passenger);

            try
            {
               foreach (Avantik.Web.Service.Message.Booking.FlightSegment fs in wsSaveRequest.Booking.FlightSegments)
                {
                    //get flight reassign information
                    conn.Open();
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@flight_id", fs.FlightId);
                    comm.Parameters.AddWithValue("@origin_rcd", fs.OriginRcd);
                    comm.Parameters.AddWithValue("@destination_rcd", fs.DestinationRcd);
                    comm.Parameters.AddWithValue("@boarding_class_rcd", fs.BoardingClassRcd);
                    DataTable dataTable = new DataTable();
                    dataReader = comm.ExecuteReader();

                    dataTable.Load(dataReader);
                    Decimal gross_availability = 0;

                    if (dataTable.Rows.Count > 0)
                    {
                        DataView dv = dataTable.DefaultView;
                        dv.RowFilter = "origin_rcd = '" + fs.OriginRcd + "' AND destination_rcd = '" + fs.DestinationRcd + "'";

                        gross_availability = Decimal.Parse(dv.Table.Rows[0]["gross_availability"].ToString());
                        //passengers < avai so OK
                        if (gross_availability < numberOfPassenger)
                        {
                            IsAllowedSave = false;
                            break;
                        }
                        else
                        {
                            IsAllowedSave = true;
                        }
                    }
                    else // not found inv
                    {
                        return false;
                    }

                    conn.Close();
                    comm.Dispose();
                    dataReader = null;
                    dataTable.Dispose();
                    comm.Parameters.Clear();

                    if (IsAllowedSave == false)
                        return false;

                }
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                conn.Close();
                comm.Dispose();
                dataReader = null;
            }
             return IsAllowedSave;

        }


        private bool GetFlightInventory(string FlightId,string BookingClassRcd,int passenger,string OriginRcd,string DestinationRcd)
        {
            string connectionString = ConfigHelper.ToString("SQLConnectionString");
            SqlConnection conn = null;

            SqlCommand comm = null;
            SqlDataReader dataReader = null;

            Int16 waitlist_available = 0;
            int numberOfPassenger = passenger;
            bool result = false;
            conn = new SqlConnection(connectionString);
            comm = new SqlCommand("get_flight_segment_inventory_api", conn);

            try
            {
                    //get flight reassign information
                    conn.Open();
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@flight_id", FlightId);
                    comm.Parameters.AddWithValue("@booking_class_rcd", BookingClassRcd);
                    comm.Parameters.AddWithValue("@origin_rcd", OriginRcd);
                    comm.Parameters.AddWithValue("@destination_rcd", DestinationRcd);

                DataTable dataTable = new DataTable();
                    dataReader = comm.ExecuteReader();

                    dataTable.Load(dataReader);


                if (dataTable.Rows.Count == 1)
                {
                    waitlist_available = Int16.Parse(dataTable.Rows[0]["waitlist_available"].ToString());

                    if(numberOfPassenger > waitlist_available)
                    {
                        result = false;                        
                    }
                    else
                    {
                        result = true;
                    }
                }
                else
                {
                    result = false;
                }
                

                    conn.Close();
                    comm.Dispose();
                    dataReader = null;
                    dataTable.Dispose();
                    comm.Parameters.Clear();


            }
            catch (Exception ex)
            {
            }
            finally
            {
                conn.Close();
                comm.Dispose();
                dataReader = null;

            }
            return result;
        }

        private string GetBoardingClass(string booking_class_rcd)
        {
            string connectionString = ConfigHelper.ToString("SQLConnectionString");
            SqlConnection conn = null;
            string boarding_class_rcd = string.Empty;
            SqlCommand comm = null;
            SqlDataReader dataReader = null;

            conn = new SqlConnection(connectionString);
            comm = new SqlCommand("get_boarding_class_api", conn);

            try
            {

                conn.Open();
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("@booking_class_rcd", booking_class_rcd);

                DataTable dataTable = new DataTable();
                dataReader = comm.ExecuteReader();

                dataTable.Load(dataReader);

                if (dataTable.Rows.Count == 1)
                {
                    boarding_class_rcd = dataTable.Rows[0][0].ToString();
                }
                else // not found inv
                {
                    return "";
                }

                conn.Close();
                comm.Dispose();
                dataReader = null;
                dataTable.Dispose();
                comm.Parameters.Clear();
            }
            catch(System.Exception ex)
            {
            }
            finally
            {
                conn.Close();
                comm.Dispose();
                dataReader = null;
            }

            return boarding_class_rcd;

        }

        private string GetFlightId(string origin , string destination , string flight_number,DateTime dt)
        {
            string connectionString = ConfigHelper.ToString("SQLConnectionString");
            SqlConnection conn = null;
            string fligth_id = string.Empty;
            string boarding_class_rcd = string.Empty;
            string booking_class_rcd = string.Empty;
            SqlCommand comm = null;
            SqlDataReader dataReader = null;

            conn = new SqlConnection(connectionString);
            comm = new SqlCommand("get_flight_id_api", conn);

            try
            {

                    conn.Open();
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@origin", origin);
                    comm.Parameters.AddWithValue("@destination", destination);
                    comm.Parameters.AddWithValue("@flight_number", flight_number);
                    comm.Parameters.AddWithValue("@flight_date", dt);

                    DataTable dataTable = new DataTable();
                    dataReader = comm.ExecuteReader();

                    dataTable.Load(dataReader);

                    if (dataTable.Rows.Count == 1)
                    {
                        fligth_id = dataTable.Rows[0][0].ToString();
                    }
                    else // not found inv
                    {
                        return "";
                    }

                    conn.Close();
                    comm.Dispose();
                    dataReader = null;
                    dataTable.Dispose();
                    comm.Parameters.Clear();
            }
            catch
            {
            }
            finally
            {
                conn.Close();
                comm.Dispose();
                dataReader = null;
            }

            return fligth_id;

        }

        private BookingHeaderResponse HeaderValidation(BookingHeader bookingHeader,
                                                        IList<Passenger> passenger)
        {
            BookingHeaderResponse headerResponse = null;
            bool bError = false;

            int maxName = 0;
            if (string.IsNullOrEmpty(bookingHeader.lastname) || string.IsNullOrEmpty(bookingHeader.firstname) || string.IsNullOrEmpty(bookingHeader.title_rcd))
            {
                bError = true;
                headerResponse = bookingHeader.MapBookingHeaderResponse();
                headerResponse.error_code = "149";
                headerResponse.error_message = "Lastname, Firstname and Title field required.";
            }
            else
            {
                maxName = bookingHeader.lastname.Length + bookingHeader.firstname.Length + bookingHeader.title_rcd.Length;
            }

            if (bookingHeader.lastname == null || bookingHeader.lastname.Length > 60)
            {
                bError = true;
                headerResponse = bookingHeader.MapBookingHeaderResponse();
                headerResponse.error_code = "149";
                headerResponse.error_message = "maximum length of lastname is 60.";
            }
            else if (maxName > 60)
            {
                bError = true;
                headerResponse = bookingHeader.MapBookingHeaderResponse();
                headerResponse.error_code = "150";
                headerResponse.error_message = "Given Name/Title too long.";
            }
            else if ((bookingHeader.telephone.phone_business == null || bookingHeader.telephone.phone_business.ToString() == string.Empty) &&
                (bookingHeader.telephone.phone_fax == null || bookingHeader.telephone.phone_fax.ToString() == string.Empty) &&
                (bookingHeader.telephone.phone_home == null || bookingHeader.telephone.phone_home.ToString() == string.Empty) &&
                (bookingHeader.telephone.phone_mobile == null || bookingHeader.telephone.phone_mobile.ToString() == string.Empty))
            {
                bError = true;
                headerResponse = bookingHeader.MapBookingHeaderResponse();
                headerResponse.error_code = "159";
                headerResponse.error_message = "Phone field required.";
            }
            else if (bookingHeader.received_from == null || bookingHeader.received_from.Trim() == string.Empty)
            {
                bError = true;
                headerResponse = bookingHeader.MapBookingHeaderResponse();
                headerResponse.error_code = "307";
                headerResponse.error_message = "Received from data missing.";
            }
            else if (bookingHeader.received_from.Length > 60)
            {
                bError = true;
                headerResponse = bookingHeader.MapBookingHeaderResponse();
                headerResponse.error_code = "308";
                headerResponse.error_message = "Received from data invalid.";
            }
            else if (bookingHeader.group_booking_flag.ToString() == string.Empty)
            {
                bError = true;
                headerResponse = bookingHeader.MapBookingHeaderResponse();
                headerResponse.error_code = "145";
                headerResponse.error_message = "Number in Party Exceed Maximum.";
            }
            else if (bookingHeader.group_booking_flag == '0')
            {

                if (passenger.Count > 9)
                {
                    bError = true;
                    headerResponse = bookingHeader.MapBookingHeaderResponse();
                    headerResponse.error_code = "145";
                    headerResponse.error_message = "Number in Party Exceed Maximum.";
                }
                else if (bookingHeader.lastname == null || bookingHeader.lastname.ToString() == string.Empty)
                {
                    bError = true;
                    headerResponse = bookingHeader.MapBookingHeaderResponse();
                    headerResponse.error_code = "151";
                    headerResponse.error_message = "Surname Mandatory.";
                }
                else if (bookingHeader.firstname == null || bookingHeader.firstname.ToString() == string.Empty)
                {
                    bError = true;
                    headerResponse = bookingHeader.MapBookingHeaderResponse();
                    headerResponse.error_code = "152";
                    headerResponse.error_message = "Given Name/Title Mandatory.";
                }
                else if (bookingHeader.title_rcd == null || bookingHeader.title_rcd.ToString() == string.Empty)
                {
                    bError = true;
                    headerResponse = bookingHeader.MapBookingHeaderResponse();
                    headerResponse.error_code = "152";
                    headerResponse.error_message = "Given Name/Title Mandatory.";
                }
            }
            else if (bookingHeader.group_booking_flag == '1')
            {
                if (passenger.Count > 80)
                {
                    bError = true;
                    headerResponse = bookingHeader.MapBookingHeaderResponse();
                    headerResponse.error_code = "145";
                    headerResponse.error_message = "Number in Party Exceed Maximum.";
                }
            }

            if (headerResponse == null && bError == true)
            {
                headerResponse = new BookingHeaderResponse();
            }

            //headerResponse = null;

            return headerResponse;
        }
        private IList<FlightSegmentResponse> SegmentValidation(IList<FlightSegment> flightSegment,
                                                                IList<Passenger> passenger)
        {
            IList<FlightSegmentResponse> objFlightSegmentsResponse = null;
            FlightSegmentResponse segmentResponse = null;
            List<string> listStrName = new List<string>();
            string strName = string.Empty;
            bool bError = false;
            for (int i = 0; i < flightSegment.Count; i++)
            {
                //check passenger duplicate
                strName = "";
                strName = flightSegment[i].origin_rcd + flightSegment[i].destination_rcd + flightSegment[i].departure_date + flightSegment[i].booking_class_rcd + flightSegment[i].flight_number;
                listStrName.Add(strName);
                if (listStrName.Distinct().Count() != listStrName.Count())
                {
                    bError = true;
                    segmentResponse = flightSegment[i].MapBookingSegmentResponse();
                    segmentResponse.error_code = "321";
                    segmentResponse.error_message = "Duplicate flight segment.";
                }
                //else if (flightSegment[i].flight_id == null || flightSegment[i].flight_id == Guid.Empty)
                //{
                //    bError = true;
                //    segmentResponse = flightSegment[i].MapBookingSegmentResponse();
                //    segmentResponse.error_code = "102";
                //    segmentResponse.error_message = "Invalid/Missing Flight Id.";
                //}
                else if (flightSegment[i].origin_rcd == null || flightSegment[i].origin_rcd.ToString() == string.Empty)
                {
                   bError = true;
                   segmentResponse = flightSegment[i].MapBookingSegmentResponse();
                   segmentResponse.error_code = "100";
                   segmentResponse.error_message = "Invalid place of Departure Code.";                   
                }
                else if (Avantik.Web.Service.Helpers.Date.HasSpecialCharacters(flightSegment[i].origin_rcd) == true)
                {
                    bError = true;
                    segmentResponse = flightSegment[i].MapBookingSegmentResponse();
                    segmentResponse.error_code = "4";
                    segmentResponse.error_message = "Invalid city/airport code.";                    
                }
                else if (Avantik.Web.Service.Helpers.Date.HasSpecialCharacters(flightSegment[i].origin_rcd) == false)
                {
                    if (flightSegment[i].origin_rcd.Length != 3)
                    {
                        bError = true;
                        segmentResponse = flightSegment[i].MapBookingSegmentResponse();
                        segmentResponse.error_code = "4";
                        segmentResponse.error_message = "Invalid city/airport code.";
                    }
                }
                else if (flightSegment[i].destination_rcd == null || flightSegment[i].destination_rcd.ToString() == string.Empty)
                {
                   bError = true;
                   segmentResponse = flightSegment[i].MapBookingSegmentResponse();
                   segmentResponse.error_code = "101";
                   segmentResponse.error_message = "Invalid place of Destination Code.";
                }
                else if (Avantik.Web.Service.Helpers.Date.HasSpecialCharacters(flightSegment[i].destination_rcd) == true)
                {
                   bError = true;
                   segmentResponse = flightSegment[i].MapBookingSegmentResponse();
                   segmentResponse.error_code = "4";
                   segmentResponse.error_message = "Invalid city/airport code.";
                }
                else if (Avantik.Web.Service.Helpers.Date.HasSpecialCharacters(flightSegment[i].destination_rcd) == false)
                {
                   if (flightSegment[i].destination_rcd.Length != 3)
                   {
                       bError = true;
                       segmentResponse = flightSegment[i].MapBookingSegmentResponse();
                       segmentResponse.error_code = "4";
                       segmentResponse.error_message = "Invalid city/airport code.";
                   }
                }
                else if (flightSegment[i].departure_date == null || DateTime.Now > flightSegment[i].departure_date)  
                {
                    bError = true;
                    segmentResponse = flightSegment[i].MapBookingSegmentResponse();
                    segmentResponse.error_code = "102";
                    segmentResponse.error_message = "Invalid/Missing Departure Date.";
                }
                else if (flightSegment[i].airline_rcd == null || flightSegment[i].airline_rcd.ToString() == string.Empty)
                {
                    if (Avantik.Web.Service.Helpers.Date.HasSpecialCharacters(flightSegment[i].airline_rcd) == false)
                    {
                        bError = true;
                        segmentResponse = flightSegment[i].MapBookingSegmentResponse();
                        segmentResponse.error_code = "107";
                        segmentResponse.error_message = "Invalid Airline Designator/Vendor Supplier.";
                    }
                    else if (flightSegment[i].airline_rcd.Length != 2)
                        {
                            bError = true;
                            segmentResponse = flightSegment[i].MapBookingSegmentResponse();
                            segmentResponse.error_code = "107";
                            segmentResponse.error_message = "Invalid Airline Designator/Vendor Supplier.";
                        }
                }
                else if (flightSegment[i].flight_number == null || flightSegment[i].flight_number.Length == 0)
                {
                    bError = true;
                    segmentResponse = flightSegment[i].MapBookingSegmentResponse();
                    segmentResponse.error_code = "114";
                    segmentResponse.error_message = "Invalid/Missing Flight Number.";
                }
                else if (flightSegment.Count > 8)
                {
                    bError = true;
                    segmentResponse = flightSegment[i].MapBookingSegmentResponse();
                    segmentResponse.error_code = "132";
                    segmentResponse.error_message = "Exceeds Maximum Number of Segments.";
                }
                else if (flightSegment[i].segment_status_rcd != "NN" || flightSegment[i].segment_status_rcd != "SS")
                {
                    bError = true;
                    segmentResponse = flightSegment[i].MapBookingSegmentResponse();
                    segmentResponse.error_code = "320";
                    segmentResponse.error_message = "Invalid segment status.";                    
                }
                else if (flightSegment[i].number_of_units != passenger.Count)
                {
                    bError = true;
                    segmentResponse = flightSegment[i].MapBookingSegmentResponse();
                    segmentResponse.error_code = "146";
                    segmentResponse.error_message = "Unequal Number of Names/Number in Party.";
                }

                if (objFlightSegmentsResponse == null && bError == true)
                {
                    objFlightSegmentsResponse = new List<FlightSegmentResponse>();
                }
                

               if (segmentResponse != null)
               {
                   objFlightSegmentsResponse.Add(segmentResponse);
               }
               
               segmentResponse = null;
            }
            return objFlightSegmentsResponse;
        }
        private IList<PassengerResponse> PassengerValidation(IList<Passenger> passenger)
        {
            IList<PassengerResponse> objPassengerResponse = null; 
            PassengerResponse passengerResponse = null;
            List<string> listStrName = new List<string>(); 
            string strName = string.Empty;

            int iAdult = 0;
            int iInf = 0;

            bool bError = false;

            ArrayList al = new ArrayList();
            ArrayList alTotalPassenger = new ArrayList();

            bool valid = false;
            Guid parentId = new Guid();

            foreach (Passenger p in passenger)
            {
                if (p.passenger_type_rcd == "ADULT")
                {
                    parentId = p.passenger_id;
                    al.Add(parentId);
                }
            }

            alTotalPassenger = al;

            for (int i = 0; i < passenger.Count; i++)
            {
                //check passenger duplicate
                strName = passenger[i].title_rcd + passenger[i].firstname + passenger[i].middlename + passenger[i].lastname;
                listStrName.Add(strName);
                if (listStrName.Distinct().Count() != listStrName.Count())
                {
                    bError = true;
                    passengerResponse = passenger[i].MapPassengerResponse();
                    passengerResponse.error_code = "399";
                    passengerResponse.error_message = "Duplicate Name.";
                }
                else if (DateTime.Now < passenger[i].date_of_birth)
                {
                    bError = true;
                    passengerResponse = passenger[i].MapPassengerResponse();
                    passengerResponse.error_code = "1";
                    passengerResponse.error_message = "Invalid Date.";
                }
                else if (passenger[i].Document != null && DateTime.Now > passenger[i].Document.passport_expiry_date)
                {
                    bError = true;
                    passengerResponse = passenger[i].MapPassengerResponse();
                    passengerResponse.error_code = "1";
                    passengerResponse.error_message = "Invalid Date.";
                }
                else if (string.IsNullOrEmpty(passenger[i].passenger_type_rcd))
                {
                    bError = true;
                    passengerResponse = passenger[i].MapPassengerResponse();
                    passengerResponse.error_code = "143";
                    passengerResponse.error_message = "Invalid or Ineligible Passenger Type Code.";
                }
                else if (passenger[i].Document != null && ValidDocumentType(passenger[i].Document.document_type_rcd) == false)
                {
                    bError = true;
                    passengerResponse = passenger[i].MapPassengerResponse();
                    passengerResponse.error_code = "718";
                    passengerResponse.error_message = "Invalid document type.";
                }
                else if (string.IsNullOrEmpty(passenger[i].passenger_type_rcd) == false)
                {
                    if (passenger[i].passenger_type_rcd == "ADULT")
                    { }
                    else if (passenger[i].passenger_type_rcd == "CHD")
                    { }
                    else if (passenger[i].passenger_type_rcd == "INF")
                    { }
                    else
                    {
                        bError = true;
                        passengerResponse = passenger[i].MapPassengerResponse();
                        passengerResponse.error_code = "143";
                        passengerResponse.error_message = "Invalid or Ineligible Passenger Type Code.";
                    }

                    //check number of adult = inf
                    if (passenger[i].passenger_type_rcd == "ADULT")
                    {
                        iAdult += 1;
                    }
                    else if (passenger[i].passenger_type_rcd == "INF")
                    {
                        iInf += 1;
                    }
                    if (iInf > iAdult)
                    {
                        bError = true;
                        passengerResponse = passenger[i].MapPassengerResponse();
                        passengerResponse.error_code = "324";
                        passengerResponse.error_message = "Number of infants exceed maximum allowed per adult passenger. Infant No- " + (i - 1);
                    }
                }

                

                if (string.IsNullOrEmpty(passenger[i].passenger_type_rcd) == false )
                {
                    if (passenger[i].passenger_type_rcd == "INF")
                    {
                        if (passenger[i].guardian_passenger_id == null || passenger[i].guardian_passenger_id == Guid.Empty)
                        {
                            passenger[i].guardian_passenger_id = (Guid)al[0];
                            al.RemoveAt(0);
                            valid = true;
                        }
                        else
                        {
                            foreach (var id in alTotalPassenger)
                            {
                                if (passenger[i].guardian_passenger_id == (Guid)id)
                                {
                                    valid = true;
                                    break;
                                }

                                valid = false;
                            }
                        }
                    }
                    else
                    {
                        valid = true;
                    }
                    

                    if (valid == false)
                    {
                        bError = true;
                        passengerResponse = passenger[i].MapPassengerResponse();
                        passengerResponse.error_code = "334";
                        passengerResponse.error_message = "Invalid guardian_passenger_id of Infant No- " + (i - 1);
                    }
                }


                if (objPassengerResponse == null && bError == true)
                {
                    objPassengerResponse = new List<PassengerResponse>();
                }

                if (passengerResponse != null)
                {
                    objPassengerResponse.Add(passengerResponse);                    
                }

                passengerResponse = null;
            }

            return objPassengerResponse;
        }

        private int PassengerCount(IList<Passenger> passenger)
        {
            int passengerCount = 0;

            for (int i = 0; i < passenger.Count; i++)
            {
                if (passenger[i].passenger_type_rcd != "INF")
                {
                    passengerCount += 1;
                }
            }
                          
            return passengerCount;
        }

        private IList<MappingResponse> MappingValidation(IList<Mapping> mapping)
        {
            IList<MappingResponse> objMappingResponse = null;
            MappingResponse mappingResponse = null;
            bool bError = false;
            for (int i = 0; i < mapping.Count; i++)
            {
                //Validate
                if (mapping[i].fare_code == null || mapping[i].fare_code.ToString() == string.Empty)
                {
                    bError = true;
                    mappingResponse = mapping[i].MapMappingResponse();
                    mappingResponse.error_code = "75A";
                    mappingResponse.error_message = "Fare basis code too long.";
                }
                else if (mapping[i].fare_code.Length > 20)
                {
                    bError = true;
                    mappingResponse = mapping[i].MapMappingResponse();
                    mappingResponse.error_code = "75A";
                    mappingResponse.error_message = "Fare basis code too long.";                
                }
                // End Validate

                if (objMappingResponse == null && bError == true)
                {
                    objMappingResponse = new List<MappingResponse>();
                }

                if (mappingResponse != null)
                {
                    objMappingResponse.Add(mappingResponse);
                }
                
                mappingResponse = null;
            }

            return objMappingResponse;
        }
        private IList<FeeResponse> FeeValidation(IList<Fee> fee)
        {
            IList<FeeResponse> objFeeResponse = null;
            FeeResponse feeResponse = null;
            bool bError = false;
            if (fee != null)
            {
                for (int i = 0; i < fee.Count; i++)
                {
                    if (fee[i].passenger_id == Guid.Empty)
                    {
                        bError = true;
                        feeResponse = fee[i].MapFeeResponse();
                        feeResponse.error_code = "191";
                        feeResponse.error_message = "Passenger reference required.";
                    }
                    else if (fee[i].booking_segment_id == Guid.Empty)
                    {
                        bError = true;
                        feeResponse = fee[i].MapFeeResponse();
                        feeResponse.error_code = "193";
                        feeResponse.error_message = "Segment reference required.";
                    }


                    if (objFeeResponse == null && bError == true)
                    {
                        objFeeResponse = new List<FeeResponse>();
                    }

                    if (feeResponse != null)
                    {
                        objFeeResponse.Add(feeResponse);
                    }

                    feeResponse = null;
                }
            }

            return objFeeResponse;                       
        }
        private IList<TaxResponse> TaxValitdation(IList<Tax> tax)
        {
            IList<TaxResponse> objTaxResponse = null;
            TaxResponse taxResponse = null;
            bool bError = false;
            if (tax != null)
            {
                for (int i = 0; i < tax.Count; i++)
                {
                    if (tax[i].passenger_id == Guid.Empty)
                    {
                        bError = true;
                        taxResponse = tax[i].MapTaxResponse();
                        taxResponse.error_code = "191";
                        taxResponse.error_message = "Name reference required.";
                    }
                    else if (tax[i].booking_segment_id == Guid.Empty)
                    {
                        bError = true;
                        taxResponse = tax[i].MapTaxResponse();
                        taxResponse.error_code = "193";
                        taxResponse.error_message = "Segment reference required.";
                    }

                    if (objTaxResponse == null && bError == true)
                    {
                        objTaxResponse = new List<TaxResponse>();
                    }

                    if (taxResponse != null)
                    {
                        objTaxResponse.Add(taxResponse);
                    }

                    taxResponse = null;
                }
            }
            return objTaxResponse;                       
        }
        private IList<RemarkResponse> RemarkValidation(IList<Remark> remark)
        {
            IList<RemarkResponse> objRemarkResponse = null;
            RemarkResponse remarkResponse = null;
            bool bError = false;
            if (remark != null)
            {
                for (int i = 0; i < remark.Count; i++)
                {
                    //validate

                    if (objRemarkResponse == null && bError == true)
                    {
                        objRemarkResponse = new List<RemarkResponse>();
                    }

                    if (remarkResponse != null)
                    {
                        objRemarkResponse.Add(remarkResponse);
                    }

                    remarkResponse = null;
                }
            }

            return objRemarkResponse;                               
        }
        private IList<ServiceResponse> ServiceValidation(IList<Service.Message.BookingService.Service> service)
        {
            IList<ServiceResponse> objServiceResponse = null;
            ServiceResponse serviceResponse = null;
            bool bError = false;
            if (service != null)
            {
                for (int i = 0; i < service.Count; i++)
                {
                    if (ValidServiceStatusInput(service[i].special_service_status_rcd) == false)
                    {
                        bError = true;
                        serviceResponse = service[i].MapServiceResponse();
                        serviceResponse.error_code = "164";
                        serviceResponse.error_message = "The SSR action code is not valid for the SSR service code.";
                    }
                    else if (service[i].number_of_units > 9)
                    {
                        bError = true;
                        serviceResponse = service[i].MapServiceResponse();
                        serviceResponse.error_code = "167";
                        serviceResponse.error_message = "Invalid number of services specified in SSR.";
                    }
                    else if (service[i].service_text.Length > 300)
                    {
                        bError = true;
                        serviceResponse = service[i].MapServiceResponse();
                        serviceResponse.error_code = "180";
                        serviceResponse.error_message = "The SSR free text description length is in error.";
                    }
                    else if (service[i].passenger_id == Guid.Empty)
                    {
                        bError = true;
                        serviceResponse = service[i].MapServiceResponse();
                        serviceResponse.error_code = "191";
                        serviceResponse.error_message = "Passenger reference required.";
                    }
                    else if (service[i].booking_segment_id == Guid.Empty)
                    {
                        bError = true;
                        serviceResponse = service[i].MapServiceResponse();
                        serviceResponse.error_code = "193";
                        serviceResponse.error_message = "Segment reference required.";
                    }
                    else if (ValidUnitRcd(service[i].unit_rcd) == false)
                    {
                        bError = true;
                        serviceResponse = service[i].MapServiceResponse();
                        serviceResponse.error_code = "188";
                        serviceResponse.error_message = "Invalid SSR measure unit qualifier, age or weight.";
                    }


                    if (objServiceResponse == null && bError == true)
                    {
                        objServiceResponse = new List<ServiceResponse>();
                    }

                    if (serviceResponse != null)
                    {
                        objServiceResponse.Add(serviceResponse);
                    }

                    serviceResponse = null;
                }
            }

            return objServiceResponse;
        }
        private IList<PaymentResponse> PaymentValidation(IList<Payment> payment)
        {
            IList<PaymentResponse> objPaymentResponse = null;
            PaymentResponse paymentResponse = null;
            bool bError = false;

            if (payment != null)
            {
                for (int i = 0; i < payment.Count; i++)
                {
                    //Validate

                    if (objPaymentResponse == null && bError == true)
                    {
                        objPaymentResponse = new List<PaymentResponse>();
                    }

                    if (paymentResponse != null)
                    {
                        objPaymentResponse.Add(paymentResponse);
                    }

                    paymentResponse = null;
                }
            }

            return objPaymentResponse;
        }
        private bool ValidDocumentType(string docTypeRcd)
        {
            if (docTypeRcd == "P")
            {
                return true;
            }
            else if (docTypeRcd == "DL")
            {
                return true;
            }
            else if (docTypeRcd == "V")
            {
                return true;
            }
            else if (docTypeRcd == "I")
            {
                return true;
            }
            else if (docTypeRcd == "B")
            {
                return true;
            }
            else if (string.IsNullOrEmpty(docTypeRcd))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool ValidServiceStatusInput(string serviceStatus)
        {
            if (string.IsNullOrEmpty(serviceStatus))
            {
                return false;
            }
            else if (serviceStatus == "SS")
            {
                return true;
            }
            else if (serviceStatus == "NN")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool ValidUnitRcd(string weightRcd)
        {
            if (string.IsNullOrEmpty(weightRcd))
            {
                return true;
            }
            else if (weightRcd.ToUpper() == "KGS")
            {
                return true;
            }
            else if (weightRcd.ToUpper() == "LBS")
            {
                return true;
            }
            else if (weightRcd.ToUpper() == "PC")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Avantik.Web.Service.Message.Booking.BookingFlightRequest SetAddFlightRequest(string agencyCode, string userId, string bookingId, Avantik.Web.Service.Message.Booking.BookingHeader header, IList<Avantik.Web.Service.Message.Booking.Passenger> passengers, IList<Avantik.Web.Service.Message.Booking.FlightSegment> flightSegments)
        {
            Avantik.Web.Service.Message.Booking.BookingFlightRequest wsFlightRequest = new Web.Service.Message.Booking.BookingFlightRequest();

            wsFlightRequest = new Web.Service.Message.Booking.BookingFlightRequest();

            // find passenger tppe
            short adult = 0;
            short child = 0;
            short infant = 0;

            foreach (Avantik.Web.Service.Message.Booking.Passenger p in passengers)
            {
                if (p.PassengerTypeRcd == "ADULT")
                    adult += 1;
                if (p.PassengerTypeRcd == "CHD")
                    child += 1;
                if (p.PassengerTypeRcd == "INF")
                    infant += 1;
            }

            wsFlightRequest.AgencyCode = agencyCode;
            wsFlightRequest.UserId = "{" + userId + "}";
            wsFlightRequest.Adults = adult;
            wsFlightRequest.Children = child;
            wsFlightRequest.Infants = infant;
            wsFlightRequest.BNoVat = false;
            wsFlightRequest.Currency = header.CurrencyRcd;
            wsFlightRequest.StrIpAddress = string.Empty;
            wsFlightRequest.StrLanguageCode = header.LanguageRcd;
            wsFlightRequest.StrOthers = string.Empty;
            wsFlightRequest.BookingId = "{" + bookingId + "}";
            wsFlightRequest.Flight = flightSegments.MapBookingFlight();

          //  string xx = XMLHelper.Serialize(wsFlightRequest.Flight,false);

            return wsFlightRequest;
        }
        private Avantik.Web.Service.Message.Booking.BookingSaveRequest MapFlightResponseToSaveRequest(Avantik.Web.Service.Message.Booking.BookingSaveRequest wsSaveRequest, Avantik.Web.Service.Message.Booking.BookingFlightResponse wsFlightResponse)
        {
            // set mapping
            #region map mapping
            foreach (Avantik.Web.Service.Message.Booking.Mapping mappingFlightResponse in wsFlightResponse.BookFlightResponse.Mappings)
            {
                foreach (Avantik.Web.Service.Message.Booking.Mapping mappingSaveRequest in wsSaveRequest.Booking.Mappings)
                {
                    //ArrayList alMapping = new ArrayList();

                    if ((mappingFlightResponse.OriginRcd == mappingSaveRequest.OriginRcd) &&
                    mappingFlightResponse.DestinationRcd == mappingSaveRequest.DestinationRcd &&
                    mappingFlightResponse.FlightNumber == mappingSaveRequest.FlightNumber &&
                    mappingFlightResponse.AirlineRcd == mappingSaveRequest.AirlineRcd &&
                    mappingFlightResponse.DepartureDate == mappingSaveRequest.DepartureDate)
                   // !alMapping.Contains(mappingSaveRequest.BookingSegmentId))
                    {

                       // alMapping.Add(mappingSaveRequest.BookingSegmentId);

                        // fill FlightId to save request segment
                        mappingSaveRequest.FlightId = mappingFlightResponse.FlightId;
                        mappingSaveRequest.BoardingClassRcd = mappingFlightResponse.BoardingClassRcd;
                        mappingSaveRequest.PassengerStatusRcd = mappingFlightResponse.PassengerStatusRcd;
                        mappingSaveRequest.OdDestinationRcd = mappingFlightResponse.OdDestinationRcd;
                        mappingSaveRequest.OdOriginRcd = mappingFlightResponse.OdOriginRcd;
                    }
                }
            }
            #endregion

            #region Map PNR with passenger
            // Map PNR with passenger
            foreach (Avantik.Web.Service.Message.Booking.Passenger passengerFlightReponse in wsFlightResponse.BookFlightResponse.Passengers)
            {//
                foreach (Avantik.Web.Service.Message.Booking.Passenger passengerSaveRequest in wsSaveRequest.Booking.Passengers)
                {
                    ArrayList alPassenger = new ArrayList();

                    if ((passengerFlightReponse.PassengerTypeRcd == passengerSaveRequest.PassengerTypeRcd) && !alPassenger.Contains(passengerFlightReponse.PassengerId))
                    {
                        Guid passIdFlightResponse = Guid.Empty;
                        Guid passIdSaveRequest = Guid.Empty;

                        passIdFlightResponse = passengerFlightReponse.PassengerId;
                        passIdSaveRequest = passengerSaveRequest.PassengerId;

                        alPassenger.Add(passengerFlightReponse.PassengerId);

                        //fill new id to fee
                        if (wsFlightResponse.BookFlightResponse.Fees != null)
                        {
                            foreach (Avantik.Web.Service.Message.Booking.Fee f in wsFlightResponse.BookFlightResponse.Fees)
                            {
                                if (f.PassengerId == passIdFlightResponse)
                                {
                                    f.PassengerId = passIdSaveRequest;
                                }
                            }
                        }

                    }// end if

                }

            }//
            #endregion

            #region flightsegment
            foreach (Avantik.Web.Service.Message.Booking.FlightSegment segmentFlightResponse in wsFlightResponse.BookFlightResponse.FlightSegments)
            {//
                foreach (Avantik.Web.Service.Message.Booking.FlightSegment segmentSaveRequest in wsSaveRequest.Booking.FlightSegments)
                {
                    ArrayList alFlightSegment = new ArrayList();

                    if ((segmentFlightResponse.OriginRcd == segmentSaveRequest.OriginRcd) &&
                    segmentFlightResponse.DestinationRcd == segmentSaveRequest.DestinationRcd &&
                    segmentFlightResponse.FlightNumber == segmentSaveRequest.FlightNumber &&
                    segmentFlightResponse.AirlineRcd == segmentSaveRequest.AirlineRcd &&
                    segmentFlightResponse.DepartureDate == segmentSaveRequest.DepartureDate &&
                    !alFlightSegment.Contains(segmentFlightResponse.BookingSegmentId))
                    {
                        Guid segmentIdFlightResponse = Guid.Empty;
                        Guid segmentIdSaveRequest = Guid.Empty;

                        segmentIdFlightResponse = segmentFlightResponse.BookingSegmentId;
                        segmentIdSaveRequest = segmentSaveRequest.BookingSegmentId;

                        alFlightSegment.Add(segmentFlightResponse.BookingSegmentId);

                        // fill FlightId to save request segment
                        segmentSaveRequest.FlightId = segmentFlightResponse.FlightId;
                        segmentSaveRequest.DepartureTime = segmentFlightResponse.DepartureTime;
                        segmentSaveRequest.ArrivalTime = segmentFlightResponse.ArrivalTime;
                        segmentSaveRequest.BoardingClassRcd = segmentFlightResponse.BoardingClassRcd;

                        //fill new id to fee
                        if (wsFlightResponse.BookFlightResponse.Fees != null)
                        {
                            foreach (Avantik.Web.Service.Message.Booking.Fee f in wsFlightResponse.BookFlightResponse.Fees)
                            {
                                if (f.BookingSegmentId == segmentIdFlightResponse)
                                {
                                    f.BookingSegmentId = segmentIdSaveRequest;
                                }

                            }
                        }


                    }
                }
            }//

            #endregion

            // concate  PNRNEW to fee save request
            if (wsFlightResponse.BookFlightResponse.Fees != null && wsSaveRequest.Booking.Fees != null)
            {
                wsSaveRequest.Booking.Fees = wsSaveRequest.Booking.Fees.Concat(wsFlightResponse.BookFlightResponse.Fees).ToList();
            }
            else if (wsFlightResponse.BookFlightResponse.Fees != null)
            {
                wsSaveRequest.Booking.Fees = wsFlightResponse.BookFlightResponse.Fees;
            }

            return wsSaveRequest;
        }

        #endregion
    }
}
