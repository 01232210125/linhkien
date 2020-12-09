using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TSQ.Models;
using PagedList;
using PagedList.Mvc;
namespace TSQ.Controllers
{
    public class SanPhamController : Controller
    {
        // GET: SanPham
        linhkienDataContext db = new linhkienDataContext();
        public ActionResult Index(int id, int? page)
        {
            int pageSize = 3;
            int pageNum = (page ?? 1);
            linhkienDataContext db = new linhkienDataContext();

            ViewBag.ML = id;
            var query = from
                        dh in db.SANPHAMs
                        where dh.MaLoai == id && dh.TrangThai == true
                        select dh;

            return View(query.ToPagedList(pageNum, pageSize));
        }
        public ActionResult Loc(int id,string loc, int? page)
        {
            int pageSize = 6;
            int pageNum = (page ?? 1);
            ViewBag.x = loc;
            ViewBag.id = id;
            if (int.Parse(loc) == 2)
            {
                var query = from dh in db.SANPHAMs
                            where dh.MaLoai == id
                            orderby dh.GiaSP descending
                            select dh;
                return View((query.ToPagedList(pageNum, pageSize)));
            }
            if (int.Parse(loc) == 1)
            {
                var query = from dh in db.SANPHAMs
                            where dh.MaLoai == id
                            orderby dh.GiaSP ascending
                            select dh;
                return View((query.ToPagedList(pageNum, pageSize)));
            }
           
                return View();
        }
        public ActionResult SPBanChay(int id)
        {

            linhkienDataContext db = new linhkienDataContext();
            var a = from dh in db.CHITIETDONTHANGs
                    group dh by dh.MaSP
                    into spgr
                    orderby spgr.Count() descending
                    select spgr.Key;

            var query1 = from dh in db.SANPHAMs
                         join hd in a on dh.MaSP equals hd
                         where dh.MaLoai == id && dh.TrangThai == true
                         orderby db.CHITIETDONTHANGs.Where(m => m.MaSP == hd).Sum( m=> m.Soluong) descending
                         select dh;
            return PartialView(query1.Take(4));
        }
        public ActionResult SPCungTH(int id, int MaTH, int? page)
        {
            int pageSize = 4;
            int pageNum = (page ?? 1);
            linhkienDataContext db = new linhkienDataContext();
            ViewData["MaSP"] = id;
            var query = from
                        dh in db.SANPHAMs
                        where dh.MaTH == MaTH && dh.MaSP != id && dh.TrangThai == true
                        select dh;
            return PartialView(query.ToPagedList(pageNum, pageSize));
        }
        public ActionResult Detail(int id)
        {
            linhkienDataContext db = new linhkienDataContext();
            var query = from
                        dh in db.SANPHAMs
                        where dh.MaSP == id
                        select dh;
            return View(query.Single());
        }
        public ActionResult THUONGHIEU(int id, int? page)
        {
            int pageSize = 3;
            int pageNum = (page ?? 1);
            linhkienDataContext db = new linhkienDataContext();
            var query = from
                        dh in db.SANPHAMs
                        where dh.MaTH == id && dh.TrangThai == true
                        select dh;

            return View(query.ToPagedList(pageNum, pageSize));
        }
        public ActionResult SPMN(int? page)
        {
            int pageSize = 3;
            int pageNum = (page ?? 1);
            linhkienDataContext db = new linhkienDataContext();
            var query = from
                        dh in db.SANPHAMs
                        where dh.TrangThai == true
                        select dh;

            return View(query.OrderByDescending(m => m.MaSP).Take(10).ToPagedList(pageNum, pageSize));
        }
    }
}