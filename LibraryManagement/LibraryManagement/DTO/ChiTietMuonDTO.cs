using System;

namespace LibraryManagement.DTO
{
    public class ChiTietMuonDTO
    {
        public int MaChiTiet { get; set; }
        public string MaPhieu { get; set; } = "";
        public string MaSach { get; set; } = "";
        public int SoLuong { get; set; }
        public DateTime? NgayTra { get; set; }
    }
}