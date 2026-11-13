using Microsoft.Data.SqlClient;
using ClinicManagement.DTO;

namespace ClinicManagement.DAL
{
    public class BenhNhanDAL
    {
        public BenhNhan? TimTheoSDT(string sdt)
        {
            const string sql = @"SELECT MaBN, HoTen, SoDienThoai, NamSinh 
                                 FROM BenhNhan WHERE SoDienThoai = @sdt";
            using (SqlConnection conn = DBConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@sdt", sdt));
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        return new BenhNhan
                        {
                            MaBN = rd.GetInt32(0),
                            HoTen = rd.GetString(1),
                            SoDienThoai = rd.GetString(2),
                            NamSinh = rd.GetInt32(3)
                        };
                    }
                }
            }
            return null;
        }

        public int Them(BenhNhan bn)
        {
            const string sql = @"INSERT INTO BenhNhan (HoTen, SoDienThoai, NamSinh)
                                 VALUES (@HoTen, @SDT, @NamSinh);
                                 SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection conn = DBConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@HoTen", bn.HoTen));
                cmd.Parameters.Add(new SqlParameter("@SDT", bn.SoDienThoai));
                cmd.Parameters.Add(new SqlParameter("@NamSinh", bn.NamSinh));
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public List<BenhNhan> LayTatCa()
        {
            var list = new List<BenhNhan>();
            const string sql = "SELECT MaBN, HoTen, SoDienThoai, NamSinh FROM BenhNhan ORDER BY MaBN";
            using (SqlConnection conn = DBConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new BenhNhan
                        {
                            MaBN = rd.GetInt32(0),
                            HoTen = rd.GetString(1),
                            SoDienThoai = rd.GetString(2),
                            NamSinh = rd.GetInt32(3)
                        });
                    }
                }
            }
            return list;
        }
    }
}