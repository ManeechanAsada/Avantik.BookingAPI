using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avantik.Web.Service;
using Avantik.Web.BookingAPI.Service.Message.BookingService;

namespace Avantik.Web.BookingAPI.Service.MessageEntension
{
    public static class APISaveRequestWsSaveRequestMapping
    {
        public static Avantik.Web.Service.Message.Booking.BookingHeader FillObjectBookingRequest(this  BookingHeader header, Guid bookingId,string agencyCode,Guid userId)
        {
            Avantik.Web.Service.Message.Booking.BookingHeader bookingheader = null;

            if (header != null)
            {
                bookingheader =new Avantik.Web.Service.Message.Booking.BookingHeader();

                bookingheader.BookingId = bookingId;

                if (header.agency_code == null || header.agency_code.ToString() == string.Empty)
                {
                    bookingheader.AgencyCode = agencyCode;
                }
                else
                {
                    bookingheader.AgencyCode = header.agency_code;
                }

                bookingheader.CurrencyRcd = header.currency_rcd;

                if (!string.IsNullOrEmpty(header.booking_number))
                    bookingheader.BookingNumber = Convert.ToInt64(header.booking_number);

                bookingheader.LanguageRcd = header.language_rcd;
                bookingheader.ContactName = header.contact_name;
                bookingheader.ContactEmail = header.contact_email;
                bookingheader.ReceivedFrom = header.received_from;
                bookingheader.PhoneSearch = header.phone_search;
                bookingheader.Comment = header.comment;
                bookingheader.TitleRcd = header.title_rcd;
                bookingheader.Lastname = header.lastname;
                bookingheader.Firstname = header.firstname;
                bookingheader.Middlename = header.middlename;
                bookingheader.CountryRcd = header.country_rcd;
                bookingheader.GroupBookingFlag = header.group_booking_flag;

                bookingheader.CreateBy = userId;
                bookingheader.CreateDateTime=   DateTime.Now;
                bookingheader.UpdateBy = userId;
                bookingheader.UpdateDateTime = DateTime.Now;

                if (header.address != null)
                {
                    bookingheader.AddressLine1 = header.address.address_line1;
                    bookingheader.AddressLine2 = header.address.address_line2;
                    bookingheader.Street = header.address.street;
                    bookingheader.PoBox = header.address.po_box;
                    bookingheader.City = header.address.city;
                    bookingheader.State = header.address.state;
                    bookingheader.District = header.address.district;
                    bookingheader.Province = header.address.province;
                    bookingheader.ZipCode = header.address.zip_code;
                    bookingheader.CountryRcd = header.address.country_rcd;
                }
                if (header.telephone != null)
                {
                    bookingheader.PhoneMobile = header.telephone.phone_mobile;
                    bookingheader.PhoneHome = header.telephone.phone_home;
                    bookingheader.PhoneBusiness = header.telephone.phone_business;
                    bookingheader.PhoneFax = header.telephone.phone_fax;
                }
            }

            return bookingheader;
        }
        public static IList<Avantik.Web.Service.Message.Booking.FlightSegment> FillObjectBookingRequest(this  IList<FlightSegment> objMessage, Guid bookingId, Guid userId)
        {
            IList<Avantik.Web.Service.Message.Booking.FlightSegment> segmentList = null;
            if (objMessage != null)
            {
                segmentList = new List<Avantik.Web.Service.Message.Booking.FlightSegment>();
                for (int i = 0; i < objMessage.Count; i++)
                {
                    segmentList.Add(objMessage[i].FillObjectBookingRequest(bookingId,userId));
                }
            }
            return segmentList;
        }
        public static Avantik.Web.Service.Message.Booking.FlightSegment FillObjectBookingRequest(this  FlightSegment segment, Guid bookingId,Guid userId)
        {
            Avantik.Web.Service.Message.Booking.FlightSegment flightSegment = null;

            if (segment != null)
            {
                flightSegment = new Avantik.Web.Service.Message.Booking.FlightSegment();
                flightSegment.BookingId = bookingId;

                flightSegment.BookingSegmentId =segment.booking_segment_id;
                flightSegment.FlightConnectionId = segment.flight_connection_id;

                flightSegment.DepartureDate = segment.departure_date;

                flightSegment.AirlineRcd = segment.airline_rcd;
                flightSegment.FlightNumber = segment.flight_number;
                flightSegment.OriginRcd = segment.origin_rcd;
                flightSegment.DestinationRcd = segment.destination_rcd;
                flightSegment.OdOriginRcd = segment.od_origin_rcd;
                flightSegment.OdDestinationRcd = segment.od_destination_rcd;
                flightSegment.BookingClassRcd = segment.booking_class_rcd;
                flightSegment.BoardingClassRcd = segment.boarding_class_rcd;
                flightSegment.SegmentStatusRcd = segment.segment_status_rcd == "NN" ? "HK" : segment.segment_status_rcd;

                //Use for passenger count in the flight.
                flightSegment.NumberOfUnits = segment.number_of_units;

                flightSegment.CreateBy = userId;
                flightSegment.CreateDateTime = DateTime.Now;
                flightSegment.UpdateBy = userId;
                flightSegment.UpdateDateTime = DateTime.Now;

                flightSegment.FlightId = segment.flight_id;
            }

            return flightSegment;
        }
        public static IList<Avantik.Web.Service.Message.Booking.Passenger> FillObjectBookingRequest(this  IList<Passenger> objMessage, Guid bookingId, Guid userId)
        {
            IList<Avantik.Web.Service.Message.Booking.Passenger> passengertList = null;
            if (objMessage != null)
            {
                passengertList = new List<Avantik.Web.Service.Message.Booking.Passenger>();
                for (int i = 0; i < objMessage.Count; i++)
                {
                    passengertList.Add(objMessage[i].FillObjectBookingRequest(bookingId,userId));
                }
            }
            return passengertList;
        }
        public static Avantik.Web.Service.Message.Booking.Passenger FillObjectBookingRequest(this  Passenger p, Guid bookingId, Guid userId)
        {
            Avantik.Web.Service.Message.Booking.Passenger passenger = null;

            if (p != null)
            {
                passenger = new Avantik.Web.Service.Message.Booking.Passenger();

                passenger.BookingId = bookingId;
                passenger.PassengerId = p.passenger_id;
                //optional use only when want to defind guardian of passenger type CHD
                passenger.GuardianPassengerId = p.guardian_passenger_id;

                passenger.DateOfBirth = p.date_of_birth;

                if (!string.IsNullOrEmpty(p.client_number))
                    passenger.ClientNumber = Convert.ToInt64(p.client_number);

                passenger.PassengerTypeRcd = p.passenger_type_rcd;
                passenger.Lastname = p.lastname;
                passenger.Firstname = p.firstname;
                passenger.Middlename = p.middlename;
                passenger.TitleRcd = p.title_rcd;
                passenger.MemberNumber = p.member_number;
                passenger.MemberAirlineRcd = p.member_airline_rcd;
                passenger.MemberLevelRcd = p.member_level_rcd;
                passenger.GenderTypeRcd = p.gender_type_rcd;
                passenger.NationalityRcd = p.nationality_rcd;
                passenger.ResidenceCountryRcd = p.residence_country_rcd;
                passenger.RedressNumber = p.redress_number;
                passenger.KnownTravelerNumber = p.known_traveler_number;

                //EDWDEV-81
                //passenger.PnrFirstName = p.firstname;
                //passenger.PnrLastName = p.lastname;

                passenger.VipFlag = p.vip_flag;

                passenger.PassengerWeight = p.passenger_weight;

                passenger.CreateBy = userId;
                passenger.CreateDateTime = DateTime.Now;
                passenger.UpdateBy = userId;
                passenger.UpdateDateTime = DateTime.Now;

                //Document
                if(p.Document != null)
                {
                    passenger.PassportNumber = p.Document.passport_number;
                    passenger.DocumentTypeRcd = p.Document.document_type_rcd;
                    passenger.PassportIssuePlace = p.Document.passport_issue_place;
                    passenger.PassportBirthPlace = p.Document.passport_birth_place;
                    passenger.PassportIssueCountryRcd = p.Document.passport_issue_country_rcd;
                    passenger.CountryCodeLong = p.Document.country_code_long;
                    passenger.PassportIssueDate = p.Document.passport_issue_date;
                    passenger.PassportExpiryDate = p.Document.passport_expiry_date;
                }
            }

            return passenger;
        }
        public static IList<Avantik.Web.Service.Message.Booking.Mapping> FillObjectBookingRequest(this  IList<Mapping> objMessage, Guid booking_id, Guid userId, string agencyCode)
        {
            IList<Avantik.Web.Service.Message.Booking.Mapping> mappingtList = null;
            if (objMessage != null)
            {
                mappingtList = new List<Avantik.Web.Service.Message.Booking.Mapping>();
                for (int i = 0; i < objMessage.Count; i++)
                {
                    mappingtList.Add(objMessage[i].FillObjectBookingRequest(booking_id,userId, agencyCode));
                }
            }
            return mappingtList;
        }
        public static Avantik.Web.Service.Message.Booking.Mapping FillObjectBookingRequest(this  Mapping m, Guid booking_id, Guid userId, string agencyCode)
        {
            Avantik.Web.Service.Message.Booking.Mapping mapping = null;

            if (m != null)
            {
                mapping = new Avantik.Web.Service.Message.Booking.Mapping();

                mapping.AgencyCode = agencyCode;
                mapping.BookingId = booking_id;
                mapping.BookingSegmentId = m.booking_segment_id;
                mapping.PassengerId = m.passenger_id;

                mapping.NotValidBeforeDate = m.not_valid_before_date;
                mapping.NotValidAfterDate = m.not_valid_after_date;

                mapping.RefundWithChargeHours = m.refund_with_charge_hours;
                mapping.RefundNotPossibleHours = m.refund_not_possible_hours;

                mapping.PieceAllowance = m.piece_allowance;

                mapping.FareCode = m.fare_code;
                if (string.IsNullOrEmpty(m.seat_number) == false && m.seat_number.Length > 1)
                {
                    mapping.SeatColumn = m.seat_number.Substring(m.seat_number.Length - 1, 1);
                    mapping.SeatRow = Convert.ToInt16(m.seat_number.Substring(0, m.seat_number.Length - 1));
                }
                mapping.SeatNumber = m.seat_number;
                mapping.CurrencyRcd = m.currency_rcd;
                mapping.EndorsementText = m.endorsement_text;
                mapping.RestrictionText = m.restriction_text;

                mapping.FareAmount = m.fare_amount;
                mapping.FareAmountIncl = m.fare_amount_incl;
                mapping.FareVat = m.fare_vat;
                mapping.NetTotal = m.net_total;
                //Optinal
                mapping.CommissionAmount = m.commission_amount;
                //Optinal
                mapping.CommissionAmountIncl = m.commission_amount_incl;
                //Optinal
                mapping.CommissionPercentage = m.commission_percentage;
                //Optinal
                mapping.PublicFareAmount = m.public_fare_amount;
                //Optinal
                mapping.PublicFareAmountIncl = m.public_fare_amount_incl;
                mapping.RefundCharge = m.refund_charge;
                mapping.BaggageWeight = m.baggage_weight;
                mapping.RedemptionPoints = m.redemption_points;

                mapping.ETicketFlag = m.e_ticket_flag;
                mapping.RefundableFlag = m.refundable_flag;
                mapping.TransferableFareFlag = m.transferable_fare_flag;
                mapping.ThroughFareFlag = m.through_fare_flag;
                mapping.ItFareFlag = m.it_fare_flag;
                mapping.DutyTravelFlag = m.duty_travel_flag;
                mapping.StandbyFlag = m.standby_flag;
                mapping.ExcludePricingFlag = m.exclude_pricing_flag;

                mapping.CreateBy = userId;
                mapping.CreateDateTime = DateTime.Now;
                mapping.UpdateBy = userId;
                mapping.UpdateDateTime = DateTime.Now;
            }

            return mapping;
        }

