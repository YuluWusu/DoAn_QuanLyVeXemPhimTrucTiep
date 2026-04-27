using DoAn_QuanLyVeXemPhimTrucTiep.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace DoAn_QuanLyVeXemPhimTrucTiep.Controllers
{
    public class HomeController : Controller
    {
        private QL_VEPHIM_DuLieu data = new QL_VEPHIM_DuLieu();
        public ActionResult TrangChu()
        {
            List<PHIM> dangChieu = data.PHIMs.Where(p => p.TRANGTHAI == "Đang chiếu").Take(5).ToList();
            List<PHIM> sapChieu = data.PHIMs.Where(p => p.TRANGTHAI == "Sắp chiếu").Take(6).ToList();
            List<PHIM> hinhNen = data.PHIMs.Take(3).ToList();
            ViewBag.DangChieu = dangChieu;
            ViewBag.SapChieu = sapChieu;
            ViewBag.HinhNen = hinhNen;
            return View();
        }
        public ActionResult GioiThieu()
        {
            return View();
        }
    }
}