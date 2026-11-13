using System;

namespace LibraryManagement.DTO
{
    public class PhieuMuonDTO
    {
        public string MaPhieu { get; set; } = "";
        public string MaDocGia { get; set; } = "";
        public string HoTenDocGia { get; set; } = "";   // field hiển thị
        public DateTime NgayMuon { get; set; }
        public DateTime HanTra { get; set; }
        public string TrangThai { get; set; } = "";
        public int SoNgayQuaHan { get; set; }         // field hiển thị
    }
}