        public static IList<Avantik.Web.Service.Message.Booking.Fee> FillObjectBookingRequest(this  IList<Fee> objMessage, Guid bookingId, Guid userId, string agencyCode)
        {
            IList<Avantik.Web.Service.Message.Booking.Fee> feeList = null;
            if (objMessage != null)
            {
                feeList = new List<Avantik.Web.Service.Message.Booking.Fee>();
                for (int i = 0; i < objMessage.Count; i++)
                {
                    feeList.Add(objMessage[i].FillObjectBookingRequest(bookingId,userId, agencyCode));
                }
            }
            return feeList;
        }
        public static Avantik.Web.Service.Message.Booking.Fee FillObjectBookingRequest(this  Fee f, Guid bookingId,Guid userId,string agencyCode)
        {
            Avantik.Web.Service.Message.Booking.Fee fee = null;

            if (f != null)
            {
                fee = new Avantik.Web.Service.Message.Booking.Fee();

                fee.BookingId = bookingId;

                fee.PassengerId = f.passenger_id;
                fee.BookingSegmentId =f.booking_segment_id;
                fee.PassengerSegmentServiceId = Guid.NewGuid(); //f.passenger_segment_service_id;

                
                if (f.fee_id != Guid.Empty)
                    fee.BookingFeeId = f.fee_id;//Guid.NewGuid();
                else
                    fee.BookingFeeId = Guid.NewGuid();

                // Remove assign FeeId
                //fee.FeeId = f.fee_id;

    

                //if (f.fee_id != Guid.Empty)
                //    fee.BookingFeeId = Guid.NewGuid();
                //else
               // //    fee.BookingFeeId = f.fee_id;
               // fee.FeeId = f.fee_id;
               // fee.BookingFeeId =  Guid.NewGuid();
                fee.FeeRcd = f.fee_rcd;
                fee.VendorRcd = f.vendor_rcd;
                fee.OdOriginRcd = f.od_origin_rcd;
                fee.OdDestinationRcd = f.od_destination_rcd;
                fee.CurrencyRcd = f.currency_rcd;
                fee.ChargeCurrencyRcd = f.charge_currency_rcd;
                fee.UnitRcd = f.unit_rcd;
                fee.MpdNumber = f.mpd_number;
                fee.ChangeComment = f.comment;
                fee.ExternalReference = f.external_reference;

                fee.FeeAmount = f.fee_amount;
                fee.FeeAmountIncl = f.fee_amount_incl;
                fee.VatPercentage = f.vat_percentage;
                fee.ChargeAmount = f.charge_amount;
                fee.ChargeAmountIncl = f.charge_amount_incl;
                fee.WeightLbs = f.weight_lbs;
                fee.WeightKgs = f.weight_kgs;
                fee.NumberOfUnits = f.number_of_units;

                fee.CreateBy = userId;
                fee.CreateDateTime = DateTime.Now;
                fee.UpdateBy = userId;
                fee.UpdateDateTime = DateTime.Now;

                fee.AgencyCode = agencyCode;

            }

            return fee;
        }
        public static IList<Avantik.Web.Service.Message.Booking.Tax> FillObjectBookingRequest(this  IList<Tax> objMessage, Guid userId)
        {
            IList<Avantik.Web.Service.Message.Booking.Tax> taxList = null;
            if (objMessage != null)
            {
                taxList = new List<Avantik.Web.Service.Message.Booking.Tax>();
                for (int i = 0; i < objMessage.Count; i++)
                {
                    taxList.Add(objMessage[i].FillObjectBookingRequest(userId));
                }
            }
            return taxList;
        }
        public static Avantik.Web.Service.Message.Booking.Tax FillObjectBookingRequest(this  Tax tx, Guid userId)
        {
            Avantik.Web.Service.Message.Booking.Tax tax = null;

            if (tx != null)
            {
                tax = new Avantik.Web.Service.Message.Booking.Tax();

                tax.PassengerId = tx.passenger_id;
                tax.BookingSegmentId = tx.booking_segment_id;

                tax.TaxRcd = tx.tax_rcd;
                tax.TaxCurrencyRcd = tx.tax_currency_rcd;
                tax.SalesCurrencyRcd = tx.sales_currency_rcd;

                tax.SalesAmount = tx.sales_amount;
                tax.SalesAmountIncl = tx.sales_amount_incl;
                tax.TaxAmount = tx.tax_amount;
                tax.TaxAmountIncl = tx.tax_amount_incl;

                tax.CreateBy = userId;
                tax.CreateDateTime = DateTime.Now;
                tax.UpdateBy = userId;
                tax.UpdateDateTime = DateTime.Now;

            }

            return tax;
        }
        public static IList<Avantik.Web.Service.Message.Booking.Remark> FillObjectBookingRequest(this  IList<Remark> objMessage, Guid bookingId,Guid userId)
        {
            IList<Avantik.Web.Service.Message.Booking.Remark> remarkList = null;
            if (objMessage != null)
            {
                remarkList = new List<Avantik.Web.Service.Message.Booking.Remark>();
                for (int i = 0; i < objMessage.Count; i++)
                {
                    remarkList.Add(objMessage[i].FillObjectBookingRequest(bookingId,userId));
                }
            }
            return remarkList;
        }
        public static Avantik.Web.Service.Message.Booking.Remark FillObjectBookingRequest(this  Remark re,  Guid bookingId,Guid userId)
        {
            Avantik.Web.Service.Message.Booking.Remark remark = null;

            if (re != null)
            {
                remark = new Avantik.Web.Service.Message.Booking.Remark();

                remark.BookingId = bookingId;

                remark.TimelimitDateTime = re.timelimit_date_time;
                remark.UtcTimelimitDateTime = re.utc_timelimit_date_time;

                remark.RemarkTypeRcd = re.remark_type_rcd;
                remark.RemarkText = re.remark_text;
                remark.Nickname = re.nickname;

                remark.CreateBy = userId;
                remark.CreateDateTime = DateTime.Now;
                remark.UpdateBy = userId;
                remark.UpdateDateTime = DateTime.Now;

            }

            return remark;
        }
        public static IList<Avantik.Web.Service.Message.Booking.PassengerService> FillObjectBookingRequest(this  IList<Avantik.Web.BookingAPI.Service.Message.BookingService.Service> objMessage, Guid userId)
        {
            IList<Avantik.Web.Service.Message.Booking.PassengerService> serviceList = null;
            if (objMessage != null)
            {
                serviceList = new List<Avantik.Web.Service.Message.Booking.PassengerService>();
                for (int i = 0; i < objMessage.Count; i++)
                {
                    serviceList.Add(objMessage[i].FillObjectBookingRequest(userId));
                }
            }
            return serviceList;
        }
        public static Avantik.Web.Service.Message.Booking.PassengerService FillObjectBookingRequest(this  Avantik.Web.BookingAPI.Service.Message.BookingService.Service ser, Guid userId)
        {
            Avantik.Web.Service.Message.Booking.PassengerService service = null;

            if (ser != null)
            {
                service = new Avantik.Web.Service.Message.Booking.PassengerService();

                service.PassengerSegmentServiceId = ser.passenger_segment_service_id;
                service.PassengerId = ser.passenger_id;
                service.BookingSegmentId = ser.booking_segment_id;

                service.NumberOfUnits = ser.number_of_units;

                service.SpecialServiceRcd = ser.special_service_rcd;
                service.ServiceText = ser.service_text;
                //The value should be SS or NN
                service.SpecialServiceStatusRcd = ser.special_service_status_rcd;
                service.UnitRcd = ser.unit_rcd;

                service.CreateBy = userId;
                service.CreateDateTime = DateTime.Now;
                service.UpdateBy = userId;
                service.UpdateDateTime = DateTime.Now;


            }

            return service;
        }
        public static IList<Avantik.Web.Service.Message.Booking.Payment> FillObjectBookingRequest(this  IList<Payment> objMessage, Guid bookingId,string agencyCode,Guid userId)
        {
            IList<Avantik.Web.Service.Message.Booking.Payment> paymentList = null;
            if (objMessage != null)
            {
                paymentList = new List<Avantik.Web.Service.Message.Booking.Payment>();
                for (int i = 0; i < objMessage.Count; i++)
                {
                    paymentList.Add(objMessage[i].FillObjectBookingRequest(bookingId,agencyCode,userId));
                }
            }
            return paymentList;
        }
        public static Avantik.Web.Service.Message.Booking.Payment FillObjectBookingRequest(this  Payment pay,  Guid bookingId,string agencyCode,Guid userId)
        {
            Avantik.Web.Service.Message.Booking.Payment payment = null;

            if (pay != null)
            {
                payment = new Avantik.Web.Service.Message.Booking.Payment();

                payment.BookingId = bookingId;
                //If not passin use server time.
                payment.PaymentDateTime = pay.payment_date_time;

                //Electronic fund transfer information.

                //Credit card information.

                payment.PaymentAmount = pay.payment_amount;
                payment.ReceivePaymentAmount = pay.receive_payment_amount;

                //Value will be SALE or REFUND
                payment.PaymentTypeRcd = pay.payment_type_rcd;
                payment.FormOfPaymentRcd = pay.form_of_payment_rcd;
                payment.FormOfPaymentSubtypeRcd = pay.form_of_payment_subtype_rcd;

                if(string.IsNullOrEmpty(payment.AgencyCode))
                    payment.AgencyCode = agencyCode;
                else
                    payment.AgencyCode = payment.AgencyCode;

                payment.DebitAgencyCode = pay.debit_agency_code;

                payment.CurrencyRcd = pay.currency_rcd;

                payment.ReceiveCurrencyRcd = pay.receive_currency_rcd;


                payment.ApprovalCode = pay.approval_code;
                payment.TransactionReference = pay.transaction_reference;
                payment.PaymentNumber = pay.payment_number;
                //EDWDEV-301
                //EDWIBENEW
                //payment.PaymentReference = pay.payment_reference + "|" + pay.transaction_reference;
                payment.PaymentReference = pay.payment_reference ;
                payment.PaymentRemark = pay.payment_remark;

                // credit card
                if (pay.credit_card != null)
                {
                    payment.DocumentNumber = pay.credit_card.credit_card_number;
                    payment.NameOnCard = pay.credit_card.name_on_card;
                    payment.CvvCode = pay.credit_card.cvv_code;
                    payment.IssueNumber = pay.credit_card.issue_number;

                    payment.ExpiryMonth = pay.credit_card.expiry_month;
                    payment.ExpiryYear = pay.credit_card.expiry_year;
                    payment.IssueMonth = pay.credit_card.issue_month;
                    payment.IssueYear = pay.credit_card.issue_year;
                }

                // address
                if (pay.credit_card != null && pay.credit_card.Address != null)
                {
                    payment.AddressLine1 = pay.credit_card.Address.address_line1;
                    payment.AddressLine2 = pay.credit_card.Address.address_line2;
                    payment.Street = pay.credit_card.Address.street;
                    payment.PoBox = pay.credit_card.Address.po_box;
                    payment.City = pay.credit_card.Address.city;
                    payment.State = pay.credit_card.Address.state;
                    payment.District = pay.credit_card.Address.district;
                    payment.Province = pay.credit_card.Address.province;
                    payment.ZipCode = pay.credit_card.Address.zip_code;
                    payment.CountryRcd = pay.credit_card.Address.country_rcd;
                }

                payment.CreateBy = userId;
                payment.PaymentBy = userId;
                payment.UpdateBy = userId;


                if (payment.CreateDateTime == DateTime.MinValue)
                    payment.CreateDateTime = DateTime.Now;
                else
                    payment.CreateDateTime = payment.CreateDateTime;


                if (payment.UpdateDateTime == DateTime.MinValue)
                    payment.UpdateDateTime = DateTime.Now;
                else
                    payment.UpdateDateTime = payment.UpdateDateTime;

            }

            return payment;
        }
        public static IList<Avantik.Web.Service.Message.Booking.Mapping> FillMapping(this  IList<Passenger> passtList, IList<Avantik.Web.Service.Message.Booking.Mapping> m)
        {

            if (passtList != null && m != null )
            {
                for (int i = 0; i < passtList.Count; i++)
                {
                    for (int j = 0; j < m.Count; j++)
                    {
                        if (passtList[i].passenger_id == m[j].PassengerId)
                            passtList[i].FillMapping(m[j]);
                    }
                }
            }
            return m;
        }
        public static Avantik.Web.Service.Message.Booking.Mapping FillMapping(this  Passenger passenger, Avantik.Web.Service.Message.Booking.Mapping mapping)
        {
            if (passenger != null && mapping != null)
            {
                mapping.Lastname = passenger.lastname;
                mapping.Firstname = passenger.firstname;
                mapping.PassengerTypeRcd = passenger.passenger_type_rcd;
                mapping.TitleRcd = passenger.title_rcd;
                mapping.GenderTypeRcd = passenger.gender_type_rcd;
                mapping.DateOfBirth = passenger.date_of_birth;
            }
       
            return mapping;
        }

