using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avantik.Web.BookingAPI.Service.Message.BookingService
{
    public class Payment
    {
        //If not passin use server time.
        public DateTime payment_date_time { get; set; }
        
        //Electronic fund transfer information.
        public EFT EFT { get; set; }
        
        //Credit card information.
        public CreditCard credit_card { get; set; }

        public decimal payment_amount { get; set; }
        public decimal receive_payment_amount { get; set; }

        //Value will be SALE or REFUND
        public string payment_type_rcd { get; set; }
        public string form_of_payment_rcd { get; set; }
        public string form_of_payment_subtype_rcd { get; set; }
        public string debit_agency_code { get; set; }
        public string currency_rcd { get; set; }
        public string receive_currency_rcd { get; set; }
        public string approval_code { get; set; }
        public string transaction_reference { get; set; }
        public string payment_number { get; set; }
        public string payment_reference { get; set; }        
        public string payment_remark { get; set; }
    }
}
