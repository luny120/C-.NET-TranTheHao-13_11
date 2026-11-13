namespace DTO
{
    public class ChiTietHoaDonDTO
    {
        public int MaHD { get; set; }
        public int MaDV { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }

        // Field JOIN thêm để hiển thị
        public string TenDV { get; set; } = "";

        public decimal ThanhTien => SoLuong * DonGia;
    }
}