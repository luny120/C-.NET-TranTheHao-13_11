using System;
using System.Collections.Generic;
using LibraryManagement.DTO;
using LibraryManagement.DAL;

namespace LibraryManagement.BUS
{
    public class DauSachBUS
    {
        private readonly DauSachDAL _dal = new DauSachDAL();

        public List<DauSachDTO> GetAll() => _dal.GetAll();
        public DauSachDTO? GetById(string ma) => _dal.GetById(ma);

        public void Insert(DauSachDTO s)
        {
            if (string.IsNullOrWhiteSpace(s.MaSach))
                throw new Exception("Mã sách không được để trống!");
            if (string.IsNullOrWhiteSpace(s.TenSach))
                throw new Exception("Tên sách không được để trống!");
            if (s.SoLuongTong < 0 || s.SoLuongCon < 0 || s.SoLuongCon > s.SoLuongTong)
                throw new Exception("Số lượng không hợp lệ!");
            if (_dal.GetById(s.MaSach) != null)
                throw new Exception("Mã sách đã tồn tại!");
            if (!_dal.Insert(s)) throw new Exception("Thêm sách thất bại!");
        }

        public void Update(DauSachDTO s)
        {
            if (_dal.GetById(s.MaSach) == null)
                throw new Exception("Đầu sách không tồn tại!");
            if (s.SoLuongCon > s.SoLuongTong)
                throw new Exception("Số lượng còn không được lớn hơn tổng!");
            if (!_dal.Update(s)) throw new Exception("Cập nhật thất bại!");
        }

        public void Delete(string ma)
        {
            if (!_dal.Delete(ma)) throw new Exception("Xóa thất bại!");
        }
    }
}