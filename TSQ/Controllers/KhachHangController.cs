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
    public class KhachHangController : Controller
    {
        linhkienDataContext db = new linhkienDataContext();


        // GET: KhachHang
        public ActionResult Index()
        {
            return View();
        }
        // List danh sách Sản Phẩm
        public ActionResult ListKH(int? page)
        {
            //Tạo biến quy định số sản phẩm trên mỗi trang
            int pageSize = 6;
            //Tạo biến số trang
            int pageNum = (page ?? 1);
            var query = from kh in db.KHACHHANGs where kh.TrangThai== true select kh;
                        
            return View(query.ToPagedList(pageNum, pageSize));
        }
        //Xoa Khách Hàng
        [HttpGet]
        public ActionResult Delete(string id)
        {
            linhkienDataContext db = new linhkienDataContext();
            var query = from kh in db.KHACHHANGs
                        where kh.CMND == id select kh;
                        
            return View(query.Single());
        }
        [HttpPost]
        public ActionResult Delete(FormCollection f)
        {
            KHACHHANG dh = db.KHACHHANGs.First(d => d.CMND == f["CMND"]);
            dh.TrangThai = false;
            UpdateModel(dh);
            db.SubmitChanges();
            return RedirectToAction("ListKH");
        }
        
    }
}