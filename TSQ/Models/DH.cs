using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TSQ.Models
{
    public class DH
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public double? GiaSP { get; set; }
        public DateTime? Ngaycapnhat { get; set; }
        public int? Soluongton { get; set; }
        public string KM { get; set; }
        public int MaKM { get; set; }
        public string TH { get; set; }
        public int MaTH { get; set; }
        public string NSX { get; set; }
        public int MaNSX { get; set; }
        public string img { get; set; }
        public int TGBH{ get; set; }
        public string HTBH { get; set; }
        public string Loai { get; set; }
    }
}