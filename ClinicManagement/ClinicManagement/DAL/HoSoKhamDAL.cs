using Microsoft.Data.SqlClient;
using ClinicManagement.DTO;

namespace ClinicManagement.DAL
{
    public class HoSoKhamDAL
    {
        public int Them(HoSoKham hs)
        {
            const string sql = @"INSERT INTO HoSoKham (MaBN, TrieuChung, ChanDoan)
                                 VALUES (@MaBN, @TrieuChung, @ChanDoan);
                                 SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection conn = DBConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@MaBN", hs.MaBN));
                cmd.Parameters.Add(new SqlParameter("@TrieuChung", (object?)hs.TrieuChung ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@ChanDoan", (object?)hs.ChanDoan ?? DBNull.Value));
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public HoSoKham? LayTheoMa(int maHoSo)
        {
            const string sql = "SELECT MaHoSo, MaBN, NgayKham, TrieuChung, ChanDoan FROM HoSoKham WHERE MaHoSo = @MaHoSo";
            using (SqlConnection conn = DBConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@MaHoSo", maHoSo));
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        return new HoSoKham
                        {
                            MaHoSo = rd.GetInt32(0),
                            MaBN = rd.GetInt32(1),
                            NgayKham = rd.GetDateTime(2),
                            TrieuChung = rd.IsDBNull(3) ? null : rd.GetString(3),
                            ChanDoan = rd.IsDBNull(4) ? null : rd.GetString(4)
                        };
                    }
                }
            }
            return null;
        }
    }
}