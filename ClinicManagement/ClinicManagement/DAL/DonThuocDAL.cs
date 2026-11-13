using Microsoft.Data.SqlClient;
using ClinicManagement.DTO;

namespace ClinicManagement.DAL
{
    public class DonThuocDAL
    {
        /// <summary>
        /// Lưu đơn thuốc + chi tiết + trừ tồn kho trong MỘT transaction.
        /// Nếu bất kỳ bước nào thất bại (kể cả không đủ tồn) -> rollback toàn bộ.
        /// </summary>
        public int LuuDonThuoc(DonThuoc don, List<ChiTietDonThuoc> chiTiet)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        // ---------- BƯỚC 1: Kiểm tra tồn kho từng loại thuốc ----------
                        foreach (ChiTietDonThuoc ct in chiTiet)
                        {
                            const string sqlCheck =
                                "SELECT SoLuongTon FROM Thuoc WITH (UPDLOCK, ROWLOCK) WHERE MaThuoc = @MaThuoc";
                            using (SqlCommand cmd = new SqlCommand(sqlCheck, conn, tran))
                            {
                                cmd.Parameters.Add(new SqlParameter("@MaThuoc", ct.MaThuoc));
                                object? r = cmd.ExecuteScalar();
                                if (r == null)
                                    throw new Exception($"Không tìm thấy thuốc có mã {ct.MaThuoc}.");

                                int ton = Convert.ToInt32(r);
                                if (ton < ct.SoLuong)
                                    throw new Exception(
                                        $"Thuốc mã {ct.MaThuoc} không đủ tồn kho (còn {ton}, cần {ct.SoLuong}).");
                            }
                        }

                        // ---------- BƯỚC 2: Insert DonThuoc, lấy MaDon vừa tạo ----------
                        int maDon;
                        const string sqlDon = @"
                            INSERT INTO DonThuoc (MaHoSo, TienKham, TongTien, TrangThai)
                            VALUES (@MaHoSo, @TienKham, @TongTien, @TrangThai);
                            SELECT CAST(SCOPE_IDENTITY() AS INT);";
                        using (SqlCommand cmd = new SqlCommand(sqlDon, conn, tran))
                        {
                            cmd.Parameters.Add(new SqlParameter("@MaHoSo", don.MaHoSo));
                            cmd.Parameters.Add(new SqlParameter("@TienKham", don.TienKham));
                            cmd.Parameters.Add(new SqlParameter("@TongTien", don.TongTien));
                            cmd.Parameters.Add(new SqlParameter("@TrangThai", don.TrangThai));
                            maDon = (int)cmd.ExecuteScalar();
                        }

                        // ---------- BƯỚC 3: Insert chi tiết + trừ tồn kho ----------
                        foreach (ChiTietDonThuoc ct in chiTiet)
                        {
                            const string sqlCT = @"
                                INSERT INTO ChiTietDonThuoc (MaDon, MaThuoc, SoLuong, LieuDung)
                                VALUES (@MaDon, @MaThuoc, @SoLuong, @LieuDung);";
                            using (SqlCommand cmd = new SqlCommand(sqlCT, conn, tran))
                            {
                                cmd.Parameters.Add(new SqlParameter("@MaDon", maDon));
                                cmd.Parameters.Add(new SqlParameter("@MaThuoc", ct.MaThuoc));
                                cmd.Parameters.Add(new SqlParameter("@SoLuong", ct.SoLuong));
                                cmd.Parameters.Add(new SqlParameter("@LieuDung",
                                    (object?)ct.LieuDung ?? DBNull.Value));
                                cmd.ExecuteNonQuery();
                            }

                            const string sqlUpdate =
                                "UPDATE Thuoc SET SoLuongTon = SoLuongTon - @SL WHERE MaThuoc = @MaThuoc";
                            using (SqlCommand cmd = new SqlCommand(sqlUpdate, conn, tran))
                            {
                                cmd.Parameters.Add(new SqlParameter("@SL", ct.SoLuong));
                                cmd.Parameters.Add(new SqlParameter("@MaThuoc", ct.MaThuoc));
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                        return maDon;
                    }
                    catch
                    {
                        try { tran.Rollback(); } catch { /* nuốt lỗi rollback */ }
                        throw;
                    }
                }
            }
        }

        /// <summary>Cập nhật trạng thái đơn thành "Đã thanh toán".</summary>
        public void ThanhToan(int maDon)
        {
            const string sql = "UPDATE DonThuoc SET TrangThai = N'Đã thanh toán' WHERE MaDon = @MaDon";
            using (SqlConnection conn = DBConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@MaDon", maDon));
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}