        public static IList<Avantik.Web.Service.Message.Booking.Mapping> FillMapping(this  IList<FlightSegment> segList, IList<Avantik.Web.Service.Message.Booking.Mapping> m)
        {

            if (segList != null && m != null )
            {
                for (int i = 0; i < segList.Count; i++)
                {
                    for (int j = 0; j < m.Count; j++)
                    {
                        if (segList[i].booking_segment_id == m[j].BookingSegmentId)
                            segList[i].FillMapping(m[j]);
                    }
                }
            }
            return m;
        }
        public static Avantik.Web.Service.Message.Booking.Mapping FillMapping(this  FlightSegment segment, Avantik.Web.Service.Message.Booking.Mapping mapping)
        {
            if (segment != null && mapping != null)
            {
                mapping.AirlineRcd = segment.airline_rcd;
                mapping.FlightNumber = segment.flight_number;
                mapping.DepartureDate = segment.departure_date;
                mapping.BoardingClassRcd = segment.boarding_class_rcd;
                mapping.BookingClassRcd = segment.booking_class_rcd;
                mapping.OriginRcd = segment.origin_rcd;
                mapping.DestinationRcd = segment.destination_rcd;
              //  mapping.FlightId = segment.flight_id;
                mapping.InventoryClassRcd = segment.booking_class_rcd;
            }

            return mapping;
        }

