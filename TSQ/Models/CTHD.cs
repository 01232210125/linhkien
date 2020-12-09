using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TSQ.Models
{
    public class CTHD
    {
        public int iMaDonHang { get; set; }
        public string sTenSP { get; set; }
        public string simg { get; set; }
        public int iSoLuong { get; set; }
        public double fDonGia { get; set; }
        public double fThanhTien { get; set; }
        public string sCMND { get; set; }
        public string sDiaChiGiaoHang { get; set; }
        public string sSDT { get; set; }
        public Boolean bTinhTrangGiaoHang { get; set; }
        public Boolean bThanhToan { get; set; }
    }
}