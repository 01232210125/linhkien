using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TSQ.Models;

namespace TSQ.Models
{
    public class GIOHANG
    {
        linhkienDataContext db = new linhkienDataContext();
        public int iMaSP { get; set; }
        public string sTenSP  { get; set; }
        public string simg { get; set; }
        public double fDonGia { get; set; }
        public int iSoLuong { get; set; }
        public double fThanhTien {
            get { return iSoLuong * fDonGia; }
        }
        public GIOHANG (int MaSP)
        {
            iMaSP = MaSP;
            SANPHAM dongho = db.SANPHAMs.Single(m => m.MaSP == iMaSP);
            sTenSP = dongho.TenSP;
            simg = dongho.img;
            fDonGia = double.Parse(dongho.GiaSP.ToString());
            iSoLuong = 1;
        }
    }
}