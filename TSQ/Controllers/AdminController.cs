using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TSQ.Models;
using System.IO;
using System.Data.Linq;
using System.ComponentModel.DataAnnotations;
using PagedList;
using PagedList.Mvc;



namespace TSQ.Controllers
{
    public class AdminController : Controller
    {

        public string kiemtraKyTu(string a, int b)
        {
            if (a.Length > b)
            {
                return "Không được nhập quá " + b + " ký tự";

            }
            else if (string.IsNullOrEmpty(a))
            {
                return "Không được để trống";
            }
            return null;

        }
        linhkienDataContext db = new linhkienDataContext();
        // GET: Admin
        public ActionResult Index()
        {
            if(Session["TaiKhoan"] != null && Session["TaiKhoan"].ToString()=="admin")
            {
                return View(); 
            }
            return RedirectToAction("Index", "Home");
        }
        // List danh sách Sản Phẩm
        public ActionResult ListLK( int ? page)
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                //Tạo biến quy định số sản phẩm trên mỗi trang
                int pageSize = 6;
                //Tạo biến số trang
                int pageNum = (page ?? 1);
                var query = from dh in db.SANPHAMs
                            join th in db.THUONGHIEUs on  dh.MaTH equals th.MaTH
                            join km in db.KHUYENMAIs on dh.MaKM equals km.MaKM
                            join nsx in db.NHASANXUATs on dh.MaNSX equals nsx.MaNSX
                            join bh in db.BAOHANHs on dh.TGBH equals bh.TGBH
                            join loai in db.LOAI_SPs on dh.MaLoai equals loai.MaLoai
                            where dh.TrangThai == true
                            select new DH()
                            {
                                MaSP = dh.MaSP,
                                TenSP = dh.TenSP,
                                GiaSP = dh.GiaSP,
                                Ngaycapnhat = dh.Ngaycapnhat,
                                Soluongton = dh.Soluongton,
                                KM = km.HinhThucKM,
                                TH = th.TenTH,
                                NSX = nsx.TenNSX,
                                img = dh.img,
                                HTBH = bh.HinhThucBH,
                                Loai = loai.TenLoai
                            };
                return View(query.ToPagedList(pageNum, pageSize));
            }

