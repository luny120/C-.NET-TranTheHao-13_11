using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using LibraryManagement.DTO;
using LibraryManagement.Utils;

namespace LibraryManagement.DAL
{
    public class DocGiaDAL
    {
        public List<DocGiaDTO> GetAll()
        {
            var list = new List<DocGiaDTO>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM DocGia ORDER BY MaDocGia", conn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new DocGiaDTO
                {
                    MaDocGia = rd["MaDocGia"].ToString(),
                    HoTen = rd["HoTen"].ToString(),
                    NgaySinh = rd["NgaySinh"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(rd["NgaySinh"]),
                    GioiTinh = rd["GioiTinh"].ToString(),
                    DiaChi = rd["DiaChi"].ToString(),
                    SoDienThoai = rd["SoDienThoai"].ToString(),
                    NgayHetHanThe = Convert.ToDateTime(rd["NgayHetHanThe"])
                });
            }
            return list;
        }

        public DocGiaDTO? GetById(string maDocGia)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM DocGia WHERE MaDocGia=@Ma", conn);
            cmd.Parameters.AddWithValue("@Ma", maDocGia);
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return new DocGiaDTO
            {
                MaDocGia = rd["MaDocGia"].ToString(),
                HoTen = rd["HoTen"].ToString(),
                NgaySinh = rd["NgaySinh"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(rd["NgaySinh"]),
                GioiTinh = rd["GioiTinh"].ToString(),
                DiaChi = rd["DiaChi"].ToString(),
                SoDienThoai = rd["SoDienThoai"].ToString(),
                NgayHetHanThe = Convert.ToDateTime(rd["NgayHetHanThe"])
            };
        }

        public bool Insert(DocGiaDTO dg)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            const string sql =
                "INSERT INTO DocGia(MaDocGia, HoTen, NgaySinh, GioiTinh, DiaChi, SoDienThoai, NgayHetHanThe) " +
                "VALUES(@Ma, @Ten, @NS, @GT, @DC, @SDT, @NHH)";
            var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Ma", dg.MaDocGia);
            cmd.Parameters.AddWithValue("@Ten", dg.HoTen);
            cmd.Parameters.AddWithValue("@NS", (object?)dg.NgaySinh.Date ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@GT", (object?)dg.GioiTinh ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DC", (object?)dg.DiaChi ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SDT", (object?)dg.SoDienThoai ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NHH", dg.NgayHetHanThe.Date);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Update(DocGiaDTO dg)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            const string sql =
                "UPDATE DocGia SET HoTen=@Ten, NgaySinh=@NS, GioiTinh=@GT, " +
                "DiaChi=@DC, SoDienThoai=@SDT, NgayHetHanThe=@NHH WHERE MaDocGia=@Ma";
            var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Ma", dg.MaDocGia);
            cmd.Parameters.AddWithValue("@Ten", dg.HoTen);
            cmd.Parameters.AddWithValue("@NS", dg.NgaySinh.Date);
            cmd.Parameters.AddWithValue("@GT", (object?)dg.GioiTinh ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DC", (object?)dg.DiaChi ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SDT", (object?)dg.SoDienThoai ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NHH", dg.NgayHetHanThe.Date);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(string maDocGia)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            var cmd = new SqlCommand("DELETE FROM DocGia WHERE MaDocGia=@Ma", conn);
            cmd.Parameters.AddWithValue("@Ma", maDocGia);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}