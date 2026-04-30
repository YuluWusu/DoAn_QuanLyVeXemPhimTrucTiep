using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn_QuanLyVeXemPhimTrucTiep.Models;

namespace DoAn_QuanLyVeXemPhimTrucTiep.Controllers
{
    public class BookingController : Controller
    {
        QL_VEPHIM_DuLieu data = new QL_VEPHIM_DuLieu();

        
        public ActionResult ChonGhe(string maLichChieu)
        {
            if (string.IsNullOrEmpty(maLichChieu))
            {
                return RedirectToAction("DanhSachPhim", "Movies");
            }

            var lichChieu = data.LICHCHIEUx.FirstOrDefault(l => l.MA_LICHCHIEU == maLichChieu);
            if (lichChieu == null)
            {
                return HttpNotFound();
            }

            var danhSachGhe = data.GHEs.Where(g => g.MA_PHONG == lichChieu.MA_PHONG).ToList();
            var gheDaDat = data.VEs.Where(v => v.MA_LICHCHIEU == maLichChieu).Select(v => v.MA_GHE).ToList();

            ViewBag.GheDaDat = gheDaDat;
            ViewBag.LichChieu = lichChieu;

            return View(danhSachGhe);
        }

        [HttpPost]
        public ActionResult ThanhToan(string maLichChieu, string selectedSeats)
        {
            if (string.IsNullOrEmpty(selectedSeats) || string.IsNullOrEmpty(maLichChieu))
            {
                return RedirectToAction("ChonGhe", new { maLichChieu = maLichChieu });
            }

            var lichChieu = data.LICHCHIEUx.FirstOrDefault(l => l.MA_LICHCHIEU == maLichChieu);
            if (lichChieu == null) return HttpNotFound();

            string[] listMaGhe = selectedSeats.Split(',');
            var gheDuocChon = data.GHEs.Where(g => listMaGhe.Contains(g.MA_GHE)).ToList();

            ViewBag.LichChieu = lichChieu;
            ViewBag.SelectedSeatsList = listMaGhe;

            return View(gheDuocChon);
        }

        [HttpPost]
        public ActionResult XuLyThanhToan(string maLichChieu, string selectedSeats, string hoTen, string soDienThoai, string Email)
        {
            var lichChieu = data.LICHCHIEUx.FirstOrDefault(l => l.MA_LICHCHIEU == maLichChieu);
            if (lichChieu == null) return HttpNotFound();

            KHACHHANG khachHang = null;
            
            if (Session["KhachHang"] != null)
            {
                var sessionKh = Session["KhachHang"] as KHACHHANG;
                khachHang = data.KHACHHANGs.FirstOrDefault(k => k.MA_KHACH == sessionKh.MA_KHACH);
            }

            if (khachHang == null)
            {
                khachHang = data.KHACHHANGs.FirstOrDefault(k => k.SDT == soDienThoai);
                if (khachHang == null)
                {
                    var allKhIds = data.KHACHHANGs.Select(k => k.MA_KHACH).ToList();
                    int maxKhId = 0;
                    foreach (var id in allKhIds)
                    {
                        if (!string.IsNullOrEmpty(id) && id.StartsWith("KH"))
                        {
                            if (int.TryParse(id.Substring(2), out int currentId))
                            {
                                if (currentId > maxKhId) maxKhId = currentId;
                            }
                        }
                    }
                    string newMaKhach = "KH" + (maxKhId + 1).ToString("D3");

                    khachHang = new KHACHHANG
                    {
                        MA_KHACH = newMaKhach,
                        HOTEN = hoTen,
                        SDT = soDienThoai,
                        EMAIL = Email,
                        VAITRO = "KhachHang",
                        NGAYTAO = DateTime.Now
                    };
                    data.KHACHHANGs.Add(khachHang);
                }
            }

            string[] listMaGhe = selectedSeats.Split(',');
            
            var allVeIds = data.VEs.Select(v => v.MA_VE).ToList();
            int maxVeId = 0;
            foreach (var id in allVeIds)
            {
                if (!string.IsNullOrEmpty(id) && id.StartsWith("VE"))
                {
                    if (int.TryParse(id.Substring(2), out int currentId))
                    {
                        if (currentId > maxVeId) maxVeId = currentId;
                    }
                }
            }

            foreach (var maGhe in listMaGhe)
            {
                var ghe = data.GHEs.FirstOrDefault(g => g.MA_GHE == maGhe);
                if (ghe != null)
                {
                    decimal phuThu = lichChieu.GIATVE_PHUTROI ?? 0;
                    decimal giaGhe = ghe.LOAIGHE.GIATVE;
                    decimal tongTien = giaGhe + phuThu;
                    
                    maxVeId++;
                    string maVeMoi = "VE" + maxVeId.ToString("D3");

                    VE veMoi = new VE
                    {
                        MA_VE = maVeMoi,
                        MA_KHACH = khachHang.MA_KHACH,
                        MA_LICHCHIEU = lichChieu.MA_LICHCHIEU,
                        MA_GHE = maGhe,
                        NGAYDAT = DateTime.Now,
                        TONGTIEN = tongTien,
                        TRANGTHAI = "Đã đặt"
                    };
                    data.VEs.Add(veMoi);
                }
            }

            data.SaveChanges();

            return RedirectToAction("DatVeThanhCong");
        }

        public ActionResult DatVeThanhCong()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LichSuVe()
        {
            if (Session["KhachHang"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var sessionKh = Session["KhachHang"] as KHACHHANG;
            
            // Lấy danh sách vé của khách hàng này, sắp xếp theo ngày đặt mới nhất
            var veList = data.VEs
                .Where(v => v.MA_KHACH == sessionKh.MA_KHACH)
                .OrderByDescending(v => v.NGAYDAT)
                .ToList();

            return View(veList);
        }
    }
}