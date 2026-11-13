using DAL;
using System.Data;

namespace BUS
{
    public class ChiTietHoaDonBUS
    {
        private readonly ChiTietHoaDonDAL _dal = new ChiTietHoaDonDAL();
        public DataTable GetChiTietDataTable(int maHD) => _dal.GetChiTietDataTable(maHD);
        public void AddOrUpdate(int maHD, int maDV, int soLuong, decimal donGia)
            => _dal.AddOrUpdate(maHD, maDV, soLuong, donGia);
        public void Delete(int maHD, int maDV) => _dal.Delete(maHD, maDV);
    }
}