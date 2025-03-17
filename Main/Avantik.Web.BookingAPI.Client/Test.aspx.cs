using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avantik.Web.BookingAPI.Service.Message;
using Avantik.Web.BookingAPI.Service.MessageEntension;
using Avantik.Web.BookingAPI.Service;
using Avantik.Web.BookingAPI.Service.Message.BookingService;

namespace Avantik.Web.BookingAPI.Client
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Guid segId = Guid.NewGuid();
            Guid segId2 = Guid.NewGuid();

            Guid passId =  Guid.NewGuid();
            Guid passId2 = Guid.NewGuid();
            Guid passId3 = Guid.NewGuid();


            BookingSaveRequest reqSave = new BookingSaveRequest();

            reqSave.AgencyCode = "agen123";
            reqSave.UserLogon = "agent001";
            reqSave.Password = "agent0011";

            reqSave.BookingHeader = new BookingHeader();
            reqSave.BookingHeader.agency_code = "B2C";
            reqSave.BookingHeader.booking_number ="0";
            reqSave.BookingHeader.lastname = "Ant";
            reqSave.BookingHeader.firstname = "Boy";
            reqSave.BookingHeader.title_rcd = "Mr";
            reqSave.BookingHeader.currency_rcd = "GBP";

            Address add = new Address();

            add.address_line1 = "115/127";
            add.city = "Nonthaburi";

            Telephone tel = new Telephone();
            tel.phone_home = "99";
            tel.phone_mobile = "888";
            tel.phone_fax = "99";
            tel.phone_business = "9999";

            reqSave.BookingHeader.address = add;
            reqSave.BookingHeader.telephone = tel;


            FlightSegment Segment = new FlightSegment();
            Segment.origin_rcd = "ZRH";
            Segment.destination_rcd = "AMS";
            Segment.flight_number = "1234";
            Segment.boarding_class_rcd = "Y";
            Segment.booking_segment_id = segId;
            Segment.departure_date = new DateTime(2014,12,12);
            Segment.number_of_units = 1;
            Segment.booking_class_rcd = "Y";
            Segment.airline_rcd = "GR";

            IList<FlightSegment> segmentList = new List<FlightSegment>();
            segmentList.Add(Segment);

            reqSave.BookingSegments = segmentList;

            Passenger p = new Passenger();
            p.passenger_id = passId;
            p.title_rcd = "MR";
            p.lastname = "lastname1";
            p.firstname = "firstname1";
            p.gender_type_rcd = "M";
            p.client_number = "0";
            p.vip_flag = 0;
            p.guardian_passenger_id = Guid.NewGuid();
            p.member_airline_rcd = "member";
            p.date_of_birth = new DateTime(1987,12,12);
            p.passenger_type_rcd = "ADULT";

            p.Document = new PassengerDocument();
            p.Document.country_code_long = "TH";
            p.Document.passport_birth_place = "bkk";

            IList<Passenger> pass = new List<Passenger>();
            pass.Add(p);

            reqSave.Passengers = pass;

            Mapping m = new Mapping();
            m.booking_segment_id = segId;
            m.commission_amount = 9;
            m.commission_amount_incl = 99;
            m.baggage_weight = 12;
            m.currency_rcd = "GBP";
            m.passenger_id = passId;
            m.through_fare_flag = 0;

            IList<Mapping> Mapping = new List<Mapping>();
            Mapping.Add(m);

            reqSave.Mappings = Mapping;

            Fee f = new Fee();
            f.booking_segment_id = segId;
            f.fee_amount = 1;
            f.currency_rcd = "GBP";
            f.charge_amount = 1;
            f.weight_kgs= 9;
            f.charge_amount_incl = 10;
            f.fee_amount = 3;
            f.fee_amount_incl = 5;
            f.passenger_id = passId;
            f.fee_id = Guid.NewGuid();

            IList<Fee> fee = new List<Fee>();
            fee.Add(f);

            reqSave.Fees = fee;


            Avantik.Web.BookingAPI.Service.Message.BookingService.Service  s= new Service.Message.BookingService.Service();
            s.booking_segment_id = segId;
            s.passenger_id = passId;
            s.service_text = "test";
            s.special_service_rcd = "rcd";
            s.number_of_units = 1;
            s.unit_rcd = "unit";

            IList<Avantik.Web.BookingAPI.Service.Message.BookingService.Service> ser = new List<Avantik.Web.BookingAPI.Service.Message.BookingService.Service>();
            ser.Add(s);

            reqSave.Services = ser;


            BookingService bs = new BookingService();

          //  Service.Message.BookingService.BookingSaveResponse res = bs.BookingSave(reqSave);


            BookingReadRequest reqRead = new BookingReadRequest();
            reqRead.AgencyCode = "agen123";
            reqRead.UserLogon = "agent001";
            reqRead.Password = "agent0011";

          //  booking_id = {e078bee3-bdaf-46aa-813a-e000f222cfbe}

          //  reqRead.booking_id = new Guid("e2a9a7dd-d5c3-4608-a4dc-94341771cbd3");

          //  Service.Message.BookingService.BookingReadResponse res = bs.BookingRead(reqRead);


            BookingCancelRequest reqCancel = new BookingCancelRequest();
            reqCancel.AgencyCode = "agen123";
            reqCancel.UserLogon = "agent001";
            reqCancel.Password = "agent0011";

            reqCancel.booking_id = new Guid("d318b232-0237-48a4-b77f-c0d61c423db2");
           // Service.Message.BookingService.BookingCancelResponse res = bs.BookingCancel(reqCancel);


            BookingItemsAddRequest req = new BookingItemsAddRequest();

            Fee fe = new Fee();
            fe.booking_segment_id = new Guid("26727a33-86f2-4974-a540-859c835c35a1");
            fe.fee_amount = 1;
            fe.currency_rcd = "CHF";
            fe.charge_amount = 1;
            fe.weight_kgs = 0;
            fe.charge_amount_incl = 10;
            fe.fee_amount = 3;
            fe.fee_amount_incl = 53;
            fe.passenger_id = new Guid("8d64f64f-f0c9-4357-b498-9731e17d9647");
            fe.number_of_units = 1;
            fe.fee_rcd = "BAGF";
            fe.fee_id = Guid.NewGuid();
            IList<Fee> fees = new List<Fee>();
            fees.Add(fe);

            //Fee fe2 = new Fee();
            //fe2.booking_segment_id = new Guid("78427650-d955-4869-bfe2-9098c77a6b2b");
            //fe2.fee_amount = 1;
            //fe2.currency_rcd = "CHF";
            //fe2.charge_amount = 1;
            //fe2.weight_kgs = 0;
            //fe2.charge_amount_incl = 10;
            //fe2.fee_amount = 3;
            //fe2.fee_amount_incl = 54;
            //fe2.passenger_id = new Guid("e811b8a5-422b-46d6-829f-d1ddc6ec57eb");
            //fe2.number_of_units = 1;
            //fe2.fee_rcd = "INSU";
            //fe2.fee_id = Guid.NewGuid();
            //fees.Add(fe2);

            req.Fees = fees;


            //Remark r = new Remark();
            //r.nickname = "nickname";
            //r.remark_type_rcd = "AUXCAR";
            //r.remark_text = "test remark";
            //IList<Remark> reList = new List<Remark>();
            //reList.Add(r);
            //req.Remarks = reList;

            Payment pay = new Payment();
            pay.form_of_payment_subtype_rcd = "VISA";
            pay.currency_rcd = "CHF";
            pay.payment_amount = 5000;
            pay.receive_currency_rcd = "CHF";
            pay.payment_date_time = new DateTime(2014,7,16);
            pay.payment_type_rcd = "CC";
            pay.transaction_reference = "500098";
            pay.payment_number = "4500000000000000";
            pay.credit_card = new CreditCard();
            pay.credit_card.Address = new Address();
            pay.credit_card.Address.zip_code = "900";
            pay.credit_card.Address.address_line1 = "addr1"; 
            IList<Payment> payList = new List<Payment>();
            payList.Add(pay);

            req.Payments = payList;
            req.booking_id = new Guid("882da74b-f7c8-4abd-8d7b-ca1247795407");

            req.AgencyCode = "soichf";
            req.UserLogon = "myuser";
            req.Password = "password123";


          Service.Message.BookingService.BookingItemsAddResponse res = bs.BookingItemsAdd(req);
        }
    }
}