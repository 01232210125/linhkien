using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Linq;
using System.ComponentModel.DataAnnotations;
using TSQ.Models;
using WebApplication2;


    public partial class vpc_dr : System.Web.UI.Page
    {
    linhkienDataContext db = new linhkienDataContext();

    protected void Page_Load(object sender, EventArgs e)
        {
			string SECURE_SECRET = "A3EFDFABA8653DF2342E8DAC29B51AF0";
			string hashvalidateResult = "";
			// Khoi tao lop thu vien
			VPCRequest conn = new VPCRequest("http://onepay.vn");
			conn.SetSecureSecret(SECURE_SECRET);
			// Xu ly tham so tra ve va kiem tra chuoi du lieu ma hoa
			hashvalidateResult = conn.Process3PartyResponse(Page.Request.QueryString);

			// Lay gia tri tham so tra ve tu cong thanh toan
			String vpc_TxnResponseCode = conn.GetResultField("vpc_TxnResponseCode", "Unknown");
			string amount = conn.GetResultField("vpc_Amount", "Unknown");
			string localed = conn.GetResultField("vpc_Locale", "Unknown"); 
			string command = conn.GetResultField("vpc_Command", "Unknown");
			string version = conn.GetResultField("vpc_Version", "Unknown");
			string cardBin = conn.GetResultField("vpc_Card", "Unknown");
			string orderInfo = conn.GetResultField("vpc_OrderInfo", "Unknown");
			string merchantID = conn.GetResultField("vpc_Merchant", "Unknown");
			string authorizeID = conn.GetResultField("vpc_AuthorizeId", "Unknown");
			string merchTxnRef = conn.GetResultField("vpc_MerchTxnRef", "Unknown");
			string transactionNo = conn.GetResultField("vpc_TransactionNo", "Unknown");
			string txnResponseCode = vpc_TxnResponseCode;
			string message = conn.GetResultField("vpc_Message", "Unknown");
            //int loop1;

            // Bo cac ham ma hoa du lieu cu
            //NameValueCollection coll = Request.QueryString;
            //// Get names of all keys into a string array.
            //String[] arr1 = coll.AllKeys;
            //for (int j = 0; j < arr1.Length;j++ )
            //{
            //    arr1[j] = Server.HtmlEncode(arr1[j]);
            //}
            //Array.Sort(arr1, arr1);
            //string sdataHash = "";
            //for (loop1 = 0; loop1 < arr1.Length; loop1++)
            //{    
            //    String[] arr2 = coll.GetValues(arr1[loop1]);      
            //    if ((arr2[0] != null) && (arr2[0].Length > 0) && (arr1[loop1]!="vpc_SecureHash"))
            //    {
            //        sdataHash += Server.HtmlEncode(arr2[0]);
            //    }
            //}

            //    sdataHash = SECURE_SECRET + sdataHash;
            //    string doSecureHash = DoMD5(sdataHash).Trim();

            // Sua lai ham check chuoi ma hoa du lieu
            string mathanhtoantructuyen = merchTxnRef;
            
           
            if (hashvalidateResult == "CORRECTED" && txnResponseCode.Trim() == "0")
            {


                // var products = (from p in db.DONDATHANGs
                //                  orderby p.MaDonHang descending
                //                  select p).Skip(0).Take(1);
                //int a = 0;
                //foreach (var item in products)
                //{
                //    a = item.MaDonHang;
                //}
                //DONDATHANG dh = db.DONDATHANGs.SingleOrDefault(m => m.MaDonHang == a);
                //// nếu tôn tại đơn hàng
                //if (dh != null)
                //{
                    
                //    dh.Dathanhtoan = true;
                //    UpdateModel(dh);
                //    db.SubmitChanges();
                //}

                //vpc_Result.Text = "Transaction was paid successful";
                Response.Write("<div class='result'>Đã Thanh Toán Thành Công</div><br/><div style='text-align:center'><a href='http://localhost:36860/Home/Update' class='btn btn-primary' style='text-decoration:none;'>Vui lòng bấm <b style='color:red'>Xác Nhận</b> để kết thúc giao dịch</a></div>");
            }
        else if(hashvalidateResult == "INVALIDATED" && txnResponseCode.Trim() == "0"){
                //var products = (from p in db.DONDATHANGs
                //                orderby p.MaDonHang descending
                //                select p).Skip(0).Take(1);
                //int a = 0;
                //foreach (var item in products)
                //{
                //    a = item.MaDonHang;
                //}
                //DONDATHANG dh = db.DONDATHANGs.SingleOrDefault(m => m.MaDonHang == a);
                //// nếu tôn tại đơn hàng
                //if (dh != null)
                //{
                //    dh.Dathanhtoan = false;
                //    UpdateModel(dh);
                //    db.SubmitChanges();
                //}
                Response.Write("Error description: "+message +"<br/>");
                Response.Write("<br/><div class='result'>Thanh Toán Đang Chờ</div><br/><div style='text-align:center'><a href='http://localhost:36860/Home/Update1' class='btn btn-primary' style='text-decoration:none;'>Vui lòng bấm <b style='color:red'>Xác Nhận</b> để kết thúc giao dịch</a></div>");
                //vpc_Result.Text = "Transaction is pending";
            }
            else{
                //vpc_Result.Text = "Transaction was not paid successful";
                Response.Write("Error description: " + message + "<br/>");
                Response.Write("<div style='text-align:center'><a href='/'class='btn btn-primary' style='text-decoration:none;color:red'>Về Trang Chủ</a></div><br/><div class='result'>Thanh Toán Không Thành Công</div>");
            }
  
        }
    }
