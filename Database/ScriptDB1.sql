-- =============================================
-- SCRIPT TẠO DATABASE LibraryDB
-- =============================================
IF DB_ID('LibraryDB') IS NOT NULL
    DROP DATABASE LibraryDB;
GO
CREATE DATABASE LibraryDB;
GO
USE LibraryDB;
GO

-- ============= BẢNG ĐỘC GIẢ =============
CREATE TABLE DocGia (
    MaDocGia      NVARCHAR(10)  PRIMARY KEY,
    HoTen         NVARCHAR(100) NOT NULL,
    NgaySinh      DATE          NULL,
    GioiTinh      NVARCHAR(10)  NULL,
    DiaChi        NVARCHAR(200) NULL,
    SoDienThoai   NVARCHAR(15)  NULL,
    NgayHetHanThe DATE          NOT NULL
);
GO

-- ============= BẢNG ĐẦU SÁCH =============
CREATE TABLE DauSach (
    MaSach        NVARCHAR(10)  PRIMARY KEY,
    TenSach       NVARCHAR(200) NOT NULL,
    TacGia        NVARCHAR(100) NULL,
    NhaXuatBan    NVARCHAR(100) NULL,
    NamXuatBan    INT           NULL,
    TheLoai       NVARCHAR(50)  NULL,
    SoLuongTong   INT           NOT NULL,
    SoLuongCon    INT           NOT NULL
);
GO

-- ============= BẢNG PHIẾU MƯỢN =============
CREATE TABLE PhieuMuon (
    MaPhieu   NVARCHAR(20) PRIMARY KEY,
    MaDocGia  NVARCHAR(10) NOT NULL,
    NgayMuon  DATE         NOT NULL,
    HanTra    DATE         NOT NULL,
    TrangThai NVARCHAR(20) NOT NULL,   -- N'Đang mượn' / N'Đã trả' / N'Quá hạn'
    CONSTRAINT FK_PhieuMuon_DocGia FOREIGN KEY (MaDocGia) REFERENCES DocGia(MaDocGia)
);
GO

-- ============= BẢNG CHI TIẾT MƯỢN =============
CREATE TABLE ChiTietMuon (
    MaChiTiet INT          IDENTITY(1,1) PRIMARY KEY,
    MaPhieu   NVARCHAR(20) NOT NULL,
    MaSach    NVARCHAR(10) NOT NULL,
    SoLuong   INT          NOT NULL,
    NgayTra   DATE         NULL,
    CONSTRAINT FK_CTM_PhieuMuon FOREIGN KEY (MaPhieu) REFERENCES PhieuMuon(MaPhieu),
    CONSTRAINT FK_CTM_DauSach  FOREIGN KEY (MaSach)  REFERENCES DauSach(MaSach)
);
GO

-- ============= DỮ LIỆU MẪU =============
INSERT INTO DocGia VALUES
('DG001', N'Nguyễn Văn A', '2000-01-15', N'Nam', N'Hà Nội',  '0901111111', '2026-12-31'),
('DG002', N'Trần Thị B',   '2001-05-20', N'Nữ',  N'TP.HCM',  '0902222222', '2024-01-01'),
('DG003', N'Lê Văn C',     '2002-08-10', N'Nam', N'Đà Nẵng',  '0903333333', '2025-06-30');

INSERT INTO DauSach VALUES
('S001', N'Lập trình C#',            N'Nguyễn A', N'NXB Giáo Dục', 2020, N'Công nghệ', 10, 10),
('S002', N'Cơ sở dữ liệu',           N'Trần B',   N'NXB Khoa Học', 2019, N'Công nghệ', 5,  5),
('S003', N'Truyện Kiều',             N'Nguyễn Du',N'NXB Văn Học',  2018, N'Văn học',   8,  8),
('S004', N'Dế Mèn Phiêu Lưu Ký',     N'Tô Hoài',  N'NXB Kim Đồng', 2020, N'Văn học',   12, 12),
('S005', N'Kinh tế vĩ mô',           N'Phạm C',   N'NXB Thống Kê', 2017, N'Kinh tế',   6,  6);
GO