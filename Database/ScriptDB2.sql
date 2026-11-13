-- =============================================
-- Tạo CSDL ClinicDB + bảng + dữ liệu mẫu
-- =============================================
IF DB_ID('ClinicDB') IS NOT NULL
    DROP DATABASE ClinicDB;
GO
CREATE DATABASE ClinicDB;
GO
USE ClinicDB;
GO

-- Bệnh nhân
CREATE TABLE BenhNhan (
    MaBN        INT IDENTITY(1,1) PRIMARY KEY,
    HoTen       NVARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(15)   NOT NULL UNIQUE,
    NamSinh     INT           NOT NULL
);

-- Hồ sơ khám
CREATE TABLE HoSoKham (
    MaHoSo      INT IDENTITY(1,1) PRIMARY KEY,
    MaBN        INT NOT NULL FOREIGN KEY REFERENCES BenhNhan(MaBN),
    NgayKham    DATETIME DEFAULT GETDATE(),
    TrieuChung  NVARCHAR(MAX),
    ChanDoan    NVARCHAR(250)
);

-- Thuốc
CREATE TABLE Thuoc (
    MaThuoc     INT IDENTITY(1,1) PRIMARY KEY,
    TenThuoc    NVARCHAR(150) NOT NULL,
    DonViTinh   NVARCHAR(20)  NOT NULL,
    DonGia      DECIMAL(18,2) NOT NULL CHECK (DonGia >= 0),
    SoLuongTon  INT           NOT NULL CHECK (SoLuongTon >= 0)
);

-- Đơn thuốc
CREATE TABLE DonThuoc (
    MaDon       INT IDENTITY(1,1) PRIMARY KEY,
    MaHoSo      INT NOT NULL FOREIGN KEY REFERENCES HoSoKham(MaHoSo),
    TienKham    DECIMAL(18,2) NOT NULL DEFAULT 0,
    TongTien    DECIMAL(18,2) NOT NULL DEFAULT 0,
    TrangThai   NVARCHAR(30)  NOT NULL DEFAULT N'Chưa thanh toán'
);

-- Chi tiết đơn thuốc
CREATE TABLE ChiTietDonThuoc (
    MaDon       INT NOT NULL FOREIGN KEY REFERENCES DonThuoc(MaDon),
    MaThuoc     INT NOT NULL FOREIGN KEY REFERENCES Thuoc(MaThuoc),
    SoLuong     INT NOT NULL CHECK (SoLuong > 0),
    LieuDung    NVARCHAR(100),
    PRIMARY KEY (MaDon, MaThuoc)
);
GO

-- ============ Dữ liệu mẫu ============
INSERT INTO BenhNhan (HoTen, SoDienThoai, NamSinh) VALUES
(N'Nguyễn Văn A', '0901234567', 1990),
(N'Trần Thị B',   '0912345678', 1985),
(N'Lê Hoàng C',   '0987654321', 2000);

INSERT INTO Thuoc (TenThuoc, DonViTinh, DonGia, SoLuongTon) VALUES
(N'Paracetamol 500mg',   N'Viên', 500,  1000),
(N'Amoxicillin 500mg',   N'Viên', 2000, 500),
(N'Vitamin C',           N'Viên', 1000, 800),
(N'Omeprazole 20mg',     N'Viên', 3000, 300),
(N'Ceftriaxone 1g',      N'Lọ',   25000, 50);
GO