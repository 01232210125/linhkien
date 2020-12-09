using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TSQ.Models
{
    public class OnepayCode
    {
        public const string VPCRequest = "http://mtf.onepay.vn/onecomm-pay/vpc.op"; //Url Payment that: https://mtf.onepay.vn/onecomm-pay/vpc.op
        public const string Merchant = "ONEPAY";
        public const string AccessCode = "D67342C2";
        public const string SECURE_SECRET = "A3EFDFABA8653DF2342E8DAC29B51AF0";

        public static string ReturnURL = "http://localhost:36860/onepay/vpc_dr.aspx";
    }
}