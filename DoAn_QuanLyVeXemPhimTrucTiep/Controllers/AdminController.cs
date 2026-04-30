using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn_QuanLyVeXemPhimTrucTiep.Models;
using System.IO;

namespace DoAn_QuanLyVeXemPhimTrucTiep.Controllers
{
    public class AdminController : Controller
    {
        QL_VEPHIM_DuLieu data = new QL_VEPHIM_DuLieu();

        // GET: Admin/Index (Dashboard)
        public ActionResult Index()
        {
            ViewBag.TotalRevenue = data.VEs.Sum(v => (decimal?)v.TONGTIEN) ?? 0;
            ViewBag.TotalTickets = data.VEs.Count();
            ViewBag.TotalMovies = data.PHIMs.Count();
            ViewBag.TotalCustomers = data.KHACHHANGs.Count();

            // Lấy danh sách 5 vé gần nhất
            ViewBag.RecentTickets = data.VEs.OrderByDescending(v => v.NGAYDAT).Take(5).ToList();

            return View();
        }

        // GET: Admin/QuanLyPhim
        public ActionResult QuanLyPhim()
        {
            var phims = data.PHIMs.OrderByDescending(p => p.MA_PHIM).ToList();
            return View(phims);
        }

        // GET: Admin/ThemPhim
        public ActionResult ThemPhim()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemPhim(PHIM phim, HttpPostedFileBase fileUpload)
        {
            if (ModelState.IsValid)
            {
                // Generate MA_PHIM
                string currentMax = data.PHIMs.Max(p => p.MA_PHIM);
                int nextId = 1;
                if (!string.IsNullOrEmpty(currentMax) && currentMax.StartsWith("P"))
                {
                    if (int.TryParse(currentMax.Substring(1), out int currId))
                    {
                        nextId = currId + 1;
                    }
                }
                phim.MA_PHIM = "P" + nextId.ToString("D2");

                // Handle file upload
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/HinhAnh/"), fileName);
                    fileUpload.SaveAs(path);
                    phim.POSTER = fileName;
                }

                data.PHIMs.Add(phim);
                data.SaveChanges();
                return RedirectToAction("QuanLyPhim");
            }
            return View(phim);
        }

