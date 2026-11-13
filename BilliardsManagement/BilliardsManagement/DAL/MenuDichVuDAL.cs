using Microsoft.Data.SqlClient;
using DTO;
using System.Collections.Generic;

namespace DAL
{
    public class MenuDichVuDAL
    {
        public List<MenuDichVuDTO> GetAll()
        {
            var list = new List<MenuDichVuDTO>();
            const string sql = "SELECT MaDV, TenDV, DonGia, TonKho FROM MenuDichVu ORDER BY MaDV";
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(sql, conn);
            conn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new MenuDichVuDTO
                {
                    MaDV = (int)rd["MaDV"],
                    TenDV = rd["TenDV"].ToString()!,
                    DonGia = (decimal)rd["DonGia"],
                    TonKho = (int)rd["TonKho"]
                });
            }
            return list;
        }

        public MenuDichVuDTO? GetByMaDV(int maDV)
        {
            const string sql = "SELECT MaDV, TenDV, DonGia, TonKho FROM MenuDichVu WHERE MaDV=@MaDV";
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaDV", maDV);
            conn.Open();
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return new MenuDichVuDTO
            {
                MaDV = (int)rd["MaDV"],
                TenDV = rd["TenDV"].ToString()!,
                DonGia = (decimal)rd["DonGia"],
                TonKho = (int)rd["TonKho"]
            };
        }
    }
}