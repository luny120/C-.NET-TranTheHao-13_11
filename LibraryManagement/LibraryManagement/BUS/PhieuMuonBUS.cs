using System;
using System.Collections.Generic;
using System.Data;
using LibraryManagement.DTO;
using LibraryManagement.DAL;

namespace LibraryManagement.BUS
{
    public class PhieuMuonBUS
    {
        private readonly PhieuMuonDAL _dal = new PhieuMuonDAL();
        private readonly DocGiaBUS _docGiaBUS;

        public PhieuMuonBUS()
        {
            _docGiaBUS = new DocGiaBUS();
        }

        public List<PhieuMuonDTO> GetAll() => _dal.GetAll();

        /// <summary>
        /// Tạo phiếu mượn: check hạn thẻ → check sách → gọi DAL.
        /// </summary>
        public void LapPhieuMuon(PhieuMuonDTO pm, string maSach, int soLuong)
        {
            // 1) Kiểm tra nghiệp vụ: thẻ phải còn hạn
            if (!_docGiaBUS.KiemTraHanThe(pm.MaDocGia))
                throw new Exception("Thẻ độc giả đã hết hạn, không thể mượn sách!");

            // 2) Validate cơ bản
            if (string.IsNullOrWhiteSpace(pm.MaPhieu))
                throw new Exception("Mã phiếu không hợp lệ!");
            if (soLuong <= 0)
                throw new Exception("Số lượng mượn phải > 0!");
            if (pm.HanTra.Date < pm.NgayMuon.Date)
                throw new Exception("Hạn trả phải >= ngày mượn!");

            // 3) Gọi DAL (đã có transaction, tự check SoLuongCon)
            _dal.TaoPhieuMuon(pm, maSach, soLuong);
        }

        public void TraSach(string maPhieu) => _dal.TraSach(maPhieu);

        public List<PhieuMuonDTO> GetQuaHan()
        {
            _dal.CapNhatTrangThaiQuaHan();
            return _dal.GetQuaHan();
        }

        public DataTable ThongKeTheoTheLoai() => _dal.ThongKeTheoTheLoai();
        public int DemLuotMuonTheoTheLoai(string tl) => _dal.DemLuotMuonTheoTheLoai(tl);
    }
}