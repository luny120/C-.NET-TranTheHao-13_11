using Microsoft.Data.SqlClient;
using DTO;
using System.Collections.Generic;

namespace DAL
{
    public class BanBidaDAL
    {
        public List<BanBidaDTO> GetAll()
        {
            var list = new List<BanBidaDTO>();
            const string sql = "SELECT MaBan, TenBan, LoaiBan, GiaTheoGio, TrangThai FROM BanBida ORDER BY MaBan";

            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(sql, conn);
            conn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new BanBidaDTO
                {
                    MaBan = (int)rd["MaBan"],
                    TenBan = rd["TenBan"].ToString()!,
                    LoaiBan = rd["LoaiBan"].ToString()!,
                    GiaTheoGio = (decimal)rd["GiaTheoGio"],
                    TrangThai = rd["TrangThai"].ToString()!
                });
            }
            return list;
        }

        public BanBidaDTO? GetByMaBan(int maBan)
        {
            const string sql = "SELECT MaBan, TenBan, LoaiBan, GiaTheoGio, TrangThai FROM BanBida WHERE MaBan=@MaBan";
            using var conn = new SqlConnection(DBConnection.ConnectionString);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaBan", maBan);
            conn.Open();
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return new BanBidaDTO
            {
                MaBan = (int)rd["MaBan"],
                TenBan = rd["TenBan"].ToString()!,
                LoaiBan = rd["LoaiBan"].ToString()!,
                GiaTheoGio = (decimal)rd["GiaTheoGio"],
                TrangThai = rd["TrangThai"].ToString()!
            };
        }
    }
}