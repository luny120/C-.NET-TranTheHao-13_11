using DTO;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BUS
{
    public class HoaDonBUS
    {
        private readonly HoaDonDAL _hdDal = new HoaDonDAL();
        private readonly ChiTietHoaDonDAL _ctDal = new ChiTietHoaDonDAL();
        private readonly BanBidaDAL _banDal = new BanBidaDAL();

        public HoaDonDTO? GetActiveByMaBan(int maBan) => _hdDal.GetActiveByMaBan(maBan);

        /// <summary>
        /// Mở bàn: tạo hóa đơn + đổi trạng thái bàn. Trả về MaHD mới.
        /// </summary>
        public int MoBan(int maBan)
        {
            var ban = _banDal.GetByMaBan(maBan)
                      ?? throw new Exception("Bàn không tồn tại!");
            if (ban.TrangThai == "Đang chơi")
                throw new Exception("Bàn đang chơi, không thể mở!");

            if (_hdDal.GetActiveByMaBan(maBan) != null)
                throw new Exception("Đã có hóa đơn mở cho bàn này!");

            return _hdDal.MoBanTransaction(maBan, DateTime.Now);
        }

        /// <summary>
        /// Tính tiền giờ chơi.
        /// Quy tắc: làm tròn LÊN theo block 15 phút (tối thiểu 1 block = 15 phút).
        /// Ví dụ: 5 phút -> 15 phút; 16 phút -> 30 phút; 1h05 -> 1h15.
        /// </summary>
        public decimal TinhTienGio(DateTime start, DateTime end, decimal giaTheoGio)
        {
            if (end < start) end = start;
            double totalMinutes = (end - start).TotalMinutes;

            // Số block 15 phút, làm tròn LÊN
            int blocks = (int)Math.Ceiling(totalMinutes / 15.0);
            if (blocks == 0) blocks = 1;      // chơi dưới 15' vẫn tính 1 block

            double hours = blocks * 15.0 / 60.0;   // đổi block -> giờ
            return Math.Round((decimal)hours * giaTheoGio, 0);
        }

        /// <summary>
        /// Thanh toán 1 hóa đơn. Trả về true nếu thành công.
        /// </summary>
        public bool ThanhToan(int maHD)
        {
            var hd = _hdDal.GetActiveByMaBan(0); // placeholder, ta cần lấy theo MaHD
            // Lấy trực tiếp theo MaHD thông qua bàn:
            // Cách đơn giản: dùng lại GetActiveByMaBan của ban chứa hd này.
            // Ở đây ta cho phép tìm theo MaBan.
            // -> Cần biết MaBan: nếu GUI đã có sẵn thì truyền vào.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Thanh toán đầy đủ — gọi từ GUI khi đã biết MaHD & MaBan.
        /// Trả về (thành công, tổng tiền).
        /// </summary>
        public (bool ok, decimal tongTien) ThanhToan(int maHD, int maBan)
        {
            var hd = _hdDal.GetActiveByMaBan(maBan);
            if (hd == null || hd.MaHD != maHD)
                return (false, 0);

            var ban = _banDal.GetByMaBan(maBan);
            if (ban == null) return (false, 0);

            // 1) Tính tiền giờ
            DateTime end = DateTime.Now;
            hd.GioKetThuc = end;
            hd.TienGioChoi = TinhTienGio(hd.GioBatDau, end, ban.GiaTheoGio);

            // 2) Tính tiền dịch vụ
            var chiTiet = _ctDal.GetByMaHD(maHD);
            hd.TienDichVu = chiTiet.Sum(x => x.ThanhTien);

            // 3) Gọi DAL transaction
            bool ok = _hdDal.ThanhToanTransaction(hd, chiTiet);
            decimal tong = hd.TienGioChoi + hd.TienDichVu;
            return (ok, tong);
        }

        /// <summary>
        /// Dùng để hiển thị "tạm tính" realtime lên GUI.
        /// </summary>
        public (decimal tienGio, decimal tienDV, decimal tong) TinhTamTinh(int maHD, int maBan)
        {
            var ban = _banDal.GetByMaBan(maBan);
            var hd = _hdDal.GetActiveByMaBan(maBan);
            if (ban == null || hd == null) return (0, 0, 0);

            decimal tienGio = TinhTienGio(hd.GioBatDau, DateTime.Now, ban.GiaTheoGio);
            var ct = _ctDal.GetByMaHD(maHD);
            decimal tienDV = ct.Sum(x => x.ThanhTien);
            return (tienGio, tienDV, tienGio + tienDV);
        }
    }
}