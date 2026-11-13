using ClinicManagement.DAL;
using ClinicManagement.DTO;

namespace ClinicManagement.BUS
{
    public class ThuocBUS
    {
        private readonly ThuocDAL dal = new ThuocDAL();

        public List<Thuoc> LayTatCa() => dal.LayTatCa();

        public void Them(Thuoc t) { Validate(t); dal.Them(t); }
        public void Sua(Thuoc t) { Validate(t); dal.Sua(t); }
        public void Xoa(int maThuoc) => dal.Xoa(maThuoc);

        public void NhapTon(int maThuoc, int soLuongThem)
        {
            if (soLuongThem <= 0)
                throw new Exception("Số lượng nhập thêm phải > 0.");
            dal.NhapTon(maThuoc, soLuongThem);
        }

        private static void Validate(Thuoc t)
        {
            if (string.IsNullOrWhiteSpace(t.TenThuoc))
                throw new Exception("Tên thuốc không được rỗng.");
            if (string.IsNullOrWhiteSpace(t.DonViTinh))
                throw new Exception("Đơn vị tính không được rỗng.");
            if (t.DonGia < 0)
                throw new Exception("Đơn giá phải >= 0.");
            if (t.SoLuongTon < 0)
                throw new Exception("Số lượng tồn phải >= 0.");
        }
    }
}