using System;

namespace DTO
{
    public class HoaDonDTO
    {
        public int MaHD { get; set; }
        public int MaBan { get; set; }
        public DateTime GioBatDau { get; set; }
        public DateTime? GioKetThuc { get; set; }
        public decimal TienGioChoi { get; set; }
        public decimal TienDichVu { get; set; }
        public int TrangThai { get; set; }   // 0: Chưa TT, 1: Đã TT
    }
}