        // add tax to mapping
        public static IList<Avantik.Web.Service.Message.Booking.Mapping> FillMapping(this  IList<Tax> taxList, IList<Avantik.Web.Service.Message.Booking.Mapping> m)
        {

            if (taxList != null && m != null)
            {
                for (int i = 0; i < taxList.Count; i++)
                {
                    for (int j = 0; j < m.Count; j++)
                    {
                        if (taxList[i].booking_segment_id == m[j].BookingSegmentId && taxList[i].passenger_id == m[j].PassengerId)
                            taxList[i].FillMapping(m[j]);
                    }
                }
            }
            return m;
        }
        public static Avantik.Web.Service.Message.Booking.Mapping FillMapping(this  Tax tax, Avantik.Web.Service.Message.Booking.Mapping mapping)
        {
            if (tax != null && mapping != null)
            {

                if (tax.tax_rcd != null && tax.tax_rcd.ToUpper() == "YQ")
                {
                    mapping.YqAmount  += tax.tax_amount;
                    mapping.YqAmountIncl += tax.tax_amount_incl;

                    mapping.AcctYqAmount += tax.tax_amount;
                    mapping.AcctYqAmountIncl += tax.tax_amount_incl;

                    mapping.YqVat += tax.tax_amount_incl - tax.tax_amount;
                }
                else
                {
                    mapping.TaxAmount += tax.tax_amount;
                    mapping.TaxAmountIncl += tax.tax_amount_incl;
                    mapping.TaxVat += tax.tax_amount_incl - tax.tax_amount;

                    mapping.AcctTaxAmount += tax.tax_amount;
                    mapping.AcctTaxAmountIncl += tax.tax_amount_incl;
                }

            }

            return mapping;
        }

    }
}
