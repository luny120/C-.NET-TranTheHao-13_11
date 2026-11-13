using Microsoft.Data.SqlClient;
using DTO;
using System;
using System.Collections.Generic;

namespace DAL
{
    public class HoaDonDAL
    {
        /// <summary>Lấy hóa đơn đang mở (chưa TT) của 1 bàn.</summary>
        public HoaDonDTO? GetActiveByMaBan(int maBan)
        {
            const string sql = @"
                SELECT TOP 1 MaHD, MaBan, GioBatDau, GioKetThuc, TienGioChoi, TienDichVu, TrangThai
                FROM HoaDon
                WHERE MaBan=@MaBan AND TrangThai=0
                ORDER BY MaHD DESC";
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaBan", maBan);
            conn.Open();
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return new HoaDonDTO
            {
                MaHD = (int)rd["MaHD"],
                MaBan = (int)rd["MaBan"],
                GioBatDau = (DateTime)rd["GioBatDau"],
                GioKetThuc = rd["GioKetThuc"] == DBNull.Value ? null : (DateTime)rd["GioKetThuc"],
                TienGioChoi = (decimal)rd["TienGioChoi"],
                TienDichVu = (decimal)rd["TienDichVu"],
                TrangThai = (int)rd["TrangThai"]
            };
        }

        /// <summary>
        /// Mở bàn: tạo HoaDon mới + đổi trạng thái bàn sang 'Đang chơi'.
        /// Dùng TRANSACTION để 2 thao tác cùng thành công hoặc cùng rollback.
        /// </summary>
        public int MoBanTransaction(int maBan, DateTime gioBatDau)
        {
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                // 1) Tạo HoaDon
                const string sql1 = @"
                    INSERT INTO HoaDon(MaBan, GioBatDau, TrangThai)
                    VALUES(@MaBan, @GioBatDau, 0);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
                using var cmd1 = new SqlCommand(sql1, conn, tran);
                cmd1.Parameters.AddWithValue("@MaBan", maBan);
                cmd1.Parameters.AddWithValue("@GioBatDau", gioBatDau);
                int maHD = (int)cmd1.ExecuteScalar()!;

                // 2) Đổi trạng thái bàn
                const string sql2 = "UPDATE BanBida SET TrangThai=N'Đang chơi' WHERE MaBan=@MaBan";
                using var cmd2 = new SqlCommand(sql2, conn, tran);
                cmd2.Parameters.AddWithValue("@MaBan", maBan);
                cmd2.ExecuteNonQuery();

                tran.Commit();
                return maHD;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        /// <summary>
        /// THANH TOÁN: cập nhật HoaDon + trừ tồn kho + trả bàn về 'Trống'.
        /// Toàn bộ nằm trong 1 TRANSACTION.
        /// </summary>
        public bool ThanhToanTransaction(HoaDonDTO hd, List<ChiTietHoaDonDTO> chiTiet)
        {
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                // 1) Update HoaDon
                const string sql1 = @"
                    UPDATE HoaDon
                    SET GioKetThuc=@GioKetThuc, TienGioChoi=@TienGioChoi,
                        TienDichVu=@TienDichVu, TrangThai=1
                    WHERE MaHD=@MaHD";
                using (var cmd = new SqlCommand(sql1, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@GioKetThuc", hd.GioKetThuc ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TienGioChoi", hd.TienGioChoi);
                    cmd.Parameters.AddWithValue("@TienDichVu", hd.TienDichVu);
                    cmd.Parameters.AddWithValue("@MaHD", hd.MaHD);
                    cmd.ExecuteNonQuery();
                }

                // 2) Trừ tồn kho (BỎ QUA dịch vụ thuê: tên bắt đầu bằng "Thuê")
                foreach (var ct in chiTiet)
                {
                    if (!string.IsNullOrEmpty(ct.TenDV) &&
                        ct.TenDV.StartsWith("Thuê", StringComparison.OrdinalIgnoreCase))
                        continue;   // thuê gậy/áo/kệ... không trừ tồn

                    const string sql2 = "UPDATE MenuDichVu SET TonKho = TonKho - @SoLuong WHERE MaDV=@MaDV";
                    using var cmd = new SqlCommand(sql2, conn, tran);
                    cmd.Parameters.AddWithValue("@SoLuong", ct.SoLuong);
                    cmd.Parameters.AddWithValue("@MaDV", ct.MaDV);
                    cmd.ExecuteNonQuery();
                }

                // 3) Trả bàn về 'Trống'
                const string sql3 = "UPDATE BanBida SET TrangThai=N'Trống' WHERE MaBan=@MaBan";
                using (var cmd = new SqlCommand(sql3, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@MaBan", hd.MaBan);
                    cmd.ExecuteNonQuery();
                }

                tran.Commit();
                return true;
            }
            catch
            {
                tran.Rollback();
                return false;
            }
        }
    }
}