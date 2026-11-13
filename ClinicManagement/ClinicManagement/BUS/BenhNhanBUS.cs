using ClinicManagement.DAL;
using ClinicManagement.DTO;

namespace ClinicManagement.BUS
{
    public class BenhNhanBUS
    {
        private readonly BenhNhanDAL dal = new BenhNhanDAL();
        private readonly HoSoKhamDAL hsDal = new HoSoKhamDAL();

        public BenhNhan? TimTheoSDT(string sdt) => dal.TimTheoSDT(sdt);
        public List<BenhNhan> LayTatCa() => dal.LayTatCa();

        /// <summary>
        /// Tra cứu theo SĐT; nếu chưa có -> tạo mới BN.
        /// Sau đó luôn tạo một HoSoKham mới cho lần khám hiện tại.
        /// </summary>
        public (BenhNhan bn, int maHoSo) TiepNhan(
            string sdt, string hoTen, int namSinh, string? trieuChung, string? chanDoan)
        {
            if (string.IsNullOrWhiteSpace(sdt))
                throw new Exception("Số điện thoại không được rỗng.");

            BenhNhan? bn = dal.TimTheoSDT(sdt);
            if (bn == null)
            {
                if (string.IsNullOrWhiteSpace(hoTen))
                    throw new Exception("Bệnh nhân mới - họ tên không được rỗng.");
                if (namSinh < 1900 || namSinh > DateTime.Now.Year)
                    throw new Exception("Năm sinh không hợp lệ.");

                bn = new BenhNhan { HoTen = hoTen, SoDienThoai = sdt, NamSinh = namSinh };
                bn.MaBN = dal.Them(bn);
            }

            var hs = new HoSoKham { MaBN = bn.MaBN, TrieuChung = trieuChung, ChanDoan = chanDoan };
            int maHoSo = hsDal.Them(hs);
            return (bn, maHoSo);
        }
    }
}