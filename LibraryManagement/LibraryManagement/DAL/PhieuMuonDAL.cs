using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using LibraryManagement.DTO;
using LibraryManagement.Utils;

namespace LibraryManagement.DAL
{
    public class PhieuMuonDAL
    {
        // --------------------------------------------
        // 1) LẤY TOÀN BỘ PHIẾU MƯỢN (join để hiện tên độc giả)
        // --------------------------------------------
        public List<PhieuMuonDTO> GetAll()
        {
            var list = new List<PhieuMuonDTO>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            const string sql = @"
                SELECT p.MaPhieu, p.MaDocGia, d.HoTen, p.NgayMuon, p.HanTra, p.TrangThai
                FROM   PhieuMuon p
                JOIN   DocGia   d ON p.MaDocGia = d.MaDocGia
                ORDER BY p.NgayMuon DESC";
            using var rd = new SqlCommand(sql, conn).ExecuteReader();
            while (rd.Read())
            {
                list.Add(new PhieuMuonDTO
                {
                    MaPhieu = rd["MaPhieu"].ToString(),
                    MaDocGia = rd["MaDocGia"].ToString(),
                    HoTenDocGia = rd["HoTen"].ToString(),
                    NgayMuon = Convert.ToDateTime(rd["NgayMuon"]),
                    HanTra = Convert.ToDateTime(rd["HanTra"]),
                    TrangThai = rd["TrangThai"].ToString()
                });
            }
            return list;
        }

        // --------------------------------------------
        // 2) TẠO PHIẾU MƯỢN  (transaction: insert PhieuMuon + ChiTietMuon + trừ SoLuongCon)
        // --------------------------------------------
        public bool TaoPhieuMuon(PhieuMuonDTO pm, string maSach, int soLuong)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                // (a) kiểm tra còn sách không
                var check = new SqlCommand(
                    "SELECT SoLuongCon FROM DauSach WHERE MaSach=@Ma", conn, tran);
                check.Parameters.AddWithValue("@Ma", maSach);
                object? o = check.ExecuteScalar();
                if (o == null || o == DBNull.Value)
                    throw new Exception("Đầu sách không tồn tại!");
                int con = Convert.ToInt32(o);
                if (con < soLuong)
                    throw new Exception($"Không đủ sách để mượn. Còn lại: {con}");

                // (b) insert PhieuMuon
                var cmd1 = new SqlCommand(
                    "INSERT INTO PhieuMuon(MaPhieu, MaDocGia, NgayMuon, HanTra, TrangThai) " +
                    "VALUES(@Ma, @DG, @NM, @HT, N'Đang mượn')", conn, tran);
                cmd1.Parameters.AddWithValue("@Ma", pm.MaPhieu);
                cmd1.Parameters.AddWithValue("@DG", pm.MaDocGia);
                cmd1.Parameters.AddWithValue("@NM", pm.NgayMuon.Date);
                cmd1.Parameters.AddWithValue("@HT", pm.HanTra.Date);
                cmd1.ExecuteNonQuery();

                // (c) insert ChiTietMuon
                var cmd2 = new SqlCommand(
                    "INSERT INTO ChiTietMuon(MaPhieu, MaSach, SoLuong) VALUES(@Ma, @MS, @SL)",
                    conn, tran);
                cmd2.Parameters.AddWithValue("@Ma", pm.MaPhieu);
                cmd2.Parameters.AddWithValue("@MS", maSach);
                cmd2.Parameters.AddWithValue("@SL", soLuong);
                cmd2.ExecuteNonQuery();

                // (d) trừ SoLuongCon của DauSach
                var cmd3 = new SqlCommand(
                    "UPDATE DauSach SET SoLuongCon = SoLuongCon - @SL WHERE MaSach=@MS",
                    conn, tran);
                cmd3.Parameters.AddWithValue("@SL", soLuong);
                cmd3.Parameters.AddWithValue("@MS", maSach);
                cmd3.ExecuteNonQuery();

                tran.Commit();
                return true;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        // --------------------------------------------
        // 3) TRẢ SÁCH  (transaction: cộng lại SoLuongCon + set NgayTra + set TrangThai = N'Đã trả')
        // --------------------------------------------
        public bool TraSach(string maPhieu)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                // Lấy danh sách chi tiết để biết mỗi sách cần cộng lại bao nhiêu
                var getDetail = new SqlCommand(
                    "SELECT MaSach, SoLuong FROM ChiTietMuon WHERE MaPhieu=@Ma", conn, tran);
                getDetail.Parameters.AddWithValue("@Ma", maPhieu);

                var details = new List<(string maSach, int sl)>();
                using (var rd = getDetail.ExecuteReader())
                {
                    while (rd.Read())
                        details.Add((rd["MaSach"].ToString()!, Convert.ToInt32(rd["SoLuong"])));
                }
                if (details.Count == 0)
                    throw new Exception("Không có chi tiết mượn cho phiếu này!");