        // GET: Admin/SuaPhim/5
        public ActionResult SuaPhim(string id)
        {
            var phim = data.PHIMs.FirstOrDefault(p => p.MA_PHIM == id);
            if (phim == null) return HttpNotFound();
            return View(phim);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaPhim(PHIM phim, HttpPostedFileBase fileUpload)
        {
            if (ModelState.IsValid)
            {
                var p = data.PHIMs.FirstOrDefault(x => x.MA_PHIM == phim.MA_PHIM);
                if (p != null)
                {
                    p.TEN_PHIM = phim.TEN_PHIM;
                    p.THOILUONG = phim.THOILUONG;
                    p.NGAYKHOICHIEU = phim.NGAYKHOICHIEU;
                    p.MOTA = phim.MOTA;
                    p.DAO_DIEN = phim.DAO_DIEN;
                    p.DIEN_VIEN = phim.DIEN_VIEN;
                    p.GIOI_HAN_TUOI = phim.GIOI_HAN_TUOI;
                    p.TRANGTHAI = phim.TRANGTHAI;

                    if (fileUpload != null && fileUpload.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(fileUpload.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/HinhAnh/"), fileName);
                        fileUpload.SaveAs(path);
                        p.POSTER = fileName;
                    }
                    data.SaveChanges();
                    return RedirectToAction("QuanLyPhim");
                }
            }
            return View(phim);
        }

        // POST: Admin/XoaPhim/5
        [HttpPost]
        public ActionResult XoaPhim(string id)
        {
            var p = data.PHIMs.FirstOrDefault(x => x.MA_PHIM == id);
            if (p != null)
            {
                data.PHIMs.Remove(p);
                data.SaveChanges();
            }
            return RedirectToAction("QuanLyPhim");
        }
        // GET: Admin/QuanLyRap
        public ActionResult QuanLyRap()
        {
            ViewBag.Title = "Quản lý Rạp chiếu";
            var raps = data.RAPCHIEUx.ToList();
            return View(raps);
        }

        public ActionResult ThemRap()
        {
            ViewBag.Title = "Thêm Rạp chiếu";
            return View();
        }

        [HttpPost]
        public ActionResult ThemRap(RAPCHIEU rap)
        {
            if (ModelState.IsValid)
            {
                string currentMax = data.RAPCHIEUx.Max(r => r.MA_RAP);
                int nextId = 1;
                if (!string.IsNullOrEmpty(currentMax) && currentMax.StartsWith("R"))
                {
                    if (int.TryParse(currentMax.Substring(1), out int currId))
                    {
                        nextId = currId + 1;
                    }
                }
                rap.MA_RAP = "R" + nextId.ToString("D2");
                
                data.RAPCHIEUx.Add(rap);
                data.SaveChanges();
                return RedirectToAction("QuanLyRap");
            }
            return View(rap);
        }

        public ActionResult SuaRap(string id)
        {
            ViewBag.Title = "Sửa Rạp chiếu";
            var rap = data.RAPCHIEUx.FirstOrDefault(r => r.MA_RAP == id);
            if (rap == null) return HttpNotFound();
            return View(rap);
        }

        [HttpPost]
        public ActionResult SuaRap(RAPCHIEU rap)
        {
            if (ModelState.IsValid)
            {
                var r = data.RAPCHIEUx.FirstOrDefault(x => x.MA_RAP == rap.MA_RAP);
                if (r != null)
                {
                    r.TEN_RAP = rap.TEN_RAP;
                    r.DIACHI = rap.DIACHI;
                    data.SaveChanges();
                    return RedirectToAction("QuanLyRap");
                }
            }
            return View(rap);
        }

        [HttpPost]
        public ActionResult XoaRap(string id)
        {
            var r = data.RAPCHIEUx.FirstOrDefault(x => x.MA_RAP == id);
            if (r != null)
            {
                // Note: In a real system, you might need to check if there are rooms associated with this rap
                if (data.RAPCHIEUx.Any(p => p.MA_RAP == id))
                {
                    TempData["Error"] = "Không thể xóa rạp này vì đang có phòng chiếu trực thuộc.";
                    return RedirectToAction("QuanLyRap");
                }
                data.RAPCHIEUx.Remove(r);
                data.SaveChanges();
            }
            return RedirectToAction("QuanLyRap");
        }
        // GET: Admin/QuanLyPhong
        public ActionResult QuanLyPhong(string idRap)
        {
            ViewBag.Title = "Quản lý Phòng chiếu";
            var phongs = data.PHONGCHIEUx.AsQueryable();
            if (!string.IsNullOrEmpty(idRap))
            {
                phongs = phongs.Where(p => p.MA_RAP == idRap);
            }
            ViewBag.Raps = new SelectList(data.RAPCHIEUx.ToList(), "MA_RAP", "TEN_RAP", idRap);
            return View(phongs.ToList());
        }

        public ActionResult ThemPhong()
        {
            ViewBag.Title = "Thêm Phòng chiếu";
            ViewBag.MA_RAP = new SelectList(data.RAPCHIEUx.ToList(), "MA_RAP", "TEN_RAP");
            return View();
        }

        [HttpPost]
        public ActionResult ThemPhong(PHONGCHIEU phong)
        {
            if (ModelState.IsValid)
            {
                string currentMax = data.PHONGCHIEUx.Max(p => p.MA_PHONG);
                int nextId = 1;
                if (!string.IsNullOrEmpty(currentMax) && currentMax.StartsWith("PC"))
                {
                    if (int.TryParse(currentMax.Substring(2), out int currId))
                    {
                        nextId = currId + 1;
                    }
                }
                phong.MA_PHONG = "PC" + nextId.ToString("D2");
                
                data.PHONGCHIEUx.Add(phong);
                data.SaveChanges();

                int maxGhe = phong.TONGSOGHE ?? 0;
                char[] rows = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K' };
                int cols = 10;
                int currentGhe = 0;
                
                for(int r = 0; r < rows.Length; r++)
                {
                    for(int c = 1; c <= cols; c++)
                    {
                        if(currentGhe >= maxGhe) break;

                        string soghe = rows[r].ToString() + c.ToString();
                        
                        string currentMaxGhe = data.GHEs.Max(g => g.MA_GHE);
                        int nextGheId = 1;
                        if (!string.IsNullOrEmpty(currentMaxGhe) && currentMaxGhe.StartsWith("G"))
                        {
                            if (int.TryParse(currentMaxGhe.Substring(1), out int currId))
                            {
                                nextGheId = currId + 1;
                            }
                        }
                        
                        string loaiGhe = "Thường";
                        if(r >= rows.Length - 2)
                        {
                            loaiGhe = "VIP";
                        }

                        data.GHEs.Add(new GHE {
                            MA_GHE = "G" + nextGheId.ToString("D4"),
                            MA_PHONG = phong.MA_PHONG,
                            SOGHE = soghe,
                            MA_LOAIGHE = loaiGhe
                        });
                        
                        data.SaveChanges();
                        currentGhe++;
                    }
                }

                return RedirectToAction("QuanLyPhong");
            }
            ViewBag.MA_RAP = new SelectList(data.RAPCHIEUx.ToList(), "MA_RAP", "TEN_RAP", phong.MA_RAP);
            return View(phong);
        }

        public ActionResult SuaPhong(string id)
        {
            ViewBag.Title = "Sửa Phòng chiếu";
            var phong = data.PHONGCHIEUx.FirstOrDefault(p => p.MA_PHONG == id);
            if (phong == null) return HttpNotFound();
            ViewBag.MA_RAP = new SelectList(data.RAPCHIEUx.ToList(), "MA_RAP", "TEN_RAP", phong.MA_RAP);
            return View(phong);
        }

        [HttpPost]
        public ActionResult SuaPhong(PHONGCHIEU phong)
        {
            if (ModelState.IsValid)
            {
                var p = data.PHONGCHIEUx.FirstOrDefault(x => x.MA_PHONG == phong.MA_PHONG);
                if (p != null)
                {
                    p.TEN_PHONG = phong.TEN_PHONG;
                    p.MA_RAP = phong.MA_RAP;
                    p.TONGSOGHE = phong.TONGSOGHE;
                    data.SaveChanges();
                    return RedirectToAction("QuanLyPhong");
                }
            }
            ViewBag.MA_RAP = new SelectList(data.RAPCHIEUx.ToList(), "MA_RAP", "TEN_RAP", phong.MA_RAP);
            return View(phong);
        }

        [HttpPost]
        public ActionResult XoaPhong(string id)
        {
            var p = data.PHONGCHIEUx.FirstOrDefault(x => x.MA_PHONG == id);
            if (p != null)
            {
                if (data.LICHCHIEUx.Any(l => l.MA_PHONG == id))
                {
                    TempData["Error"] = "Không thể xóa phòng này vì đang có lịch chiếu diễn ra tại đây.";
                    return RedirectToAction("QuanLyPhong");
                }

                var ghes = data.GHEs.Where(g => g.MA_PHONG == id).ToList();
                data.GHEs.RemoveRange(ghes);

                data.PHONGCHIEUx.Remove(p);
                data.SaveChanges();
            }
            return RedirectToAction("QuanLyPhong");
        }
        // GET: Admin/QuanLyLichChieu
        public ActionResult QuanLyLichChieu(string maPhim)
        {
            ViewBag.Title = "Quản lý Lịch chiếu";
            var lichchieus = data.LICHCHIEUx.Include("PHIM").Include("PHONGCHIEU").Include("KHUNGGIO").AsQueryable();
            
            if (!string.IsNullOrEmpty(maPhim))
            {
                lichchieus = lichchieus.Where(l => l.MA_PHIM == maPhim);
            }
            
            ViewBag.Phims = new SelectList(data.PHIMs.ToList(), "MA_PHIM", "TEN_PHIM", maPhim);
            return View(lichchieus.OrderByDescending(l => l.NGAYCHIEU).ToList());
        }

        public ActionResult ThemLichChieu()
        {
            ViewBag.Title = "Thêm Lịch chiếu";
            ViewBag.MA_PHIM = new SelectList(data.PHIMs.ToList(), "MA_PHIM", "TEN_PHIM");
            ViewBag.MA_PHONG = new SelectList(data.PHONGCHIEUx.ToList(), "MA_PHONG", "TEN_PHONG");
            ViewBag.MA_KHUNGGIO = new SelectList(data.KHUNGGIOs.ToList(), "MA_KHUNGGIO", "GIOBATDAU");
            return View();
        }

        [HttpPost]
        public ActionResult ThemLichChieu(LICHCHIEU lichChieu)
        {
            if (ModelState.IsValid)
            {
                // Validate duplicate
                bool isDuplicate = data.LICHCHIEUx.Any(l => 
                    l.MA_PHONG == lichChieu.MA_PHONG && 
                    l.MA_KHUNGGIO == lichChieu.MA_KHUNGGIO && 
                    l.NGAYCHIEU == lichChieu.NGAYCHIEU);
                
                if (isDuplicate)
                {
                    TempData["Error"] = "Lịch chiếu này bị trùng lặp (Cùng phòng, cùng ngày, cùng khung giờ). Vui lòng chọn lại.";
                }
                else
                {
                    string currentMax = data.LICHCHIEUx.Max(l => l.MA_LICHCHIEU);
                    int nextId = 1;
                    if (!string.IsNullOrEmpty(currentMax) && currentMax.StartsWith("LC"))
                    {
                        if (int.TryParse(currentMax.Substring(2), out int currId))
                        {
                            nextId = currId + 1;
                        }
                    }
                    lichChieu.MA_LICHCHIEU = "LC" + nextId.ToString("D3");
                    
                    data.LICHCHIEUx.Add(lichChieu);
                    data.SaveChanges();
                    return RedirectToAction("QuanLyLichChieu");
                }
            }
            ViewBag.MA_PHIM = new SelectList(data.PHIMs.ToList(), "MA_PHIM", "TEN_PHIM", lichChieu.MA_PHIM);
            ViewBag.MA_PHONG = new SelectList(data.PHONGCHIEUx.ToList(), "MA_PHONG", "TEN_PHONG", lichChieu.MA_PHONG);
            ViewBag.MA_KHUNGGIO = new SelectList(data.KHUNGGIOs.ToList(), "MA_KHUNGGIO", "GIOBATDAU", lichChieu.MA_KHUNGGIO);
            return View(lichChieu);
        }

        public ActionResult SuaLichChieu(string id)
        {
            ViewBag.Title = "Sửa Lịch chiếu";
            var lichChieu = data.LICHCHIEUx.FirstOrDefault(l => l.MA_LICHCHIEU == id);
            if (lichChieu == null) return HttpNotFound();

            ViewBag.MA_PHIM = new SelectList(data.PHIMs.ToList(), "MA_PHIM", "TEN_PHIM", lichChieu.MA_PHIM);
            ViewBag.MA_PHONG = new SelectList(data.PHONGCHIEUx.ToList(), "MA_PHONG", "TEN_PHONG", lichChieu.MA_PHONG);
            ViewBag.MA_KHUNGGIO = new SelectList(data.KHUNGGIOs.ToList(), "MA_KHUNGGIO", "GIOBATDAU", lichChieu.MA_KHUNGGIO);
            return View(lichChieu);
        }

        [HttpPost]
        public ActionResult SuaLichChieu(LICHCHIEU lichChieu)
        {
            if (ModelState.IsValid)
            {
                bool isDuplicate = data.LICHCHIEUx.Any(l => 
                    l.MA_LICHCHIEU != lichChieu.MA_LICHCHIEU &&
                    l.MA_PHONG == lichChieu.MA_PHONG && 
                    l.MA_KHUNGGIO == lichChieu.MA_KHUNGGIO && 
                    l.NGAYCHIEU == lichChieu.NGAYCHIEU);
                
                if (isDuplicate)
                {
                    TempData["Error"] = "Lịch chiếu cập nhật bị trùng lặp với một suất khác. Vui lòng chọn lại.";
                }
                else
                {
                    var l = data.LICHCHIEUx.FirstOrDefault(x => x.MA_LICHCHIEU == lichChieu.MA_LICHCHIEU);
                    if (l != null)
                    {
                        l.MA_PHIM = lichChieu.MA_PHIM;
                        l.MA_PHONG = lichChieu.MA_PHONG;
                        l.MA_KHUNGGIO = lichChieu.MA_KHUNGGIO;
                        l.NGAYCHIEU = lichChieu.NGAYCHIEU;
                        l.GIATVE_PHUTROI = lichChieu.GIATVE_PHUTROI;
                        data.SaveChanges();
                        return RedirectToAction("QuanLyLichChieu");
                    }
                }
            }
            ViewBag.MA_PHIM = new SelectList(data.PHIMs.ToList(), "MA_PHIM", "TEN_PHIM", lichChieu.MA_PHIM);
            ViewBag.MA_PHONG = new SelectList(data.PHONGCHIEUx.ToList(), "MA_PHONG", "TEN_PHONG", lichChieu.MA_PHONG);
            ViewBag.MA_KHUNGGIO = new SelectList(data.KHUNGGIOs.ToList(), "MA_KHUNGGIO", "GIOBATDAU", lichChieu.MA_KHUNGGIO);
            return View(lichChieu);
        }

        [HttpPost]
        public ActionResult XoaLichChieu(string id)
        {
            var l = data.LICHCHIEUx.FirstOrDefault(x => x.MA_LICHCHIEU == id);
            if (l != null)
            {
                if (data.VEs.Any(v => v.MA_LICHCHIEU == id))
                {
                    TempData["Error"] = "Không thể xóa lịch chiếu này vì đã có khách hàng đặt vé.";
                    return RedirectToAction("QuanLyLichChieu");
                }

                data.LICHCHIEUx.Remove(l);
                data.SaveChanges();
            }
            return RedirectToAction("QuanLyLichChieu");
        }
        // GET: Admin/QuanLyVe
        public ActionResult QuanLyVe(string searchSdt, string searchMaLich)
        {
            ViewBag.Title = "Quản lý Vé";
            var ves = data.VEs.Include("KHACHHANG").Include("LICHCHIEU").Include("GHE").AsQueryable();

            if (!string.IsNullOrEmpty(searchSdt))
            {
                ves = ves.Where(v => v.KHACHHANG.SDT.Contains(searchSdt));
            }
            if (!string.IsNullOrEmpty(searchMaLich))
            {
                ves = ves.Where(v => v.MA_LICHCHIEU.Contains(searchMaLich));
            }

            ViewBag.searchSdt = searchSdt;
            ViewBag.searchMaLich = searchMaLich;

            return View(ves.OrderByDescending(v => v.NGAYDAT).ToList());
        }

        [HttpPost]
        public ActionResult CapNhatTrangThaiVe(string id, string status)
        {
            var ve = data.VEs.FirstOrDefault(v => v.MA_VE == id);
            if (ve != null)
            {
                ve.TRANGTHAI = status;
                data.SaveChanges();
            }
            return RedirectToAction("QuanLyVe");
        }

        [HttpPost]
        public ActionResult XoaVe(string id)
        {
            var ve = data.VEs.FirstOrDefault(v => v.MA_VE == id);
            if (ve != null)
            {
                data.VEs.Remove(ve);
                data.SaveChanges();
            }
            return RedirectToAction("QuanLyVe");
        }
        // GET: Admin/QuanLyKhachHang
        public ActionResult QuanLyKhachHang()
        {
            ViewBag.Title = "Quản lý Khách hàng";
            var khachHangs = data.KHACHHANGs.OrderByDescending(k => k.NGAYTAO).ToList();
            return View(khachHangs);
        }

        public ActionResult ChiTietKhachHang(string id)
        {
            ViewBag.Title = "Chi tiết Khách hàng";
            var kh = data.KHACHHANGs.FirstOrDefault(k => k.MA_KHACH == id);
            if (kh == null) return HttpNotFound();

            ViewBag.LichSuVe = data.VEs.Where(v => v.MA_KHACH == id).OrderByDescending(v => v.NGAYDAT).ToList();
            return View(kh);
        }

        [HttpPost]
        public ActionResult SuaQuyenKhachHang(string id, string vaiTro)
        {
            var kh = data.KHACHHANGs.FirstOrDefault(k => k.MA_KHACH == id);
            if (kh != null)
            {
                kh.VAITRO = vaiTro;
                data.SaveChanges();
            }
            return RedirectToAction("QuanLyKhachHang");
        }

        [HttpPost]
        public ActionResult XoaKhachHang(string id)
        {
            var kh = data.KHACHHANGs.FirstOrDefault(k => k.MA_KHACH == id);
            if (kh != null)
            {
                if (data.VEs.Any(v => v.MA_KHACH == id))
                {
                    TempData["Error"] = "Không thể xóa tài khoản này vì khách hàng đã có lịch sử đặt vé.";
                    return RedirectToAction("QuanLyKhachHang");
                }

                data.KHACHHANGs.Remove(kh);
                data.SaveChanges();
            }
            return RedirectToAction("QuanLyKhachHang");
        }
    }
}