using ClinicManagement.DAL;
using ClinicManagement.DTO;

namespace ClinicManagement.BUS
{
    public class DonThuocBUS
    {
        private readonly DonThuocDAL dal = new DonThuocDAL();

        public int LuuDonThuoc(DonThuoc don, List<ChiTietDonThuoc> chiTiet)
        {
            if (chiTiet == null || chiTiet.Count == 0)
                throw new Exception("Đơn thuốc phải có ít nhất một loại thuốc.");
            if (don.TienKham < 0)
                throw new Exception("Tiền khám phải >= 0.");
            if (don.TongTien < 0)
                throw new Exception("Tổng tiền không hợp lệ.");

            return dal.LuuDonThuoc(don, chiTiet);
        }

        public void ThanhToan(int maDon)
        {
            if (maDon <= 0) throw new Exception("Mã đơn không hợp lệ.");
            dal.ThanhToan(maDon);
        }
    }
}