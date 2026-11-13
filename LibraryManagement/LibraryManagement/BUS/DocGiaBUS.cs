using System;
using System.Collections.Generic;
using LibraryManagement.DTO;
using LibraryManagement.DAL;

namespace LibraryManagement.BUS
{
    public class DocGiaBUS
    {
        private readonly DocGiaDAL _dal = new DocGiaDAL();

        public List<DocGiaDTO> GetAll() => _dal.GetAll();
        public DocGiaDTO? GetById(string ma) => _dal.GetById(ma);

        public void Insert(DocGiaDTO dg)
        {
            if (string.IsNullOrWhiteSpace(dg.MaDocGia))
                throw new Exception("Mã độc giả không được để trống!");
            if (string.IsNullOrWhiteSpace(dg.HoTen))
                throw new Exception("Họ tên không được để trống!");
            if (_dal.GetById(dg.MaDocGia) != null)
                throw new Exception("Mã độc giả đã tồn tại!");
            if (!_dal.Insert(dg))
                throw new Exception("Thêm độc giả thất bại!");
        }

        public void Update(DocGiaDTO dg)
        {
            if (_dal.GetById(dg.MaDocGia) == null)
                throw new Exception("Độc giả không tồn tại!");
            if (!_dal.Update(dg))
                throw new Exception("Cập nhật thất bại!");
        }

        public void Delete(string ma)
        {
            if (!_dal.Delete(ma))
                throw new Exception("Xóa thất bại!");
        }

        /// <summary>Kiểm tra thẻ còn hạn hay không (NgayHetHanThe >= GETDATE()).</summary>
        public bool KiemTraHanThe(string maDocGia)
        {
            var dg = _dal.GetById(maDocGia)
                     ?? throw new Exception("Độc giả không tồn tại!");
            return dg.NgayHetHanThe.Date >= DateTime.Now.Date;
        }
    }
}