                // (a) Cộng lại từng SoLuongCon
                foreach (var (maSach, sl) in details)
                {
                    var up = new SqlCommand(
                        "UPDATE DauSach SET SoLuongCon = SoLuongCon + @SL WHERE MaSach=@MS",
                        conn, tran);
                    up.Parameters.AddWithValue("@SL", sl);
                    up.Parameters.AddWithValue("@MS", maSach);
                    up.ExecuteNonQuery();
                }

                // (b) Set NgayTra cho chi tiết
                var upDetail = new SqlCommand(
                    "UPDATE ChiTietMuon SET NgayTra=@NT WHERE MaPhieu=@Ma", conn, tran);
                upDetail.Parameters.AddWithValue("@NT", DateTime.Now.Date);
                upDetail.Parameters.AddWithValue("@Ma", maPhieu);
                upDetail.ExecuteNonQuery();

                // (c) Cập nhật trạng thái phiếu
                var upPhieu = new SqlCommand(
                    "UPDATE PhieuMuon SET TrangThai = N'Đã trả' WHERE MaPhieu=@Ma", conn, tran);
                upPhieu.Parameters.AddWithValue("@Ma", maPhieu);
                upPhieu.ExecuteNonQuery();

                tran.Commit();
                return true;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        // --------------------------------------------
        // 4) CẢNH BÁO QUÁ HẠN  (dùng SqlDataReader)
        // --------------------------------------------
        public List<PhieuMuonDTO> GetQuaHan()
        {
            var list = new List<PhieuMuonDTO>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            const string sql = @"
                SELECT p.MaPhieu, p.MaDocGia, d.HoTen, p.NgayMuon, p.HanTra, p.TrangThai,
                       DATEDIFF(DAY, p.HanTra, GETDATE()) AS SoNgayQuaHan
                FROM   PhieuMuon p
                JOIN   DocGia   d ON p.MaDocGia = d.MaDocGia
                WHERE  p.HanTra < GETDATE() AND p.TrangThai <> N'Đã trả'
                ORDER BY SoNgayQuaHan DESC";
            using var rd = new SqlCommand(sql, conn).ExecuteReader();
            while (rd.Read())
            {
                list.Add(new PhieuMuonDTO
                {
                    MaPhieu = rd["MaPhieu"].ToString(),
                    MaDocGia = rd["MaDocGia"].ToString(),
                    HoTenDocGia = rd["HoTen"].ToString(),
                    NgayMuon = Convert.ToDateTime(rd["NgayMuon"]),
                    HanTra = Convert.ToDateTime(rd["HanTra"]),
                    TrangThai = rd["TrangThai"].ToString(),
                    SoNgayQuaHan = Convert.ToInt32(rd["SoNgayQuaHan"])
                });
            }
            return list;
        }

        // Cập nhật trạng thái "Quá hạn" cho các phiếu đang mượn mà đã quá HanTra
        public void CapNhatTrangThaiQuaHan()
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            var cmd = new SqlCommand(
                "UPDATE PhieuMuon SET TrangThai = N'Quá hạn' " +
                "WHERE HanTra < GETDATE() AND TrangThai = N'Đang mượn'", conn);
            cmd.ExecuteNonQuery();
        }

        // --------------------------------------------
        // 5) THỐNG KÊ SỐ LƯỢT MƯỢN THEO THỂ LOẠI  (GROUP BY)
        // --------------------------------------------
        public DataTable ThongKeTheoTheLoai()
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            const string sql = @"
                SELECT ds.TheLoai                              AS [Thể loại],
                       COUNT(DISTINCT ct.MaPhieu)              AS [Số lượt mượn],
                       SUM(ct.SoLuong)                         AS [Tổng số lượng]
                FROM   ChiTietMuon ct
                JOIN   DauSach     ds ON ct.MaSach = ds.MaSach
                GROUP BY ds.TheLoai
                ORDER BY [Số lượt mượn] DESC";
            var adapter = new SqlDataAdapter(sql, conn);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        // (Bonus) ExecuteScalar: tổng số lượt mượn của 1 thể loại cụ thể
        public int DemLuotMuonTheoTheLoai(string theLoai)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            var cmd = new SqlCommand(@"
                SELECT COUNT(ct.MaChiTiet)
                FROM   ChiTietMuon ct
                JOIN   DauSach ds ON ct.MaSach = ds.MaSach
                WHERE  ds.TheLoai = @TL", conn);
            cmd.Parameters.AddWithValue("@TL", theLoai);
            object? o = cmd.ExecuteScalar();
            return o == null || o == DBNull.Value ? 0 : Convert.ToInt32(o);
        }
    }
}