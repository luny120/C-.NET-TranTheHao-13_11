-- ============================================================
-- SCRIPT TẠO DATABASE + DỮ LIỆU MẪU
-- Chạy trên SSMS với quyền sysadmin
-- ============================================================
IF DB_ID('BilliardsDB') IS NOT NULL
BEGIN
    ALTER DATABASE BilliardsDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE BilliardsDB;
END
GO

CREATE DATABASE BilliardsDB;
GO
USE BilliardsDB;
GO

-- 1. BÀN BIDA
CREATE TABLE BanBida (
    MaBan       INT IDENTITY(1,1) PRIMARY KEY,
    TenBan      NVARCHAR(50)  NOT NULL,
    LoaiBan     NVARCHAR(50)  NOT NULL,
    GiaTheoGio  DECIMAL(18,2) NOT NULL,
    TrangThai   NVARCHAR(20)  NOT NULL DEFAULT N'Trống'
        CHECK (TrangThai IN (N'Trống', N'Đang chơi'))
);
GO

-- 2. MENU DỊCH VỤ
CREATE TABLE MenuDichVu (
    MaDV    INT IDENTITY(1,1) PRIMARY KEY,
    TenDV   NVARCHAR(100) NOT NULL,
    DonGia  DECIMAL(18,2) NOT NULL,
    TonKho  INT           NOT NULL DEFAULT 0
);
GO

-- 3. HÓA ĐƠN
CREATE TABLE HoaDon (
    MaHD        INT IDENTITY(1,1) PRIMARY KEY,
    MaBan       INT      NOT NULL FOREIGN KEY REFERENCES BanBida(MaBan),
    GioBatDau   DATETIME NOT NULL,
    GioKetThuc  DATETIME NULL,
    TienGioChoi DECIMAL(18,2) NULL DEFAULT 0,
    TienDichVu  DECIMAL(18,2) NULL DEFAULT 0,
    TrangThai   INT      NOT NULL DEFAULT 0   -- 0: Chưa TT, 1: Đã TT
);
GO

-- 4. CHI TIẾT HÓA ĐƠN (PK phức hợp)
CREATE TABLE ChiTietHoaDon (
    MaHD    INT NOT NULL FOREIGN KEY REFERENCES HoaDon(MaHD),
    MaDV    INT NOT NULL FOREIGN KEY REFERENCES MenuDichVu(MaDV),
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    DonGia  DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (MaHD, MaDV)
);
GO

-- ============== DỮ LIỆU MẪU ==============
INSERT INTO BanBida (TenBan, LoaiBan, GiaTheoGio, TrangThai) VALUES
(N'Bàn 1', N'Pool',    60000,  N'Trống'),
(N'Bàn 2', N'Pool',    60000,  N'Trống'),
(N'Bàn 3', N'Pool',    70000,  N'Trống'),
(N'Bàn 4', N'Snooker', 100000, N'Trống'),
(N'Bàn 5', N'Snooker', 100000, N'Trống'),
(N'Bàn 6', N'Phăng',   50000,  N'Trống'),
(N'Bàn 7', N'Phăng',   50000,  N'Trống'),
(N'Bàn 8', N'Pool',    80000,  N'Trống');
GO

INSERT INTO MenuDichVu (TenDV, DonGia, TonKho) VALUES
(N'Nước suối',   10000, 100),
(N'Coca Cola',   15000, 100),
(N'Bia Tiger',   25000, 50),
(N'Mì tôm',      20000, 30),
(N'Khăn lạnh',   5000,  200),
(N'Trái cây',    40000, 20),
(N'Thuê gậy',    20000, 1000);   -- dịch vụ thuê: KHÔNG trừ tồn kho
GO

PRINT N'>>> Tạo CSDL BilliardsDB thành công!';