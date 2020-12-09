using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TSQ.Models;
using WebApplication2;

namespace TSQ.Controllers
{
    public class GioHangController : Controller
    {
        linhkienDataContext db = new linhkienDataContext();
        // GET: GioHang
        public ActionResult GioHang()
        {
            if (Session["TaiKhoan"]!=null)
            {
                ViewBag.Bien = Session["TaiKhoan"].ToString();
                ViewBag.TongTien = TongTien() - TongTien() * 5 / 100;
            }
            else
            {
                ViewBag.Bien = "";
                ViewBag.TongTien = TongTien();
            }
            List<GIOHANG> lstGioHang = LayGioHang();
            if (lstGioHang.Count() == 0)
            {
                return RedirectToAction("Index","Home");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            return View(lstGioHang);
        }
        // Lay gio hang
        public List<GIOHANG> LayGioHang()
        {
            List<GIOHANG> lstGioHang = Session["GIOHANG"] as List<GIOHANG>;
            if (lstGioHang == null)
            {
                //Neu gio hang chua ton tai thi khoi tao list gio hang
                lstGioHang = new List<GIOHANG>();
                Session["GIOHANG"] = lstGioHang;
            }
            return lstGioHang;
        }
        // Them Gio Hang
        public ActionResult ThemGioHang(int iMaSP, string strURL)
        {
            //Lay ra session gio hang
            List<GIOHANG> lstGioHang = LayGioHang();
            // kiem tra san pham nay co ton tai trong gio hang hay khong ???
            GIOHANG sanpham = lstGioHang.Find(m => m.iMaSP == iMaSP);
            if (sanpham == null)
            {
                sanpham = new GIOHANG(iMaSP);
                lstGioHang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoLuong++;
                return Redirect(strURL);
            }
        }
        //Tong So Luong
        public int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GIOHANG> lstGioHang = Session["GIOHANG"] as List<GIOHANG>;
            if(lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(m => m.iSoLuong);
            }
            return iTongSoLuong;
        }
        //Tong Tien

