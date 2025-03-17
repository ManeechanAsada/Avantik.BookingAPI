using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avantik.Web.Service;
using Avantik.Web.BookingAPI.Service.Message.BookingService;

namespace Avantik.Web.BookingAPI.Service.MessageEntension
{
    public static class BookingReadAPIResponseMapping
    {
        public static BookingHeaderReadResponse FillObjectAPIResponse(this  Avantik.Web.Service.Message.Booking.BookingHeader header)
        {
            BookingHeaderReadResponse bookingHeader = null;

            if (header != null)
            {
                bookingHeader = new BookingHeaderReadResponse();
                bookingHeader.booking_id = header.BookingId;
                bookingHeader.record_locator = header.RecordLocator;
                bookingHeader.agency_code = header.AgencyCode;
                bookingHeader.currency_rcd = header.CurrencyRcd;
                bookingHeader.booking_number =Convert.ToString(header.BookingNumber);
                bookingHeader.language_rcd = header.LanguageRcd;
                bookingHeader.contact_name = header.ContactName;
                bookingHeader.contact_email = header.ContactEmail;
                bookingHeader.received_from = header.ReceivedFrom;
                bookingHeader.phone_search = header.PhoneSearch;
                bookingHeader.comment = header.Comment;
                bookingHeader.title_rcd = header.TitleRcd;
                bookingHeader.lastname = header.Lastname;
                bookingHeader.firstname = header.Firstname;
                bookingHeader.middlename = header.Middlename;
                bookingHeader.country_rcd = header.CountryRcd;
                bookingHeader.group_booking_flag = header.GroupBookingFlag;

                bookingHeader.address = new Message.Address();
                bookingHeader.address.address_line1 = header.AddressLine1;
                bookingHeader.address.address_line2 = header.AddressLine2;
                bookingHeader.address.street = header.Street;
                bookingHeader.address.po_box = header.PoBox;
                bookingHeader.address.city = header.City;
                bookingHeader.address.state = header.State;
                bookingHeader.address.district = header.District;
                bookingHeader.address.province = header.Province;
                bookingHeader.address.zip_code = header.ZipCode;
                bookingHeader.address.country_rcd = header.CountryRcd;

                bookingHeader.telephone = new Message.Telephone();
                bookingHeader.telephone.phone_mobile = header.PhoneMobile;
                bookingHeader.telephone.phone_home = header.PhoneHome;
                bookingHeader.telephone.phone_business = header.PhoneBusiness;
                bookingHeader.telephone.phone_fax = header.PhoneFax;
            }

            return bookingHeader;
        }
        public static IList<FlightSegmentReadResponse> FillObjectAPIResponse(this  IList<Avantik.Web.Service.Message.Booking.FlightSegment> objBooking)
        {
            IList<FlightSegmentReadResponse> objResponseList = null;
            if (objBooking != null)
            {
                objResponseList = new List<FlightSegmentReadResponse>();
                for (int i = 0; i < objBooking.Count; i++)
                {
                    objResponseList.Add(objBooking[i].FillObjectAPIResponse());
                }
            }
            return objResponseList;
        }
        public static FlightSegmentReadResponse FillObjectAPIResponse(this  Avantik.Web.Service.Message.Booking.FlightSegment objBookingResponse)
        {
            FlightSegmentReadResponse objReadResponse = null;

            if (objBookingResponse != null)
            {
                objReadResponse = new FlightSegmentReadResponse();
                objReadResponse.booking_segment_id = objBookingResponse.BookingSegmentId;
                objReadResponse.flight_connection_id = objBookingResponse.FlightConnectionId;

                objReadResponse.departure_date = objBookingResponse.DepartureDate;

                objReadResponse.airline_rcd = objBookingResponse.AirlineRcd;
                objReadResponse.flight_number = objBookingResponse.FlightNumber;
                objReadResponse.origin_rcd = objBookingResponse.OriginRcd;
                objReadResponse.destination_rcd = objBookingResponse.DestinationRcd;
                objReadResponse.od_origin_rcd = objBookingResponse.OdOriginRcd;
                objReadResponse.od_destination_rcd = objBookingResponse.OdDestinationRcd;
                objReadResponse.booking_class_rcd = objBookingResponse.BookingClassRcd;
                objReadResponse.boarding_class_rcd = objBookingResponse.BoardingClassRcd;
                objReadResponse.segment_status_rcd = objBookingResponse.SegmentStatusRcd;

                //Use for passenger count in the flight.
                objReadResponse.number_of_units = objBookingResponse.NumberOfUnits;

            }

            return objReadResponse;
        }
        public static IList<PassengerReadResponse> FillObjectAPIResponse(this  IList<Avantik.Web.Service.Message.Booking.Passenger> objBooking)
        {
            IList<PassengerReadResponse> objResponseList = null;
            if (objBooking != null)
            {
                objResponseList = new List<PassengerReadResponse>();
                for (int i = 0; i < objBooking.Count; i++)
                {
                    objResponseList.Add(objBooking[i].FillObjectAPIResponse());
                }
            }
            return objResponseList;
        }
        public static PassengerReadResponse FillObjectAPIResponse(this  Avantik.Web.Service.Message.Booking.Passenger objBookingResponse)
        {
            PassengerReadResponse objReadResponse = null;

            if (objBookingResponse != null)
            {
                objReadResponse = new PassengerReadResponse();
                objReadResponse.passenger_id = objBookingResponse.PassengerId;
                //optional use only when want to defind guardian of passenger type CHD
                objReadResponse.guardian_passenger_id = objBookingResponse.GuardianPassengerId;

                objReadResponse.date_of_birth = objBookingResponse.DateOfBirth;

                objReadResponse.client_number = objBookingResponse.ClientNumber.ToString();

                objReadResponse.passenger_type_rcd = objBookingResponse.PassengerTypeRcd;
                objReadResponse.lastname = objBookingResponse.Lastname;
                objReadResponse.firstname = objBookingResponse.Firstname;
                objReadResponse.middlename = objBookingResponse.Middlename;
                objReadResponse.title_rcd = objBookingResponse.TitleRcd;
                objReadResponse.member_number = objBookingResponse.MemberNumber;
                objReadResponse.member_airline_rcd = objBookingResponse.MemberAirlineRcd;
                objReadResponse.member_level_rcd = objBookingResponse.MemberLevelRcd;
                objReadResponse.gender_type_rcd = objBookingResponse.GenderTypeRcd;
                objReadResponse.nationality_rcd = objBookingResponse.NationalityRcd;
                objReadResponse.residence_country_rcd = objBookingResponse.ResidenceCountryRcd;
                objReadResponse.redress_number = objBookingResponse.RedressNumber;

                objReadResponse.known_traveler_number = objBookingResponse.KnownTravelerNumber;

                //EDWDEV-81
               // objReadResponse.pnr_firstname = objBookingResponse.PnrFirstName;
               // objReadResponse.pnr_lastname = objBookingResponse.PnrLastName;

                objReadResponse.vip_flag = objBookingResponse.VipFlag;

                objReadResponse.passenger_weight = objBookingResponse.PassengerWeight;

                //Document
                objReadResponse.Document = new PassengerDocument();
                if (objReadResponse.Document != null)
                {
                    objReadResponse.Document.passport_number = objBookingResponse.PassportNumber;
                    objReadResponse.Document.document_type_rcd = objBookingResponse.DocumentTypeRcd;
                    objReadResponse.Document.passport_issue_place = objBookingResponse.PassportIssuePlace;
                    objReadResponse.Document.passport_birth_place = objBookingResponse.PassportBirthPlace;
                    objReadResponse.Document.passport_issue_country_rcd = objBookingResponse.PassportIssueCountryRcd;
                    objReadResponse.Document.country_code_long = objBookingResponse.CountryCodeLong;
                    objReadResponse.Document.passport_issue_date = objBookingResponse.PassportIssueDate;
                    objReadResponse.Document.passport_expiry_date = objBookingResponse.PassportExpiryDate;
                }
            }

            return objReadResponse;
        }
        public static IList<MappingReadResponse> FillObjectAPIResponse(this  IList<Avantik.Web.Service.Message.Booking.Mapping> objBooking)
        {
            IList<MappingReadResponse> objResponseList = null;
            if (objBooking != null)
            {
                objResponseList = new List<MappingReadResponse>();
                for (int i = 0; i < objBooking.Count; i++)
                {
                    objResponseList.Add(objBooking[i].FillObjectAPIResponse());
                }
            }
            return objResponseList;
        }
        public static MappingReadResponse FillObjectAPIResponse(this  Avantik.Web.Service.Message.Booking.Mapping objBookingResponse)
        {
            MappingReadResponse objReadResponse = null;

            if (objBookingResponse != null)
            {
                objReadResponse = new MappingReadResponse();
                objReadResponse.passenger_id = objBookingResponse.PassengerId;
                objReadResponse.booking_segment_id = objBookingResponse.BookingSegmentId;

                objReadResponse.not_valid_before_date = objBookingResponse.NotValidBeforeDate;
                objReadResponse.not_valid_after_date = objBookingResponse.NotValidAfterDate;

                objReadResponse.refund_with_charge_hours = objBookingResponse.RefundWithChargeHours;
                objReadResponse.refund_not_possible_hours = objBookingResponse.RefundNotPossibleHours;

                objReadResponse.piece_allowance = objBookingResponse.PieceAllowance;

                objReadResponse.fare_code = objBookingResponse.FareCode;
                objReadResponse.seat_number = objBookingResponse.SeatNumber;
                objReadResponse.currency_rcd = objBookingResponse.CurrencyRcd;
                objReadResponse.endorsement_text = objBookingResponse.EndorsementText;
                objReadResponse.restriction_text = objBookingResponse.RestrictionText;

                objReadResponse.fare_amount = objBookingResponse.FareAmount;
                objReadResponse.fare_amount_incl = objBookingResponse.FareAmountIncl;
                objReadResponse.fare_vat = objBookingResponse.FareVat;
                objReadResponse.net_total = objBookingResponse.NetTotal;
                //Optinal
                objReadResponse.commission_amount = objBookingResponse.CommissionAmount;
                //Optinal
                objReadResponse.commission_amount_incl = objBookingResponse.CommissionAmountIncl;
                //Optinal
                objReadResponse.commission_percentage = objBookingResponse.CommissionPercentage;
                //Optinal
                objReadResponse.public_fare_amount = objBookingResponse.PublicFareAmount;
                //Optinal
                objReadResponse.public_fare_amount_incl = objBookingResponse.PublicFareAmountIncl;
                objReadResponse.refund_charge = objBookingResponse.RefundCharge;
                objReadResponse.baggage_weight = objBookingResponse.BaggageWeight;
                objReadResponse.redemption_points = objBookingResponse.RedemptionPoints;

                objReadResponse.e_ticket_flag = objBookingResponse.ETicketFlag;
                objReadResponse.refundable_flag = objBookingResponse.RefundableFlag;
                objReadResponse.transferable_fare_flag = objBookingResponse.TransferableFareFlag;
                objReadResponse.through_fare_flag = objBookingResponse.ThroughFareFlag;
                objReadResponse.it_fare_flag = objBookingResponse.ItFareFlag;
                objReadResponse.duty_travel_flag = objBookingResponse.DutyTravelFlag;
                objReadResponse.standby_flag = objBookingResponse.StandbyFlag;
                objReadResponse.exclude_pricing_flag = objBookingResponse.ExcludePricingFlag;

            }

            return objReadResponse;
        }
        public static IList<TaxReadResponse> FillObjectAPIResponse(this  IList<Avantik.Web.Service.Message.Booking.Tax> objBooking)
        {
            IList<TaxReadResponse> objResponseList = null;
            if (objBooking != null)
            {
                objResponseList = new List<TaxReadResponse>();
                for (int i = 0; i < objBooking.Count; i++)
                {
                    objResponseList.Add(objBooking[i].FillObjectAPIResponse());
                }
            }
            return objResponseList;
        }
        public static TaxReadResponse FillObjectAPIResponse(this  Avantik.Web.Service.Message.Booking.Tax objBookingResponse)
        {
            TaxReadResponse objReadResponse = null;

            if (objBookingResponse != null)
            {
                objReadResponse = new TaxReadResponse();
                objReadResponse.passenger_id = objBookingResponse.PassengerId;
                objReadResponse.booking_segment_id = objBookingResponse.BookingSegmentId;

                objReadResponse.tax_rcd = objBookingResponse.TaxRcd;
                objReadResponse.tax_currency_rcd = objBookingResponse.TaxCurrencyRcd;
                objReadResponse.sales_currency_rcd = objBookingResponse.SalesCurrencyRcd;

                objReadResponse.sales_amount = objBookingResponse.SalesAmount;
                objReadResponse.sales_amount_incl = objBookingResponse.SalesAmountIncl;
                objReadResponse.tax_amount = objBookingResponse.TaxAmount;
                objReadResponse.tax_amount_incl = objBookingResponse.TaxAmountIncl;

            }

            return objReadResponse;
        }
        public static IList<FeeReadResponse> FillObjectAPIResponse(this  IList<Avantik.Web.Service.Message.Booking.Fee> objBooking)
        {
            IList<FeeReadResponse> objResponseList = null;
            if (objBooking != null)
            {
                objResponseList = new List<FeeReadResponse>();
                for (int i = 0; i < objBooking.Count; i++)
                {
                    objResponseList.Add(objBooking[i].FillObjectAPIResponse());
                }
            }
            return objResponseList;
        }
        public static FeeReadResponse FillObjectAPIResponse(this  Avantik.Web.Service.Message.Booking.Fee objBookingResponse)
        {
            FeeReadResponse objReadResponse = null;

            if (objBookingResponse != null)
            {
                objReadResponse = new FeeReadResponse();
                objReadResponse.passenger_id = objBookingResponse.PassengerId;
                objReadResponse.booking_segment_id = objBookingResponse.BookingSegmentId;
                objReadResponse.passenger_segment_service_id = objBookingResponse.PassengerSegmentServiceId;

                objReadResponse.fee_id = objBookingResponse.BookingFeeId;//objBookingResponse.FeeId;

                objReadResponse.fee_rcd = objBookingResponse.FeeRcd;
                objReadResponse.vendor_rcd = objBookingResponse.VendorRcd;
                objReadResponse.od_origin_rcd = objBookingResponse.OdOriginRcd;
                objReadResponse.od_destination_rcd = objBookingResponse.OdDestinationRcd;
                objReadResponse.currency_rcd = objBookingResponse.CurrencyRcd;
                objReadResponse.charge_currency_rcd = objBookingResponse.ChargeCurrencyRcd;
                objReadResponse.unit_rcd = objBookingResponse.UnitRcd;
                objReadResponse.mpd_number = objBookingResponse.MpdNumber;
                objReadResponse.comment = objBookingResponse.ChangeComment;
                objReadResponse.external_reference = objBookingResponse.ExternalReference;

                objReadResponse.fee_amount = objBookingResponse.FeeAmount;
                objReadResponse.fee_amount_incl = objBookingResponse.FeeAmountIncl;
                objReadResponse.vat_percentage = objBookingResponse.VatPercentage;
                objReadResponse.charge_amount = objBookingResponse.ChargeAmount;
                objReadResponse.charge_amount_incl = objBookingResponse.ChargeAmountIncl;
                objReadResponse.weight_lbs = objBookingResponse.WeightLbs;
                objReadResponse.weight_kgs = objBookingResponse.WeightKgs;
                objReadResponse.number_of_units = objBookingResponse.NumberOfUnits;
            }

            return objReadResponse;
        }
        public static IList<RemarkReadResponse> FillObjectAPIResponse(this  IList<Avantik.Web.Service.Message.Booking.Remark> objBooking)
        {
            IList<RemarkReadResponse> objResponseList = null;
            if (objBooking != null)
            {
                objResponseList = new List<RemarkReadResponse>();
                for (int i = 0; i < objBooking.Count; i++)
                {
                    objResponseList.Add(objBooking[i].FillObjectAPIResponse());
                }
            }
            return objResponseList;
        }
        public static RemarkReadResponse FillObjectAPIResponse(this  Avantik.Web.Service.Message.Booking.Remark objBookingResponse)
        {
            RemarkReadResponse objReadResponse = null;

            if (objBookingResponse != null)
            {
                objReadResponse = new RemarkReadResponse();
                objReadResponse.timelimit_date_time = objBookingResponse.TimelimitDateTime;
                objReadResponse.utc_timelimit_date_time = objBookingResponse.UtcTimelimitDateTime;

                objReadResponse.remark_type_rcd = objBookingResponse.RemarkTypeRcd;
                objReadResponse.remark_text = objBookingResponse.RemarkText;
                objReadResponse.nickname = objBookingResponse.Nickname;

            }

            return objReadResponse;
        }
        public static IList<ServiceReadResponse> FillObjectAPIResponse(this  IList<Avantik.Web.Service.Message.Booking.PassengerService> objBooking)
        {
            IList<ServiceReadResponse> objResponseList = null;
            if (objBooking != null)
            {
                objResponseList = new List<ServiceReadResponse>();
                for (int i = 0; i < objBooking.Count; i++)
                {
                    objResponseList.Add(objBooking[i].FillObjectAPIResponse());
                }
            }
            return objResponseList;
        }
        public static ServiceReadResponse FillObjectAPIResponse(this  Avantik.Web.Service.Message.Booking.PassengerService objBookingResponse)
        {
            ServiceReadResponse objReadResponse = null;

            if (objBookingResponse != null)
            {
                objReadResponse = new ServiceReadResponse();
                objReadResponse.passenger_segment_service_id = objBookingResponse.PassengerSegmentServiceId;
                objReadResponse.passenger_id = objBookingResponse.PassengerId;
                objReadResponse.booking_segment_id = objBookingResponse.BookingSegmentId;

                objReadResponse.number_of_units = objBookingResponse.NumberOfUnits;

                objReadResponse.special_service_rcd = objBookingResponse.SpecialServiceRcd;
                objReadResponse.service_text = objBookingResponse.ServiceText;
                //The value should be SS or NN
                objReadResponse.special_service_status_rcd = objBookingResponse.SpecialServiceStatusRcd;
                objReadResponse.unit_rcd = objBookingResponse.UnitRcd;

            }

            return objReadResponse;
        }
        public static IList<PaymentReadResponse> FillObjectAPIResponse(this  IList<Avantik.Web.Service.Message.Booking.Payment> objBooking)
        {
            IList<PaymentReadResponse> objResponseList = null;
            if (objBooking != null)
            {
                objResponseList = new List<PaymentReadResponse>();
                for (int i = 0; i < objBooking.Count; i++)
                {
                    objResponseList.Add(objBooking[i].FillObjectAPIResponse());
                }
            }
            return objResponseList;
        }
        public static PaymentReadResponse FillObjectAPIResponse(this  Avantik.Web.Service.Message.Booking.Payment objBookingResponse)
        {
            PaymentReadResponse objReadResponse = null;

            if (objBookingResponse != null)
            {
                objReadResponse = new PaymentReadResponse();
                objReadResponse.payment_date_time = objBookingResponse.PaymentDateTime;

                objReadResponse.payment_amount = objBookingResponse.PaymentAmount;
                objReadResponse.receive_payment_amount = objBookingResponse.ReceivePaymentAmount;

                //Value will be SALE or REFUND
                objReadResponse.payment_type_rcd = objBookingResponse.PaymentTypeRcd;
                objReadResponse.form_of_payment_rcd = objBookingResponse.FormOfPaymentRcd;
                objReadResponse.form_of_payment_subtype_rcd = objBookingResponse.FormOfPaymentSubtypeRcd;
                objReadResponse.debit_agency_code = objBookingResponse.DebitAgencyCode;
                objReadResponse.currency_rcd = objBookingResponse.CurrencyRcd;
                objReadResponse.receive_currency_rcd = objBookingResponse.ReceiveCurrencyRcd;
                objReadResponse.approval_code = objBookingResponse.ApprovalCode;
                objReadResponse.transaction_reference = objBookingResponse.TransactionReference;
                objReadResponse.payment_number = objBookingResponse.PaymentNumber;
                objReadResponse.payment_reference = objBookingResponse.PaymentReference;
                objReadResponse.payment_remark = objBookingResponse.PaymentRemark;

                // credit card
                if (objReadResponse.credit_card == null) objReadResponse.credit_card = new CreditCard();
                if (objReadResponse.credit_card.Address == null) objReadResponse.credit_card.Address = new Message.Address();

                objReadResponse.credit_card.Address.address_line1 = objBookingResponse.AddressLine1;

            }

            return objReadResponse;
        }
        public static IList<QuoteReadResponse> FillObjectAPIResponse(this  IList<Avantik.Web.Service.Message.Booking.Quote> objBooking)
        {
            IList<QuoteReadResponse> objResponseList = null;
            if (objBooking != null)
            {
                objResponseList = new List<QuoteReadResponse>();
                for (int i = 0; i < objBooking.Count; i++)
                {
                    objResponseList.Add(objBooking[i].FillObjectAPIResponse());
                }
            }
            return objResponseList;
        }
        public static QuoteReadResponse FillObjectAPIResponse(this  Avantik.Web.Service.Message.Booking.Quote objBookingResponse)
        {
            QuoteReadResponse objReadResponse = null;

            if (objBookingResponse != null)
            {
                objReadResponse = new QuoteReadResponse();
                objReadResponse.booking_segment_id = objBookingResponse.BookingSegmentId;
                objReadResponse.passenger_type_rcd = objBookingResponse.PassengerTypeRcd;
                objReadResponse.currency_rcd = objBookingResponse.CurrencyRcd;
                objReadResponse.charge_type = objBookingResponse.ChargeType;
                objReadResponse.charge_name = objBookingResponse.ChargeName;
                objReadResponse.charge_amount = objBookingResponse.ChargeAmount;
                objReadResponse.total_amount = objBookingResponse.TotalAmount;
                objReadResponse.tax_amount = objBookingResponse.TaxAmount;
                objReadResponse.passenger_count = objBookingResponse.PassengerCount;
                objReadResponse.sort_sequence = objBookingResponse.SortSequence;
                objReadResponse.create_by = objBookingResponse.CreateBy;
                objReadResponse.create_date_time = objBookingResponse.CreateDateTime;
                objReadResponse.update_by = objBookingResponse.UpdateBy;
                objReadResponse.update_date_time = objBookingResponse.UpdateDateTime;
                objReadResponse.redemption_points = objBookingResponse.RedemptionPoints;
                objReadResponse.charge_amount_incl = objBookingResponse.ChargeAmountIncl; 
            }

            return objReadResponse;
        }

    }
}
