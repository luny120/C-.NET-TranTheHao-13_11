namespace LibraryManagement.DTO
{
    public class DauSachDTO
    {
        public string MaSach { get; set; } = "";
        public string TenSach { get; set; } = "";
        public string TacGia { get; set; } = "";
        public string NhaXuatBan { get; set; } = "";
        public int NamXuatBan { get; set; }
        public string TheLoai { get; set; } = "";
        public int SoLuongTong { get; set; }
        public int SoLuongCon { get; set; }
    }
}