using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TSQ.Models;
using PagedList;
using PagedList.Mvc;
using System.Text;
using System.Net.Mail;
namespace TSQ.Controllers
{
    public class HomeController : Controller
    {
        linhkienDataContext DB = new linhkienDataContext();
        public ActionResult Index()
        {
            
            var query = from th in DB.LOAI_SPs where th.TrangThai == true select th;   
            return View(query.ToList());
        }
        public ActionResult Menu()
        {
            
            var query = from th in DB.THUONGHIEUs where th.TrangThai == true select th;
            return PartialView(query);
        }
        public ActionResult Slideshow() {
            var query = from th in DB.THUONGHIEUs where th.TrangThai == true select th;
            return PartialView(query);
        }
        public ActionResult HeaderImg(int id)
        {
            
            var query = from th in DB.THUONGHIEUs where th.MaTH==id && th.TrangThai == true select th;
            
            return PartialView(query.Single());
        }
       
        public ActionResult Search(string search, int? page)
        {
            int pageSize = 6;
            int pageNum = (page ?? 1);
            if(string.IsNullOrEmpty(search) || string.IsNullOrWhiteSpace(search))
            {
                ViewBag.search = "Không Tìm Thấy Kết Quả";
                return View();
            }
            ViewBag.x = search;
            
            var query = from dh in DB.SANPHAMs where dh.TenSP.Contains(search) && dh.TrangThai == true select dh;
            if (query.ToList().Count == 0)
            {
                ViewBag.search = "Không Tìm Thấy Kết Quả";
                return View();
            }
            return View((query.ToPagedList(pageNum, pageSize)));
        }
       
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult TaiKhoan()
        {
            //List<PHANQUYEN> list = Session["TaiKhoan"] as List<PHANQUYEN>;
            if(Session["TaiKhoan"] == null)
            {
                return PartialView();
            }
            else
            {
                var query = DB.KHACHHANGs.SingleOrDefault(m => m.CMND == Session["TaiKhoan"].ToString());
                return PartialView(query);
            }
            
        }
        //Đăng Nhập
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            string acc = f["TaiKhoan"].ToString();
            string pass = EncryptMD5.MD5Hash(f["pass"]);
                PHANQUYEN pq = DB.PHANQUYENs.SingleOrDefault(a => a.TaiKhoan == f["TaiKhoan"]
                                                             && a.PassWord ==EncryptMD5.MD5Hash(f["pass"]));
                if (pq != null && pq.TaiKhoan == "admin")
                {
                    Session["TaiKhoan"] = pq.TaiKhoan;
                    return RedirectToAction("Index", "Admin");
                }
                else if (pq != null && pq.TaiKhoan != "admin")
                {
                    ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                    Session["TaiKhoan"] = pq.TaiKhoan;
                    return RedirectToAction("Index", "Home");
                }
                else ViewData["Loi1"] = "Tên đăng nhập hoặc mật khẩu không đúng";
            return View();

        }
        //Thoát Tài Khoản
        public ActionResult Logout()
        {
            Session["TaiKhoan"] = null;
            Session["GIOHANG"] = null;
            return RedirectToAction("Index","Home");
        }
        //Đăng ký 
        public ActionResult DangKy()
        {
            return View();

        }
        [HttpPost]
        public ActionResult DangKy(FormCollection f)
        {
            if(f["cmnd"].Length!=9 && f["cmnd"].Length!=12)
            {
                ViewData["L1"] = "Vui lòng nhập đúng CMND";
                return View();
            }
            KHACHHANG k = new KHACHHANG();
            PHANQUYEN p = new PHANQUYEN();
            linhkienDataContext db = new linhkienDataContext();
            var query = from kh in db.KHACHHANGs where kh.CMND == f["cmnd"].ToString() select kh;
            if (query.ToList().Count != 0)
            {
                ViewData["L1"] = "Tài Khoản đã tồn tại";
                return View();
            }
            k.CMND = f["cmnd"];
            if (string.Compare(f["gioitinh"],"nam") == 0)
            {
                k.GioiTinh = false;
            }
            else {
                k.GioiTinh = true;
            }
            
            k.TenKH = f["hoten"];
            k.Ngaysinh = DateTime.Parse(f["ngaysinh"]);
            k.Email = f["email"];
            k.TrangThai = true;
            if (string.Compare(f["pass"], f["xnpass"]) != 0)
            {
                ViewData["L3"] = "Mật khẩu không trùng khớp";
                return View();
            }
            p.TrangThai = true;
            p.TaiKhoan = f["cmnd"];
            p.PassWord = EncryptMD5.MD5Hash(f["pass"]);
            p.MaCN = 2;
            db.PHANQUYENs.InsertOnSubmit(p);
            db.KHACHHANGs.InsertOnSubmit(k);
            db.SubmitChanges();
            return RedirectToAction("Login", "Home");
        }
        //Sua kh
        public ActionResult SuaKH()
        {
            var query = DB.KHACHHANGs.SingleOrDefault(m => m.CMND == Session["TaiKhoan"].ToString());
            return View(query);
        }
        [HttpPost]
        public ActionResult SuaKH(FormCollection f)
        {

            linhkienDataContext db = new linhkienDataContext();
            KHACHHANG khachhang = db.KHACHHANGs.First(m => m.CMND == f["CMND"]);
            if (ModelState.IsValid)
            {
                var a= f["Ngaysinh"];
                khachhang.TenKH = f["TenKH"];
                if (!string.IsNullOrEmpty(f["Ngaysinh"]))
                {
                    khachhang.Ngaysinh = DateTime.Parse(f["Ngaysinh"].ToString());
                }
                khachhang.GioiTinh =Convert.ToBoolean(f["GioiTinh"]);
                khachhang.Email = f["Email"];
                UpdateModel(khachhang);
                db.SubmitChanges();
            }
            return RedirectToAction("Index", "Home");
        }
        //ĐỔI PASS
        public ActionResult DoiPass()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DoiPass(FormCollection f)
        {
            PHANQUYEN pq = DB.PHANQUYENs.First(m => m.TaiKhoan == Session["TaiKhoan"].ToString());
            if (pq.PassWord ==EncryptMD5.MD5Hash(f["passold"]))
            {
                if (f["passnew"]==f["xnpass"])
                {
                    pq.PassWord =EncryptMD5.MD5Hash(f["passnew"]);
                    UpdateModel(pq);
                    DB.SubmitChanges();
                }
                else
                {
                    ViewData["Loi1"] = "Mật Khẩu không khớp vui lòng nhập lại!!!";
                    return View();
                }
            }
            else
            {
                ViewData["Loi2"] = "Mật Khẩu Hiện Tại Không Đúng!!!!!";
                return View();
            }
            return RedirectToAction("SuaKH","Home");
        }
        // Liên Hệ
        public ActionResult LienHe()
        {
            return View();
        }
        // Điểm Bán
        public ActionResult DiemBan()
        {
            return View();
        }
        public ActionResult Update()
        {
            var products = (from p in DB.DONDATHANGs
                            orderby p.MaDonHang descending
                            select p).Skip(0).Take(1);
            int a = 0;
            foreach (var item in products)
            {
                a = item.MaDonHang;
            }
            DONDATHANG dh = DB.DONDATHANGs.SingleOrDefault(m => m.MaDonHang == a);
            // nếu tôn tại đơn hàng
            if (dh != null)
            {

                dh.Dathanhtoan = true;
                UpdateModel(dh);
                DB.SubmitChanges();
            }
            return RedirectToAction("Index","Home");
        }
        public ActionResult Update1()
        {
            var products = (from p in DB.DONDATHANGs
                            orderby p.MaDonHang descending
                            select p).Skip(0).Take(1);
            int a = 0;
            foreach (var item in products)
            {
                a = item.MaDonHang;
            }
            DONDATHANG dh = DB.DONDATHANGs.SingleOrDefault(m => m.MaDonHang == a);
            // nếu tôn tại đơn hàng
            if (dh != null)
            {

                dh.Dathanhtoan = false;
                UpdateModel(dh);
                DB.SubmitChanges();
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult CauHoi()
        {
            return View();
        }
    public ActionResult QuenPass()
        {
            return View();
        }
        [HttpPost]
        public ActionResult QuenPass(FormCollection f)
        {
            var query = from kh in DB.KHACHHANGs where kh.CMND == f["CMND"] && kh.Email == f["email"] select kh;
            KHACHHANG k = query.SingleOrDefault();
            if(k != null)
            {
                // phan email
                StringBuilder Body = new StringBuilder();
                Body.Append("<p>Cảm ơn quý khách đã sử dụng dịch của chúng tôi. Dưới đây là mật khẩu mới của bạn:</p>");
                Body.Append("<p>Mật khẩu mới là: <b>1</b></p>");
                MailMessage mail = new MailMessage();
                mail.To.Add(k.Email);
                mail.From = new MailAddress("dangcongvinh19972004@gmail.com");
                mail.Subject = "Thay Đổi Mật Khẩu";
                mail.Body = Body.ToString();// phần thân của mail ở trên
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = new System.Net.NetworkCredential("dangcongvinh19972004@gmail.com", "vinhbinh1000");// tài khoản Gmail của bạn
                smtp.EnableSsl = true;
                smtp.Send(mail);
                PHANQUYEN pq = DB.PHANQUYENs.SingleOrDefault(m => m.TaiKhoan == k.CMND);
                pq.PassWord = EncryptMD5.MD5Hash("1");
                UpdateModel(pq);
                DB.SubmitChanges();
                return RedirectToAction("Login");
            }
            else
            {
                ViewData["Loi1"] = "Vui lòng Nhập đúng email đã đăng ký";
                return View();
            }
            
        }
        [HttpPost]
        [ValidateInput(false)]

        public ActionResult NhanKhuyenMai(DSKhuyenMai DSKM, FormCollection f)
        {
            DSKM.Email = f["email"];
            DSKM.SDT = f["sdt"];
            //Lưu vào CSDL
            DB.DSKhuyenMais.InsertOnSubmit(DSKM);
            DB.SubmitChanges();
            //gui mail thong bao
            StringBuilder Body = new StringBuilder();
            Body.Append("<p>Chúng tôi đã ghi nhận lại thông tin của quý khách.</p>");
            Body.Append("<p>Khi có khuyến mãi hệ thống sẽ thông báo thông tin cho quý khách</p>");
            MailMessage mail = new MailMessage();
            mail.To.Add(f["email"]);
            mail.From = new MailAddress("dangcongvinh19972004@gmail.com");
            mail.Subject = "Hệ Thống Cửa Hàng Linh Kiện 4T- Thông Báo";
            mail.Body = Body.ToString();// phần thân của mail ở trên
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("some.server.com");
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = new System.Net.NetworkCredential("dangcongvinh19972004@gmail.com", "vinhbinh1000");// tài khoản Gmail của bạn
            smtp.EnableSsl = true;
            smtp.Send(mail);
            return RedirectToAction("Index","Home");
        }

    }

}