using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using LibraryManagement.DTO;
using LibraryManagement.Utils;

namespace LibraryManagement.DAL
{
    public class DauSachDAL
    {
        public List<DauSachDTO> GetAll()
        {
            var list = new List<DauSachDTO>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM DauSach ORDER BY MaSach", conn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new DauSachDTO
                {
                    MaSach = rd["MaSach"].ToString(),
                    TenSach = rd["TenSach"].ToString(),
                    TacGia = rd["TacGia"].ToString(),
                    NhaXuatBan = rd["NhaXuatBan"].ToString(),
                    NamXuatBan = rd["NamXuatBan"] == DBNull.Value ? 0 : Convert.ToInt32(rd["NamXuatBan"]),
                    TheLoai = rd["TheLoai"].ToString(),
                    SoLuongTong = Convert.ToInt32(rd["SoLuongTong"]),
                    SoLuongCon = Convert.ToInt32(rd["SoLuongCon"])
                });
            }
            return list;
        }

        public DauSachDTO? GetById(string maSach)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM DauSach WHERE MaSach=@Ma", conn);
            cmd.Parameters.AddWithValue("@Ma", maSach);
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return new DauSachDTO
            {
                MaSach = rd["MaSach"].ToString(),
                TenSach = rd["TenSach"].ToString(),
                TacGia = rd["TacGia"].ToString(),
                NhaXuatBan = rd["NhaXuatBan"].ToString(),
                NamXuatBan = rd["NamXuatBan"] == DBNull.Value ? 0 : Convert.ToInt32(rd["NamXuatBan"]),
                TheLoai = rd["TheLoai"].ToString(),
                SoLuongTong = Convert.ToInt32(rd["SoLuongTong"]),
                SoLuongCon = Convert.ToInt32(rd["SoLuongCon"])
            };
        }

        public bool Insert(DauSachDTO s)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            const string sql =
                "INSERT INTO DauSach(MaSach, TenSach, TacGia, NhaXuatBan, NamXuatBan, TheLoai, SoLuongTong, SoLuongCon) " +
                "VALUES(@Ma, @Ten, @TG, @NXB, @Nam, @TL, @Tong, @Con)";
            var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Ma", s.MaSach);
            cmd.Parameters.AddWithValue("@Ten", s.TenSach);
            cmd.Parameters.AddWithValue("@TG", (object?)s.TacGia ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NXB", (object?)s.NhaXuatBan ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Nam", s.NamXuatBan);
            cmd.Parameters.AddWithValue("@TL", (object?)s.TheLoai ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Tong", s.SoLuongTong);
            cmd.Parameters.AddWithValue("@Con", s.SoLuongCon);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Update(DauSachDTO s)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            const string sql =
                "UPDATE DauSach SET TenSach=@Ten, TacGia=@TG, NhaXuatBan=@NXB, NamXuatBan=@Nam, " +
                "TheLoai=@TL, SoLuongTong=@Tong, SoLuongCon=@Con WHERE MaSach=@Ma";
            var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Ma", s.MaSach);
            cmd.Parameters.AddWithValue("@Ten", s.TenSach);
            cmd.Parameters.AddWithValue("@TG", (object?)s.TacGia ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NXB", (object?)s.NhaXuatBan ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Nam", s.NamXuatBan);
            cmd.Parameters.AddWithValue("@TL", (object?)s.TheLoai ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Tong", s.SoLuongTong);
            cmd.Parameters.AddWithValue("@Con", s.SoLuongCon);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(string maSach)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            var cmd = new SqlCommand("DELETE FROM DauSach WHERE MaSach=@Ma", conn);
            cmd.Parameters.AddWithValue("@Ma", maSach);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}