            return RedirectToAction("Index", "Home");
            
        }

        //Get Thêm mới sách
        [HttpGet]
        public ActionResult ThemLK()
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                ViewBag.MaTH = new SelectList(db.THUONGHIEUs.ToList().OrderBy(th => th.TenTH), "MaTH", "TenTH");
                ViewBag.MaNSX = new SelectList(db.NHASANXUATs.ToList().OrderBy(nsx => nsx.TenNSX), "MaNSX", "TenNSX");
                ViewBag.MaKM = new SelectList(db.KHUYENMAIs.ToList().OrderBy(km => km.HinhThucKM), "MaKM", "HinhThucKM");
                ViewBag.TGBH = new SelectList(db.BAOHANHs.ToList().OrderBy(bh => bh.TGBH), "TGBH", "HinhThucBH");
                ViewBag.MaLoai = new SelectList(db.LOAI_SPs.ToList().OrderBy(bh => bh.MaLoai), "MaLoai", "TenLoai");
                return View();
            }

            return RedirectToAction("Index", "Home");
            //Dữ liệu láy vào DropdownList
            // Lấy danh sách từ table chủ đề,sắp xếp tăng dần theo tên nhà sản xuất , 
            //tên thương hiệu, khuyến mãi , thòi gain bảo hành

        }
        [HttpPost]
        [ValidateInput(false)]

        public ActionResult ThemLK(SANPHAM linhkien, HttpPostedFileBase fileupload, FormCollection f)
        {

            ViewBag.MaTH = new SelectList(db.THUONGHIEUs.ToList().OrderBy(th => th.TenTH), "MaTH", "TenTH");
            ViewBag.MaNSX = new SelectList(db.NHASANXUATs.ToList().OrderBy(nsx => nsx.TenNSX), "MaNSX", "TenNSX");
            ViewBag.MaKM = new SelectList(db.KHUYENMAIs.ToList().OrderBy(km => km.HinhThucKM), "MaKM", "HinhThucKM");
            ViewBag.TGBH = new SelectList(db.BAOHANHs.ToList().OrderBy(bh => bh.TGBH), "TGBH", "HinhThucBH");
            ViewBag.MaLoai = new SelectList(db.LOAI_SPs.ToList().OrderBy(bh => bh.MaLoai), "MaLoai", "TenLoai");
            if (kiemtraKyTu(f["TenSP"], 100) != null)
            {
                ViewBag.ThongBaoLoi = kiemtraKyTu(f["TenSP"], 100);
                return View(linhkien);
            }

            if (fileupload == null)
            {

                ViewBag.Thongbao = "Vui lòng chọn ảnh sản phẩm";
                return View(linhkien);

            }
            else if (ModelState.IsValid)
            {

                //Lưu tên file ;
                // dong bat loi file la anh
                var filename = Path.GetFileName(fileupload.FileName);
                string fileExtexsion = Path.GetExtension(fileupload.FileName);
                if (fileExtexsion.ToLower() == ".gif" || fileExtexsion.ToLower() == ".png" || fileExtexsion.ToLower() == ".jpg" || fileExtexsion.ToLower() == ".jpeg")
                {
                    //Lưu đường dẫn của File 
                    var path = Path.Combine(Server.MapPath("~/img"), filename);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        //Lưu hình ảnh vào đường dẫn
                        fileupload.SaveAs(path);
                    }
                    linhkien.img = filename;
                    linhkien.Ngaycapnhat = DateTime.Now;
                    //Lưu vào CSDL
                    db.SANPHAMs.InsertOnSubmit(linhkien);
                    db.SubmitChanges();
                }
                else
                {
                    ViewBag.Thongbao = "Vui lòng chọn file là ảnh";
                    return View(linhkien);
                }
            }

            return RedirectToAction("ListLK");

        }
        // Sửa linh kiện
        //GET LINHKIEN/MaSP
        [HttpGet]
        public ActionResult SuaLK(int id)
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                SANPHAM linhkien = db.SANPHAMs.SingleOrDefault(dh => dh.MaSP == id);


                if ( linhkien== null)
                {
                    return HttpNotFound();
                }

                // Đưa dữ liệu vào dropdown list
                ViewBag.MaTH = new SelectList(db.THUONGHIEUs.ToList().OrderBy(th => th.TenTH), "MaTH", "TenTH", linhkien.MaTH);
                ViewBag.MaNSX = new SelectList(db.NHASANXUATs.ToList().OrderBy(nsx => nsx.TenNSX), "MaNSX", "TenNSX", linhkien.MaNSX);
                ViewBag.MaKM = new SelectList(db.KHUYENMAIs.ToList().OrderBy(km => km.HinhThucKM), "MaKM", "HinhThucKM", linhkien.MaKM);
                ViewBag.TGBH = new SelectList(db.BAOHANHs.ToList().OrderBy(bh => bh.TGBH), "TGBH", "HinhThucBH", linhkien.TGBH);
                ViewBag.MaLoai = new SelectList(db.LOAI_SPs.ToList().OrderBy(bh => bh.MaLoai), "MaLoai", "TenLoai",linhkien.MaLoai);
                return View(linhkien);
            }

            return RedirectToAction("Index", "Home");
            
        }
        //POST BOOK/ID BOOK/ID
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaLK( HttpPostedFileBase fileupload, FormCollection f)
        {
            SANPHAM linhkien = db.SANPHAMs.First(d => d.MaSP == int.Parse(f["MaSP"]));
            ViewBag.MaTH = new SelectList(db.THUONGHIEUs.ToList().OrderBy(th => th.TenTH), "MaTH", "TenTH");
            ViewBag.MaNSX = new SelectList(db.NHASANXUATs.ToList().OrderBy(nsx => nsx.TenNSX), "MaNSX", "TenNSX");
            ViewBag.MaKM = new SelectList(db.KHUYENMAIs.ToList().OrderBy(km => km.HinhThucKM), "MaKM", "HinhThucKM");
            ViewBag.TGBH = new SelectList(db.BAOHANHs.ToList().OrderBy(bh => bh.TGBH), "TGBH", "HinhThucBH");
            ViewBag.MaLoai = new SelectList(db.LOAI_SPs.ToList().OrderBy(bh => bh.MaLoai), "MaLoai", "TenLoai");
            if (kiemtraKyTu(f["TenSP"], 100) != null)
            {
                ViewBag.ThongBaoLoi = kiemtraKyTu(f["TenSP"], 100);
                return View(linhkien);
            }

            if (ModelState.IsValid)
            {

                //Lưu tên file ;
                // dong bat loi file la anh

                linhkien.TenSP = f["TenSP"];
                linhkien.Mota = f["Mota"];
                linhkien.GiaSP = double.Parse(f["GiaSP"]);
                linhkien.Soluongton = int.Parse(f["Soluongton"]);
                linhkien.MaTH = int.Parse(f["MaTH"]);
                linhkien.MaKM = int.Parse(f["MaKM"]);
                linhkien.MaNSX = int.Parse(f["MaNSX"]);
                linhkien.TGBH = int.Parse(f["TGBH"]);
                linhkien.Ngaycapnhat = DateTime.Now;
                linhkien.TrangThai = true;
                if (fileupload != null)
                {
                    var filename = Path.GetFileName(fileupload.FileName);
                    string fileExtexsion = Path.GetExtension(fileupload.FileName);
                    if (fileExtexsion.ToLower() == ".gif" || fileExtexsion.ToLower() == ".png" || fileExtexsion.ToLower() == ".jpg" || fileExtexsion.ToLower() == ".jpeg")
                    {
                        //Lưu đường dẫn của File 
                        var path = Path.Combine(Server.MapPath("~/img"), filename);
                        if (System.IO.File.Exists(path))
                        {
                            ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                        }
                        else
                        {
                            //Lưu hình ảnh vào đường dẫn
                            fileupload.SaveAs(path);
                        }
                        linhkien.img = filename;
                    }
                    else
                    {
                        ViewBag.Thongbao = "Vui lòng chọn file là ảnh";
                        return View(linhkien);
                    }

                }
                //Lưu vào CSDL
                UpdateModel(linhkien);
                db.SubmitChanges();
            }
            return RedirectToAction("ListLK");
        }

        //Xoa SanPham
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                linhkienDataContext db = new linhkienDataContext();
                var query = from dh in db.SANPHAMs
                            join th in db.THUONGHIEUs on dh.MaTH equals th.MaTH
                            join km in db.KHUYENMAIs on dh.MaKM equals km.MaKM
                            join nsx in db.NHASANXUATs on dh.MaNSX equals nsx.MaNSX
                            join bh in db.BAOHANHs on dh.TGBH equals bh.TGBH
                            join ls in db.LOAI_SPs on dh.MaLoai equals ls.MaLoai
                            where dh.MaSP == id
                            select new DH()
                            {
                                MaSP = dh.MaSP,
                                TenSP = dh.TenSP,
                                GiaSP = dh.GiaSP,
                                Ngaycapnhat = dh.Ngaycapnhat,
                                Soluongton = dh.Soluongton,
                                KM = km.HinhThucKM,
                                TH = th.TenTH,
                                NSX = nsx.TenNSX,
                                img = dh.img,
                                HTBH = bh.HinhThucBH,
                                Loai = ls.TenLoai
                            };
                return View(query.Single());
            }

            return RedirectToAction("Index", "Home");
           
        }
        [HttpPost]
        public ActionResult Delete(FormCollection f)
        {
            SANPHAM dh = db.SANPHAMs.First(d => d.MaSP == int.Parse(f["MaSP"]));
            dh.TrangThai = false;
            UpdateModel(dh);
            db.SubmitChanges();
            return RedirectToAction("ListLK");
        }
        // Nhap Hang
        [HttpGet]
        public ActionResult NhapSLSP(int ? page)
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                int pageSize = 6;
                int pageNum = (page ?? 1);
                var query = from dh in db.SANPHAMs where dh.TrangThai == true select new DH() { MaSP = dh.MaSP, TenSP = dh.TenSP, Soluongton = dh.Soluongton, img = dh.img };
              
                return View(query.ToPagedList(pageNum, pageSize));
            }

            return RedirectToAction("Index", "Home");

        }
        [HttpPost]
        public ActionResult CapNhatSLSP(int id,FormCollection f)
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                SANPHAM dh = db.SANPHAMs.SingleOrDefault(d => d.MaSP == id);
                dh.Soluongton = dh.Soluongton + int.Parse(f["SoLuong"]);
                UpdateModel(dh);
                db.SubmitChanges();
                return RedirectToAction("NhapSLSP");
            }

            return RedirectToAction("Index", "Home");

        }

        //List hoa don
        public ActionResult ListHoaDon(int ? page)
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                int pageSize = 6;
                //Tạo biến số trang
                int pageNum = (page ?? 1);
                var query = from hd in db.DONDATHANGs
                            where hd.TrangThai == true
                            select hd;

                return View(query.ToPagedList(pageNum, pageSize));
            }

            return RedirectToAction("Index", "Home");
            //Tạo biến quy định số sản phẩm trên mỗi trang

        }
        [HttpPost]
        public ActionResult ListHoaDon(int? page,FormCollection f)// dua thang nam vao de loc
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                int pageSize = 6;
                //Tạo biến số trang
                int pageNum = (page ?? 1);

                //Tạo biến quy định số sản phẩm trên mỗi trang
                if (String.IsNullOrEmpty(f["ThangNam"]))
                {
                    return RedirectToAction("ListHoaDon", "Admin");
                }
                else
                {
                    ViewBag.TB = f["ThangNam"];
                    var query = from hd in db.DONDATHANGs
                                where hd.TrangThai == true && Convert.ToDateTime(hd.Ngaydat).Month == Convert.ToDateTime(f["ThangNam"]).Month
                                && Convert.ToDateTime(hd.Ngaydat).Year == Convert.ToDateTime(f["ThangNam"]).Year
                                select hd;
                    ViewBag.TongTien = TongTienTN(Convert.ToDateTime(f["ThangNam"]).Month, Convert.ToDateTime(f["ThangNam"]).Year);
                    ViewBag.Thang = Convert.ToDateTime(f["ThangNam"]).Month;
                    ViewBag.Nam = Convert.ToDateTime(f["ThangNam"]).Year;
                    return View(query.ToPagedList(pageNum, pageSize));
                }
            }
            return RedirectToAction("Index", "Home");
        }
        //Chi Tiet Hoa Don
        [HttpGet]
        public ActionResult ChiTiet(int  id, string url)
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                var query = from ct in db.CHITIETDONTHANGs
                            join d in db.SANPHAMs
                            on ct.MaSP equals d.MaSP
                            where ct.MaDonHang == id
                            select new CTHD()
                            {

                                iMaDonHang = ct.MaDonHang,
                                sTenSP = d.TenSP,
                                simg = d.img,
                                iSoLuong = int.Parse(ct.Soluong.ToString()),
                                fDonGia = double.Parse(d.GiaSP.ToString()),
                                fThanhTien = double.Parse(d.GiaSP.ToString())
                            };
                ViewBag.Ma = id;
                ViewData["Url"] = url;
                return View(query);
            }

            return RedirectToAction("Index", "Home");

        }
        //Xóa Hóa Đơn
        public ActionResult XoaHoaDon(int id)
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                DONDATHANG dh = db.DONDATHANGs.First(d => d.MaDonHang == id);
                dh.TrangThai = false;
                UpdateModel(dh);
                db.SubmitChanges();
                return RedirectToAction("ListHoaDon");
            }

            return RedirectToAction("Index", "Home");

        }

        // Tinh trang giao hàng
        [HttpGet]
        public ActionResult GiaoHang(int? page)
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                int pageSize = 6;
                int pageNum = (page ?? 1);
                var query = from gh in db.DONDATHANGs
                            where gh.TrangThai == true
                            select new CTHD()
                            {
                                iMaDonHang = gh.MaDonHang,
                                sCMND = gh.CMND,
                                bTinhTrangGiaoHang = Boolean.Parse(gh.Tinhtranggiaohang.ToString()),
                                bThanhToan = Boolean.Parse(gh.Dathanhtoan.ToString()),
                                sDiaChiGiaoHang = gh.DiaChiLienHe,
                                sSDT = gh.SDTLienHe
                            };
          
                return View(query.ToPagedList(pageNum, pageSize));
            }

            return RedirectToAction("Index", "Home");

        }
        [HttpPost]
        public ActionResult GiaoHang(int id, FormCollection f)
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                DONDATHANG gh = db.DONDATHANGs.SingleOrDefault(d => d.MaDonHang == id);
                gh.Tinhtranggiaohang = Convert.ToBoolean(f["TinhTrangGiaoHang"]);
                gh.Dathanhtoan = Convert.ToBoolean(f["ThanhToan"]);
                UpdateModel(gh);
                db.SubmitChanges();
                return RedirectToAction("GiaoHang");
            }

            return RedirectToAction("Index", "Home");

        }
        // Doi pass
        public ActionResult DoiPass()
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                return View();
            }

            return RedirectToAction("Index", "Home");
            
        }
        [HttpPost]
        public ActionResult DoiPass(FormCollection f)
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                PHANQUYEN pq = db.PHANQUYENs.First(m => m.TaiKhoan == Session["TaiKhoan"].ToString());
                if (pq.PassWord == EncryptMD5.MD5Hash(f["passold"]))
                {
                    if (f["passnew"] == f["xnpass"])
                    {
                        pq.PassWord = EncryptMD5.MD5Hash(f["passnew"]);
                        UpdateModel(pq);
                        db.SubmitChanges();
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
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Index", "Home");

        }
        // Tong Tien Cac Thang
        public double TongTienTN(int thang, int nam)
        {
            double s = 0;
            var query = from hd in db.DONDATHANGs
                        where hd.TrangThai == true && Convert.ToDateTime(hd.Ngaydat).Month == thang
                        && Convert.ToDateTime(hd.Ngaydat).Year == nam && hd.Dathanhtoan ==true
                        select hd;
            if(query.ToList().Count != 0)
            {
                s = query.ToList().Sum(m => m.TongTien);
            }
            return s;
        }

        public ActionResult ThongKe()
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                ViewBag.t1 = TongTienTN(1, 2018);
                ViewBag.t2 = TongTienTN(2, 2018);
                ViewBag.t3 = TongTienTN(3, 2018);
                ViewBag.t4 = TongTienTN(4, 2018);
                ViewBag.t5 = TongTienTN(5, 2018);
                ViewBag.t6 = TongTienTN(6, 2018);
                ViewBag.t7 = TongTienTN(7, 2018);
                ViewBag.t8 = TongTienTN(8, 2018);
                ViewBag.t9 = TongTienTN(9, 2018);
                ViewBag.t10 = TongTienTN(10, 2018);
                ViewBag.t11 = TongTienTN(11, 2018);
                ViewBag.t12 = TongTienTN(12, 2018);
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        // lich su
        [HttpGet]
        public ActionResult LichSu()
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                var dongho = db.SANPHAMs.Where(m => m.TrangThai == false);
                ViewBag.DH = dongho.ToList();
                var kh = db.KHACHHANGs.Where(m => m.TrangThai == false);
                ViewBag.KH = kh.ToList();
                var th = db.THUONGHIEUs.Where(m => m.TrangThai == false);
                ViewBag.TH = th.ToList();
                var nsx = db.NHASANXUATs.Where(m => m.TrangThai == false);
                ViewBag.NSX = nsx.ToList();
                return View();
            }

            return RedirectToAction("Index", "Home");

        }
        public ActionResult KhoiPhucKH( string id)
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(m => m.CMND == id);
                kh.TrangThai = true;
                UpdateModel(kh);
                db.SubmitChanges();
                return RedirectToAction("LichSu", "Admin");
            }

            return RedirectToAction("Index", "Home");

        }
        public ActionResult KhoiPhucSP(int id)
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
               SANPHAM kh = db.SANPHAMs.SingleOrDefault(m => m.MaSP == id);
                kh.TrangThai = true;
                UpdateModel(kh);
                db.SubmitChanges();
                return RedirectToAction("LichSu", "Admin");
            }

            return RedirectToAction("Index", "Home");

        }
        public ActionResult KhoiPhucTH(int id)
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                THUONGHIEU kh = db.THUONGHIEUs.SingleOrDefault(m => m.MaTH == id);
                kh.TrangThai = true;
                UpdateModel(kh);
                db.SubmitChanges();
                return RedirectToAction("LichSu", "Admin");
            }

            return RedirectToAction("Index", "Home");

        }
        public ActionResult KhoiPhucNSX(int id)
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                NHASANXUAT kh = db.NHASANXUATs.SingleOrDefault(m => m.MaNSX == id);
                kh.TrangThai = true;
                UpdateModel(kh);
                db.SubmitChanges();
                return RedirectToAction("LichSu", "Admin");
            }

            return RedirectToAction("Index", "Home");

        }
        // Dich vu
        public ActionResult NhaSanXuat()
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                var query = db.NHASANXUATs.Where(m => m.TrangThai == true);
                return View(query);
            }

            return RedirectToAction("Index", "Home");

        }
        public ActionResult ThemNSX()
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                return View();
            }

            return RedirectToAction("Index", "Home");
           
        }
        [HttpPost]
        [ValidateInput(false)]

        public ActionResult ThemNSX(NHASANXUAT nsx, FormCollection f)
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                if (kiemtraKyTu(f["TenNSX"], 50) != null)
                {
                    ViewBag.ThongBaoLoi = kiemtraKyTu(f["TenNSX"], 50);
                    return View(nsx);
                }
                if (kiemtraKyTu(f["Diachi"], 200) != null)
                {
                    ViewBag.ThongBaoLoi = kiemtraKyTu(f["Diachi"], 200);
                    return View(nsx);
                }
                if (kiemtraKyTu(f["DienThoai"], 15) != null)
                {
                    ViewBag.ThongBaoLoi = kiemtraKyTu(f["DienThoai"], 15);
                    return View(nsx);
                }
                else if (ModelState.IsValid)
                {

                    //Lưu tên file ;
                    // dong bat loi file la anh

                    //Lưu vào CSDL
                    nsx.TrangThai = true;
                    db.NHASANXUATs.InsertOnSubmit(nsx);
                    db.SubmitChanges();
                }
                return RedirectToAction("NhaSanXuat");
            }

            return RedirectToAction("Index", "Home");

        }
        public ActionResult XoaNSX(int id)
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                NHASANXUAT kh = db.NHASANXUATs.SingleOrDefault(m => m.MaNSX == id);
                kh.TrangThai = false;
                UpdateModel(kh);
                db.SubmitChanges();
                return RedirectToAction("NhaSanXuat", "Admin");
            }

            return RedirectToAction("Index", "Home");

        }
        //Thuong Hieu
        public ActionResult ThuongHieu()
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                var query = db.THUONGHIEUs.Where(m => m.TrangThai == true);
                return View(query);
            }

            return RedirectToAction("Index", "Home");

          
        }
        public ActionResult XoaTH(int id)
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                THUONGHIEU kh = db.THUONGHIEUs.SingleOrDefault(m => m.MaTH == id);
                kh.TrangThai = false;
                UpdateModel(kh);
                db.SubmitChanges();
                return RedirectToAction("ThuongHieu", "Admin");
            }

            return RedirectToAction("Index", "Home");

        }
        // Them TH
        [HttpGet]
        public ActionResult ThemTH()
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                return View();
            }

            return RedirectToAction("Index", "Home");
            
        }
        [HttpPost]
        [ValidateInput(false)]

        public ActionResult ThemTH(THUONGHIEU thuonghieu, HttpPostedFileBase fileupload, FormCollection f)
        {

            if (Session["TaiKhoan"].ToString() == "admin")
            {
                if (kiemtraKyTu(f["TenTH"], 50) != null)
                {
                    ViewBag.ThongBaoLoi = kiemtraKyTu(f["TenTH"], 50);
                    return View(thuonghieu);
                }

                if (fileupload == null)
                {

                    ViewBag.Thongbao = "Vui lòng chọn ảnh sản phẩm";
                    return View(thuonghieu);

                }
                else if (ModelState.IsValid)
                {

                    //Lưu tên file ;
                    // dong bat loi file la anh
                    var filename = Path.GetFileName(fileupload.FileName);
                    string fileExtexsion = Path.GetExtension(fileupload.FileName);
                    if (fileExtexsion.ToLower() == ".gif" || fileExtexsion.ToLower() == ".png" || fileExtexsion.ToLower() == ".jpg" || fileExtexsion.ToLower() == ".jpeg")
                    {
                        //Lưu đường dẫn của File 
                        var path = Path.Combine(Server.MapPath("~/img"), filename);
                        if (System.IO.File.Exists(path))
                        {
                            ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                        }
                        else
                        {
                            //Lưu hình ảnh vào đường dẫn
                            fileupload.SaveAs(path);
                        }
                        thuonghieu.imgHeader = filename;
                        thuonghieu.TenTH = f["TenTH"].ToString();
                        thuonghieu.TrangThai = true;
                        db.THUONGHIEUs.InsertOnSubmit(thuonghieu);
                        db.SubmitChanges();

                    }
                    else
                    {
                        ViewBag.Thongbao = "Vui lòng chọn file là ảnh";
                        return View(thuonghieu);
                    }
                }

                return RedirectToAction("ThuongHieu");
            }

            return RedirectToAction("Index", "Home");



        }
        // KHUYEN MAI
        public ActionResult KhuyenMai()
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                var query = db.KHUYENMAIs.Where(m => m.TrangThai == true);
                return View(query);
            }

            return RedirectToAction("Index", "Home");

        }
        // Bảo HÀNH
        public ActionResult BaoHanh()
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                var query = db.BAOHANHs.Where(m => m.TrangThai == true);
                return View(query);
            }
            else  return RedirectToAction("Index", "Home");
        }
        public ActionResult DSNKM(int? page)
        {
            if (Session["TaiKhoan"].ToString() == "admin")
            {
                //Tạo biến quy định số sản phẩm trên mỗi trang
                int pageSize = 6;
                //Tạo biến số trang
                int pageNum = (page ?? 1);
                var query = from ds in db.DSKhuyenMais
                            select ds;
                return View(query.ToPagedList(pageNum, pageSize));
            }

            return RedirectToAction("Index", "Home");

        }
    }
}