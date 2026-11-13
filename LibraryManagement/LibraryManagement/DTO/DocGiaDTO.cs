namespace LibraryManagement.DTO
{
    public class DocGiaDTO
    {
        public string MaDocGia { get; set; } = "";
        public string HoTen { get; set; } = "";
        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; } = "";
        public string DiaChi { get; set; } = "";
        public string SoDienThoai { get; set; } = "";
        public DateTime NgayHetHanThe { get; set; }
    }
}