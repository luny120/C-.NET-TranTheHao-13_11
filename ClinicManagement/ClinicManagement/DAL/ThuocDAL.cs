using Microsoft.Data.SqlClient;
using ClinicManagement.DTO;

namespace ClinicManagement.DAL
{
    public class ThuocDAL
    {
        public List<Thuoc> LayTatCa()
        {
            var list = new List<Thuoc>();
            const string sql = "SELECT MaThuoc, TenThuoc, DonViTinh, DonGia, SoLuongTon FROM Thuoc ORDER BY TenThuoc";
            using (SqlConnection conn = DBConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new Thuoc
                        {
                            MaThuoc = rd.GetInt32(0),
                            TenThuoc = rd.GetString(1),
                            DonViTinh = rd.GetString(2),
                            DonGia = rd.GetDecimal(3),
                            SoLuongTon = rd.GetInt32(4)
                        });
                    }
                }
            }
            return list;
        }

        public void Them(Thuoc t)
        {
            const string sql = @"INSERT INTO Thuoc (TenThuoc, DonViTinh, DonGia, SoLuongTon)
                                 VALUES (@TenThuoc, @DVT, @DonGia, @SoLuongTon)";
            using (SqlConnection conn = DBConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@TenThuoc", t.TenThuoc));
                cmd.Parameters.Add(new SqlParameter("@DVT", t.DonViTinh));
                cmd.Parameters.Add(new SqlParameter("@DonGia", t.DonGia));
                cmd.Parameters.Add(new SqlParameter("@SoLuongTon", t.SoLuongTon));
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Sua(Thuoc t)
        {
            const string sql = @"UPDATE Thuoc SET TenThuoc=@TenThuoc, DonViTinh=@DVT,
                                 DonGia=@DonGia, SoLuongTon=@SoLuongTon WHERE MaThuoc=@MaThuoc";
            using (SqlConnection conn = DBConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@TenThuoc", t.TenThuoc));
                cmd.Parameters.Add(new SqlParameter("@DVT", t.DonViTinh));
                cmd.Parameters.Add(new SqlParameter("@DonGia", t.DonGia));
                cmd.Parameters.Add(new SqlParameter("@SoLuongTon", t.SoLuongTon));
                cmd.Parameters.Add(new SqlParameter("@MaThuoc", t.MaThuoc));
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Xoa(int maThuoc)
        {
            const string sql = "DELETE FROM Thuoc WHERE MaThuoc=@MaThuoc";
            using (SqlConnection conn = DBConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@MaThuoc", maThuoc));
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void NhapTon(int maThuoc, int soLuongThem)
        {
            const string sql = "UPDATE Thuoc SET SoLuongTon = SoLuongTon + @SL WHERE MaThuoc=@MaThuoc";
            using (SqlConnection conn = DBConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@SL", soLuongThem));
                cmd.Parameters.Add(new SqlParameter("@MaThuoc", maThuoc));
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}