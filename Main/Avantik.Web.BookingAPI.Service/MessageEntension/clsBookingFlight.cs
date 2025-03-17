using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Avantik.Web.BookingAPI.Service.Message.BookingService;

namespace Avantik.Web.BookingAPI.Service.MessageEntension
{
    public static class BookingFlight
    {

        #region mapping save reponse  ws object
        public static IList<Avantik.Web.Service.Message.Booking.Flight> MapBookingFlight(this  IList<Avantik.Web.Service.Message.Booking.FlightSegment> objBooking)
        {
            IList<Avantik.Web.Service.Message.Booking.Flight> objResponseList = new List<Avantik.Web.Service.Message.Booking.Flight>();
            if (objBooking != null)
            {
                for (int i = 0; i < objBooking.Count; i++)
                {
                    objResponseList.Add(objBooking[i].MapBookingFlight());
                }
            }
            return objResponseList;
        }
        public static Avantik.Web.Service.Message.Booking.Flight MapBookingFlight(this  Avantik.Web.Service.Message.Booking.FlightSegment objBooking)
        {
            Avantik.Web.Service.Message.Booking.Flight objResponse = new Avantik.Web.Service.Message.Booking.Flight();

            if (objBooking != null)
            {
                objResponse.BoardingClassRcd = objBooking.BoardingClassRcd; // "Y"; //
                objResponse.BookingClassRcd = objBooking.BookingClassRcd; //"E";//
                objResponse.DepartureDate = objBooking.DepartureDate; //new DateTime(2014, 08, 29);// 
                objResponse.OdOriginRcd = objBooking.OdOriginRcd; //"XCN"; 
                objResponse.OdDestinationRcd = objBooking.OdDestinationRcd; //"LWY";
                objResponse.FlightConnectionId = objBooking.FlightConnectionId;
                objResponse.DestinationRcd = objBooking.DestinationRcd; // "LWY"; 
                objResponse.OriginRcd = objBooking.OriginRcd; //"XCN"; 
                objResponse.AirlineRcd = objBooking.AirlineRcd;// "AB";
                objResponse.FlightNumber = objBooking.FlightNumber;// "220";
                objResponse.NumberOfUnits = objBooking.NumberOfUnits; // number of passenger on flight.
                objResponse.FlightId = objBooking.FlightId;

            }

            return objResponse;
        }

        #endregion

        public static void MapPassengerObject(this  Avantik.Web.Service.Message.Booking.Passenger passengerSaveRequest, Avantik.Web.Service.Message.Booking.Passenger passengerFlightReponse)
        {
            if (passengerSaveRequest != null)
            {
                passengerFlightReponse.TitleRcd = passengerSaveRequest.TitleRcd;
                passengerFlightReponse.Lastname = passengerSaveRequest.Lastname;
                passengerFlightReponse.Firstname = passengerSaveRequest.Firstname;
                passengerFlightReponse.Middlename = passengerSaveRequest.Middlename;
                passengerFlightReponse.NationalityRcd = passengerSaveRequest.NationalityRcd;
                passengerFlightReponse.PassengerWeight = passengerSaveRequest.PassengerWeight;
                passengerFlightReponse.GenderTypeRcd = passengerSaveRequest.GenderTypeRcd;
                passengerFlightReponse.PassengerTypeRcd = passengerSaveRequest.PassengerTypeRcd;
                passengerFlightReponse.ClientProfileId = passengerSaveRequest.ClientProfileId;
                passengerFlightReponse.ClientNumber = passengerSaveRequest.ClientNumber;

                passengerFlightReponse.AddressLine1 = passengerSaveRequest.AddressLine1;
                passengerFlightReponse.AddressLine2 = passengerSaveRequest.AddressLine2;
                passengerFlightReponse.State = passengerSaveRequest.State;
                passengerFlightReponse.District = passengerSaveRequest.District;
                passengerFlightReponse.Province = passengerSaveRequest.Province;
                passengerFlightReponse.ZipCode = passengerSaveRequest.ZipCode;
                passengerFlightReponse.PoBox = passengerSaveRequest.PoBox;
                passengerFlightReponse.CountryRcd = passengerSaveRequest.CountryRcd;
                passengerFlightReponse.Street = passengerSaveRequest.Street;
                passengerFlightReponse.City = passengerSaveRequest.City;

                passengerFlightReponse.DocumentTypeRcd = passengerSaveRequest.DocumentTypeRcd;
                passengerFlightReponse.DocumentNumber = passengerSaveRequest.DocumentNumber;
                passengerFlightReponse.ResidenceCountryRcd = passengerSaveRequest.ResidenceCountryRcd;
                passengerFlightReponse.PassportNumber = passengerSaveRequest.PassportNumber;
                passengerFlightReponse.PassportIssueDate = passengerSaveRequest.PassportIssueDate;
                passengerFlightReponse.PassportExpiryDate = passengerSaveRequest.PassportExpiryDate;
                passengerFlightReponse.PassportIssuePlace = passengerSaveRequest.PassportIssuePlace;
                passengerFlightReponse.PassportBirthPlace = passengerSaveRequest.PassportBirthPlace;
                passengerFlightReponse.DateOfBirth = passengerSaveRequest.DateOfBirth;
                passengerFlightReponse.PassportIssueCountryRcd = passengerSaveRequest.PassportIssueCountryRcd;

                passengerFlightReponse.ContactName = passengerSaveRequest.ContactName;
                passengerFlightReponse.ContactEmail = passengerSaveRequest.ContactEmail;
                passengerFlightReponse.MobileEmail = passengerSaveRequest.MobileEmail;
                passengerFlightReponse.PhoneMobile = passengerSaveRequest.PhoneMobile;
                passengerFlightReponse.PhoneHome = passengerSaveRequest.PhoneHome;
                passengerFlightReponse.PhoneFax = passengerSaveRequest.PhoneFax;
                passengerFlightReponse.PhoneBusiness = passengerSaveRequest.PhoneBusiness;
                passengerFlightReponse.ReceivedFrom = passengerSaveRequest.ReceivedFrom;

                passengerFlightReponse.CreateBy = passengerSaveRequest.CreateBy;
                passengerFlightReponse.CreateDateTime = passengerSaveRequest.CreateDateTime;
                passengerFlightReponse.UpdateBy = passengerSaveRequest.UpdateBy;
                passengerFlightReponse.UpdateDateTime = passengerSaveRequest.UpdateDateTime;

                passengerFlightReponse.EmployeeNumber = passengerSaveRequest.EmployeeNumber;
                passengerFlightReponse.PassengerRoleRcd = passengerSaveRequest.PassengerRoleRcd;
                passengerFlightReponse.MemberLevelRcd = passengerSaveRequest.MemberLevelRcd;
                passengerFlightReponse.MemberNumber = passengerSaveRequest.MemberNumber;
                passengerFlightReponse.RedressNumber = passengerSaveRequest.RedressNumber;
                passengerFlightReponse.PnrName = passengerSaveRequest.PnrName;
                passengerFlightReponse.WheelchairFlag = passengerSaveRequest.WheelchairFlag;
                passengerFlightReponse.VipFlag = passengerSaveRequest.VipFlag;
                passengerFlightReponse.WindowSeatFlag = passengerSaveRequest.WindowSeatFlag;


            }

        }


    }
         

}