        public double TongTien ()
        {
            double fTongTien = 0;
            List<GIOHANG> lstGioHang = Session["GIOHANG"] as List<GIOHANG>;
            if (lstGioHang != null)
            {
                fTongTien = lstGioHang.Sum(m => m.fThanhTien);
            }
            return fTongTien;
        }
        public ActionResult GioHangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        //Xoa San Pham
        public ActionResult XoaSP(int id)
        {
            //lay gio hang tu session
            List<GIOHANG> lstGioHang = LayGioHang();
            GIOHANG sanpham = lstGioHang.SingleOrDefault(m => m.iMaSP == id);
            if(sanpham != null)
            {
                lstGioHang.RemoveAll(k => k.iMaSP == id);
                return RedirectToAction("GioHang","GioHang");
            }
            if(lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("GioHang","GioHang");
        }
        [HttpPost]
        // Cập nhật Sản pHẩm
        public ActionResult CapNhat(FormCollection f)
        {
            //lay gio hang tu session
            List<GIOHANG> lstGioHang = LayGioHang();
            GIOHANG sanpham = lstGioHang.SingleOrDefault(m => m.iMaSP == int.Parse(f["MaSP"]));
            if (sanpham != null)
            {
                sanpham.iSoLuong = int.Parse(f["SoLuong"].ToString());
               
            }

            return RedirectToAction("GioHang","GioHang");
        }
        //Xoa tat ca gio Hang
        public ActionResult XoaGioHang()
        {
            //lay gio hang tu session
            List<GIOHANG> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            return RedirectToAction("Index", "Home");
        }
        //Xac nhan don hang
        [HttpPost]
        public ActionResult XacNhanDonHang(FormCollection f)
        {
            if (f["CMND"].Length != 9 && f["CMND"].Length != 12)
            {
                ViewData["L1"] = "Vui lòng nhập đúng CMND";
                return View();
            }
            List<GIOHANG> gh = LayGioHang();
            DONDATHANG donhang = new DONDATHANG();
            donhang.CMND = f["CMND"];
            donhang.Ngaydat = DateTime.Now;
            donhang.Ngaygiao = DateTime.Parse(f["NgayGiao"]);
            donhang.DiaChiLienHe = f["DCGiaoHang"];
            donhang.EmailLienHe = f["Email"];
            donhang.SDTLienHe = f["SDT"];
            donhang.Tinhtranggiaohang = false;
            donhang.TongTien = TongTien();
            donhang.Dathanhtoan = false;
            donhang.TrangThai = true;
            db.DONDATHANGs.InsertOnSubmit(donhang);
            db.SubmitChanges();
            string mathanhtoantructuyen = DateTime.Now.Ticks.ToString();
            if (string.Compare(f["rbHinhThucThanhToan"], "Onepay") == 0)
            {

                string SECURE_SECRET = OnepayCode.SECURE_SECRET;// HAO : CAN THANH MA THAT TRONG APP CODE
                //// KHOI TAO LOP THU VIEN  VA GAN GIA TRI CAC THAM SO
                VPCRequest conn = new VPCRequest(OnepayCode.VPCRequest);
                conn.SetSecureSecret(SECURE_SECRET);
                //add digital order field
                conn.AddDigitalOrderField("Title", "onepay paygate");
                conn.AddDigitalOrderField("vpc_Locale", "vn");
                conn.AddDigitalOrderField("vpc_Version", "2");
                conn.AddDigitalOrderField("vpc_Command", "pay");
                conn.AddDigitalOrderField("vpc_Merchant", OnepayCode.Merchant);
                conn.AddDigitalOrderField("vpc_AccessCode", OnepayCode.AccessCode);
                conn.AddDigitalOrderField("vpc_MerchTxnRef", mathanhtoantructuyen);
                conn.AddDigitalOrderField("vpc_OrderInfo", mathanhtoantructuyen);
                conn.AddDigitalOrderField("vpc_Amount", (TongTien() * 100).ToString());
                conn.AddDigitalOrderField("vpc_Currency", "VND");
                conn.AddDigitalOrderField("vpc_ReturnURL", OnepayCode.ReturnURL);
                conn.AddDigitalOrderField("vpc_SHIP_Street01", "");
                conn.AddDigitalOrderField("vpc_SHIP_Provice", "");
                conn.AddDigitalOrderField("vpc_SHIP_City", "");
                conn.AddDigitalOrderField("vpc_SHIP_Country", "");
                conn.AddDigitalOrderField("vpc_Customer_Phone", "");
                conn.AddDigitalOrderField("vpc_Customer_Email", "");
                conn.AddDigitalOrderField("vpc_Customer_Id", "");
                conn.AddDigitalOrderField("vpc_TicketNo", Request.UserHostAddress);
                string ketQua = "";
                string url = conn.Create3PartyQueryString();
                ketQua = url;
                foreach (var item in gh)
                {
                    CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                    SANPHAM dh = db.SANPHAMs.SingleOrDefault(m => m.MaSP == item.iMaSP);
                    ctdh.MaDonHang = donhang.MaDonHang;
                    ctdh.MaSP = item.iMaSP;
                    ctdh.Soluong = item.iSoLuong;
                    ctdh.ThanhTien = item.fThanhTien;
                    ctdh.Dongia = item.fDonGia;
                    db.CHITIETDONTHANGs.InsertOnSubmit(ctdh);
                    dh.Soluongton = dh.Soluongton - item.iSoLuong;
                    UpdateModel(dh);
                }
                Session["GIOHANG"] = null;
                db.SubmitChanges();
                ViewBag.Ma = donhang.MaDonHang;
                return Redirect(ketQua);
            }
            else
            {
                    foreach (var item in gh)
                {
                    CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                    SANPHAM dh = db.SANPHAMs.SingleOrDefault(m => m.MaSP == item.iMaSP);
                    ctdh.MaDonHang = donhang.MaDonHang;
                    ctdh.MaSP = item.iMaSP;
                    ctdh.Soluong = item.iSoLuong;
                    ctdh.ThanhTien = item.fThanhTien;
                    ctdh.Dongia = item.fDonGia;
                    db.CHITIETDONTHANGs.InsertOnSubmit(ctdh);
                    dh.Soluongton = dh.Soluongton - item.iSoLuong;
                    UpdateModel(dh);
                }
                Session["GIOHANG"] = null;
                db.SubmitChanges();
                return View();
            }

        }
    }
}