namespace ClinicManagement.DTO
{
    public class DonThuoc
    {
        public int MaDon { get; set; }
        public int MaHoSo { get; set; }
        public decimal TienKham { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; } = "Chưa thanh toán";
    }
}