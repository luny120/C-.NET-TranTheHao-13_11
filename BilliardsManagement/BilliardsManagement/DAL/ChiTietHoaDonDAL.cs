using Microsoft.Data.SqlClient;
using System.Data;
using DTO;
using System.Collections.Generic;

namespace DAL
{
    public class ChiTietHoaDonDAL
    {
        /// <summary>
        /// DISCONNECTED MODEL: Load danh sách dịch vụ đã gọi về DataTable.
        /// Dùng SqlDataAdapter.Fill() — kết nối được đóng ngay sau khi Fill.
        /// </summary>
        public DataTable GetChiTietDataTable(int maHD)
        {
            var dt = new DataTable();
            const string sql = @"
                SELECT ct.MaDV, dv.TenDV, ct.SoLuong, ct.DonGia,
                       CAST(ct.SoLuong * ct.DonGia AS DECIMAL(18,2)) AS ThanhTien
                FROM ChiTietHoaDon ct
                JOIN MenuDichVu dv ON ct.MaDV = dv.MaDV
                WHERE ct.MaHD = @MaHD
                ORDER BY dv.TenDV";

            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@MaHD", maHD);
            da.Fill(dt);   // <-- Disconnected: Fill xong không giữ kết nối
            return dt;
        }

        /// <summary>Lấy list chi tiết để BUS tính tổng tiền dịch vụ.</summary>
        public List<ChiTietHoaDonDTO> GetByMaHD(int maHD)
        {
            var list = new List<ChiTietHoaDonDTO>();
            const string sql = @"
                SELECT ct.MaHD, ct.MaDV, dv.TenDV, ct.SoLuong, ct.DonGia
                FROM ChiTietHoaDon ct
                JOIN MenuDichVu dv ON ct.MaDV = dv.MaDV
                WHERE ct.MaHD = @MaHD";
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaHD", maHD);
            conn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new ChiTietHoaDonDTO
                {
                    MaHD = (int)rd["MaHD"],
                    MaDV = (int)rd["MaDV"],
                    TenDV = rd["TenDV"].ToString()!,
                    SoLuong = (int)rd["SoLuong"],
                    DonGia = (decimal)rd["DonGia"]
                });
            }
            return list;
        }

        /// <summary>Thêm mới hoặc cộng dồn số lượng nếu món đã tồn tại.</summary>
        public void AddOrUpdate(int maHD, int maDV, int soLuong, decimal donGia)
        {
            const string sql = @"
                IF EXISTS (SELECT 1 FROM ChiTietHoaDon WHERE MaHD=@MaHD AND MaDV=@MaDV)
                    UPDATE ChiTietHoaDon
                    SET SoLuong = SoLuong + @SoLuong
                    WHERE MaHD=@MaHD AND MaDV=@MaDV;
                ELSE
                    INSERT INTO ChiTietHoaDon(MaHD, MaDV, SoLuong, DonGia)
                    VALUES(@MaHD, @MaDV, @SoLuong, @DonGia);";
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaHD", maHD);
            cmd.Parameters.AddWithValue("@MaDV", maDV);
            cmd.Parameters.AddWithValue("@SoLuong", soLuong);
            cmd.Parameters.AddWithValue("@DonGia", donGia);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        /// <summary>Xóa 1 dòng dịch vụ khỏi hóa đơn.</summary>
        public void Delete(int maHD, int maDV)
        {
            const string sql = "DELETE FROM ChiTietHoaDon WHERE MaHD=@MaHD AND MaDV=@MaDV";
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaHD", maHD);
            cmd.Parameters.AddWithValue("@MaDV", maDV);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}