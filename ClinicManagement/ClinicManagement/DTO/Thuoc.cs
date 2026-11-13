namespace ClinicManagement.DTO
{
    public class Thuoc
    {
        public int MaThuoc { get; set; }
        public string TenThuoc { get; set; } = string.Empty;
        public string DonViTinh { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
        public int SoLuongTon { get; set; }
    }
}