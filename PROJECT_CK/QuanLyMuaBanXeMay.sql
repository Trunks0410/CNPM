CREATE DATABASE QuanLyXeMuaBanXeMay;
GO

USE QuanLyXeMuaBanXeMay;
GO

-- =============================
-- 1️⃣ BẢNG PHÂN LOẠI KHÁCH HÀNG
-- =============================
CREATE TABLE PhanLoaiKH (
    MaLoaiKH INT IDENTITY(1,1) PRIMARY KEY,
    TenLoaiKH NVARCHAR(20) NOT NULL UNIQUE
);
GO

-- =============================
-- 2️⃣ BẢNG NHÀ CUNG CẤP
-- =============================
CREATE TABLE NhaCungCap (
    NCCID INT IDENTITY(1,1) PRIMARY KEY,
    TenNCC NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(200),
    SoDienThoai VARCHAR(15) UNIQUE
);
GO

-- =============================
-- 3️⃣ BẢNG KHÁCH HÀNG
-- =============================
CREATE TABLE KhachHang (
    KhachHangID INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(200),
    SoDienThoai VARCHAR(15) UNIQUE,
    MaLoaiKH INT NULL,
	NgayTao DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_KhachHang_PhanLoaiKH FOREIGN KEY (MaLoaiKH)
        REFERENCES PhanLoaiKH(MaLoaiKH)
        ON UPDATE CASCADE
        ON DELETE SET NULL
);
GO


-- =============================
-- 4️⃣ BẢNG XE
-- =============================
CREATE TABLE Xe (
    XeID INT IDENTITY(1,1) PRIMARY KEY,
    TenXe NVARCHAR(100) NOT NULL,
    HangXe NVARCHAR(50) NOT NULL,
    LoaiXe NVARCHAR(50) CHECK (LoaiXe IN ('TayGa', 'XeSo', 'ConTay', 'Dien')),
    MauSac NVARCHAR(50) NOT NULL,
    NamSX INT NOT NULL CHECK (NamSX <= YEAR(GETDATE()) + 1),
    DungTich FLOAT CHECK (DungTich > 0),
    MoTa NVARCHAR(500),
    HinhAnh NVARCHAR(255),
    GiaNhap DECIMAL(18,2) NOT NULL CHECK (GiaNhap >= 0),
    GiaBan DECIMAL(18,2) NOT NULL CHECK (GiaBan >= 0)
);
GO

CREATE TYPE XeTempType AS TABLE
(
    SoKhung NVARCHAR(50),
    SoMay NVARCHAR(50),
    TenXe NVARCHAR(100),
    HangXe NVARCHAR(50),
    LoaiXe NVARCHAR(50),
    MauSac NVARCHAR(50),
    NamSX INT,
    DonGia DECIMAL(18,2),
    HinhAnh NVARCHAR(255),
    MoTa NVARCHAR(500),
    TinhTrang NVARCHAR(20),
	DungTich FLOAT
);
GO

-- =============================
-- 5️⃣ XE CHI TIẾT
-- =============================
CREATE TABLE XeChiTiet (
    XeCTID INT IDENTITY(1,1) PRIMARY KEY,
    XeID INT NOT NULL,
    SoKhung NVARCHAR(50) UNIQUE NOT NULL,
    SoMay NVARCHAR(50) UNIQUE NOT NULL,
    TinhTrang NVARCHAR(20) CHECK (TinhTrang IN (N'Mới', N'Cũ')),
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'TonKho'
        CHECK (TrangThai IN (N'TonKho', N'DaBan', N'DangSua', N'Hong')),
    CONSTRAINT FK_XeChiTiet_Xe FOREIGN KEY (XeID) REFERENCES Xe(XeID)
);
GO

-- =============================
-- 6️⃣ PHIẾU NHẬP
-- =============================
CREATE TABLE PhieuNhap (
    PhieuNhapID INT IDENTITY(1,1) PRIMARY KEY,
    NgayNhap DATE NOT NULL DEFAULT GETDATE(),
    LoaiNhap NVARCHAR(20) CHECK (LoaiNhap IN ('NCC', 'KhachHang')),
    NCCID INT NULL,
    KhachHangID INT NULL,
    CONSTRAINT FK_PhieuNhap_NCC FOREIGN KEY (NCCID) REFERENCES NhaCungCap(NCCID),
    CONSTRAINT FK_PhieuNhap_KH FOREIGN KEY (KhachHangID) REFERENCES KhachHang(KhachHangID),
    CONSTRAINT CK_PhieuNhap_Loai CHECK (
        (LoaiNhap = 'NCC' AND NCCID IS NOT NULL AND KhachHangID IS NULL) OR
        (LoaiNhap = 'KhachHang' AND KhachHangID IS NOT NULL AND NCCID IS NULL)
    )
);
GO

-- =============================
-- 7️⃣ CHI TIẾT NHẬP
-- =============================
CREATE TABLE ChiTietNhap (
    ChiTietNhapID INT IDENTITY(1,1) PRIMARY KEY,
    PhieuNhapID INT NOT NULL,
    XeCTID INT NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL CHECK (DonGia >= 0),
    CONSTRAINT FK_CTNhap_PhieuNhap FOREIGN KEY (PhieuNhapID) REFERENCES PhieuNhap(PhieuNhapID),
    CONSTRAINT FK_CTNhap_XeCT FOREIGN KEY (XeCTID) REFERENCES XeChiTiet(XeCTID)
);
GO



-- =============================
-- 11️⃣ ƯU ĐÃI
-- =============================
CREATE TABLE UuDai (
    MaUuDai VARCHAR(20) PRIMARY KEY,
    TenUuDai NVARCHAR(100) NOT NULL,
    
    -- Trường mới: Xác định loại ưu đãi (VD: 'PHANTRAM', 'TRUCTIEP')
    LoaiUuDai VARCHAR(20) NOT NULL CHECK (LoaiUuDai IN ('PHANTRAM', 'TRUCTIEP')), 
    
    -- Trường mới: Lưu trữ giá trị giảm (có thể là % hoặc số tiền)
    GiaTriGiam DECIMAL(18, 2) NOT NULL CHECK (GiaTriGiam >= 0), 
    DieuKienToiThieu DECIMAL(18,2) NOT NULL DEFAULT 0,
    -- Các trường ngày tháng và mô tả giữ nguyên
    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE NOT NULL,
    MoTa NVARCHAR(200),
    
    -- Ràng buộc kiểm tra ngày
    CONSTRAINT CK_UuDai_Ngay CHECK (NgayKetThuc >= NgayBatDau),
    
    -- Ràng buộc kiểm tra giá trị giảm theo loại (tùy chọn nhưng nên có)
    -- Nếu là 'PHANTRAM', GiaTriGiam phải <= 100
    CONSTRAINT CK_UuDai_GiaTri CHECK (
        (LoaiUuDai = 'PHANTRAM' AND GiaTriGiam <= 100) OR 
        (LoaiUuDai = 'TRUCTIEP') -- Giảm trực tiếp có thể không có giới hạn trên cụ thể ở đây
    ) 
);
GO




-- =============================
-- 13️⃣ LỊCH HẸN BẢO TRÌ
-- =============================
CREATE TABLE LichHenBaoTri(
    MaLichHen INT IDENTITY(1,1) PRIMARY KEY,
    XeCTID INT NOT NULL,
    NgayHen DATE NOT NULL,
    CaLamViec NVARCHAR(20) NOT NULL,
    TrangThaiHen NVARCHAR(50) DEFAULT N'Đã xác nhận',
    LyDoHen NVARCHAR(500),
    GhiChu NVARCHAR(500),
    NgayTao DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (XeCTID) REFERENCES XeChiTiet(XeCTID)
        ON UPDATE CASCADE
        ON DELETE CASCADE
);
GO

-- =============================
-- 14️⃣ NHÂN VIÊN + CÔNG + LƯƠNG + TÀI KHOẢN + KPI
-- =============================
CREATE TABLE NhanVien (
    MaNV INT PRIMARY KEY IDENTITY (1,1),
    HoTenNV NVARCHAR(255) NOT NULL,
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    SoDT VARCHAR(15),
    Email VARCHAR(100) UNIQUE,
    CCCD VARCHAR(20) NULL,
    DiaChi NVARCHAR(255),
    NgayNhanViec DATE NOT NULL,
    LuongCB DECIMAL(18,2) NOT NULL,
    HinhAnh NVARCHAR(255)
);
GO

CREATE TABLE ChamCong (
    MaChamCong INT PRIMARY KEY IDENTITY(1,1),
    MaNV INT NOT NULL,
    NgayLamViec DATE NOT NULL,
    TgVaoLam TIME,
    TgTanCa TIME,
    TrangThai NVARCHAR(50) DEFAULT N'Có mặt',
    LyDoNghiPhep NVARCHAR(255),
    FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV) ON DELETE CASCADE
);
GO

CREATE TABLE Luong (
    MaLuong INT PRIMARY KEY IDENTITY(1,1),
    MaNV INT NOT NULL,
    ThangLuong DATE NOT NULL,
    TongGioThang DECIMAL(10,2),
    GioLamThem DECIMAL(10,2) DEFAULT 0,
    KhauTru DECIMAL(18,2) DEFAULT 0,
    TienThuong DECIMAL(18,2) DEFAULT 0,
    LgThucNhan DECIMAL(18,2) NOT NULL,
    NgayThanhToan DATE,
    FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV) ON DELETE CASCADE
);
GO

CREATE TABLE TaiKhoan (
    MaTK INT PRIMARY KEY IDENTITY(1,1),
    MaNV INT,
    TenTK VARCHAR(50) NOT NULL,
    MatKhau VARCHAR(255) NOT NULL,
    VaiTro NVARCHAR(100) NOT NULL,
    FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV) ON DELETE CASCADE
);
GO

CREATE TABLE ThongBao (
    MaThongBao INT PRIMARY KEY IDENTITY(1,1),
    TieuDe NVARCHAR(255) NOT NULL,
    LoaiThongBao NVARCHAR(100) NOT NULL,
    NoiDung NVARCHAR(MAX) NOT NULL,
    NgayTao DATETIME DEFAULT GETDATE(),
    MaNV INT NULL,
    FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV) ON DELETE SET NULL
);
GO

-- =============================
-- 15️⃣ ĐƠN HÀNG + CHI TIẾT + THANH TOÁN
-- =============================
CREATE TABLE DonHang (
    MaDonHang INT IDENTITY(1,1) PRIMARY KEY,
    KhachHangID INT NOT NULL,
    MaNV INT NULL,
    MaUuDai VARCHAR(20) NULL,
    NgayGiaoDich DATE NOT NULL DEFAULT GETDATE(),
    TongTien DECIMAL(18,2) DEFAULT 0,
    TrangThai NVARCHAR(50) DEFAULT N'Chờ xử lý',
    GhiChu NVARCHAR(255),
    SoLuong INT DEFAULT 0,
    FOREIGN KEY (KhachHangID) REFERENCES KhachHang(KhachHangID)
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV)
        ON UPDATE CASCADE
        ON DELETE SET NULL,
    FOREIGN KEY (MaUuDai) REFERENCES UuDai(MaUuDai)
        ON UPDATE CASCADE
        ON DELETE SET NULL
);
GO

CREATE TABLE ChiTietDonHang (
    MaCTDH INT IDENTITY(1,1) PRIMARY KEY,
    MaDonHang INT NOT NULL,
    XeCTID INT NOT NULL,
    SoLuong INT NOT NULL DEFAULT 1,
    DonGia DECIMAL(18,2) NOT NULL CHECK (DonGia >= 0),
    ThanhTien AS (SoLuong * DonGia) PERSISTED,
    FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang)
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    FOREIGN KEY (XeCTID) REFERENCES XeChiTiet(XeCTID)
        ON UPDATE CASCADE
        ON DELETE CASCADE
);
GO

CREATE TABLE ThanhToan (
    MaThanhToan INT IDENTITY(1,1) PRIMARY KEY,
    MaDonHang INT NOT NULL,
    SoTien DECIMAL(18,2) NOT NULL CHECK (SoTien > 0),
    NgayThanhToan DATE NOT NULL DEFAULT GETDATE(),
    PhuongThuc NVARCHAR(50) NOT NULL,
    TrangThai NVARCHAR(50) DEFAULT N'Hoàn thành',
    FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang)
        ON UPDATE CASCADE
        ON DELETE CASCADE
);
GO

CREATE TABLE DanhMuc (
    DanhMucID INT IDENTITY(1,1) PRIMARY KEY,
    LoaiDanhMuc NVARCHAR(50) NOT NULL, -- HangXe, LoaiXe, MauSac, TinhTrang, TrangThai
    GiaTri NVARCHAR(100) NOT NULL,
    CONSTRAINT UQ_DanhMuc UNIQUE(LoaiDanhMuc, GiaTri)
);


--1. Hàm trả về giá trị--

IF OBJECT_ID('fn_TongGiaTriNhap', 'FN') IS NOT NULL
    DROP FUNCTION fn_TongGiaTriNhap;
GO

CREATE FUNCTION fn_TongGiaTriNhap()
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @TongGiaTri DECIMAL(18,2);
    SELECT @TongGiaTri = SUM(DonGia)
    FROM ChiTietNhap;
    RETURN ISNULL(@TongGiaTri, 0);
END;
GO

IF OBJECT_ID('fn_TongSoXeNhap', 'FN') IS NOT NULL
    DROP FUNCTION fn_TongSoXeNhap;
GO

CREATE FUNCTION fn_TongSoXeNhap()
RETURNS INT
AS
BEGIN
    DECLARE @SoLuong INT;
    SELECT @SoLuong = COUNT(*)
    FROM ChiTietNhap;
    RETURN ISNULL(@SoLuong, 0);
END;
GO

IF OBJECT_ID('fn_GetTongSoXeTonKho', 'FN') IS NOT NULL
    DROP FUNCTION fn_GetTongSoXeTonKho;
GO

CREATE FUNCTION fn_GetTongSoXeTonKho()
RETURNS INT
AS
BEGIN
    DECLARE @TongSoXe INT;
    SELECT @TongSoXe = COUNT(*)
    FROM XeChiTiet
    WHERE TrangThai = N'TonKho';
    RETURN @TongSoXe;
END;
GO

IF OBJECT_ID('fn_GetTongGiaTriTonKho', 'FN') IS NOT NULL
    DROP FUNCTION fn_GetTongGiaTriTonKho;
GO

CREATE FUNCTION fn_GetTongGiaTriTonKho()
RETURNS DECIMAL(18, 2)
AS
BEGIN
    DECLARE @TongGiaTri DECIMAL(18, 2);
    SELECT @TongGiaTri = SUM(x.GiaBan)
    FROM XeChiTiet xct
    INNER JOIN Xe x ON xct.XeID = x.XeID
    WHERE xct.TrangThai = N'TonKho';
    RETURN ISNULL(@TongGiaTri, 0);
END;
GO

IF OBJECT_ID('fn_GetTongLoiNhuanXe', 'FN') IS NOT NULL
    DROP FUNCTION fn_GetTongLoiNhuanXe;
GO

CREATE FUNCTION fn_GetTongLoiNhuanXe()
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @TongLoiNhuan DECIMAL(18,2);

    SELECT @TongLoiNhuan = SUM(x.GiaBan - x.GiaNhap)
    FROM Xe x
    INNER JOIN XeChiTiet xct ON x.XeID = xct.XeID
    WHERE xct.TrangThai = N'TonKho'; -- hoặc bỏ điều kiện này nếu muốn tính toàn bộ

    RETURN ISNULL(@TongLoiNhuan, 0);
END;
GO

IF OBJECT_ID('fn_IsKhachHangExistsBySDT', 'FN') IS NOT NULL
    DROP FUNCTION fn_IsKhachHangExistsBySDT;
GO
CREATE FUNCTION fn_IsKhachHangExistsBySDT(@SoDT VARCHAR(15))
RETURNS BIT
AS
BEGIN
    DECLARE @Exists BIT = 0;
    IF EXISTS (SELECT 1 FROM KhachHang WHERE SoDienThoai = @SoDT)
        SET @Exists = 1;
    RETURN @Exists;
END;
GO

IF OBJECT_ID('fn_GetLastNhapByLoaiXe', 'FN') IS NOT NULL
    DROP FUNCTION fn_GetLastNhapByLoaiXe;
GO
CREATE FUNCTION fn_GetLastNhapByLoaiXe(@LoaiXe NVARCHAR(50))
RETURNS DATE
AS
BEGIN
    RETURN (
        SELECT MAX(pn.NgayNhap)
        FROM PhieuNhap pn
        INNER JOIN ChiTietNhap ctn ON pn.PhieuNhapID = ctn.PhieuNhapID
        INNER JOIN XeChiTiet xct ON ctn.XeCTID = xct.XeCTID
        INNER JOIN Xe x ON xct.XeID = x.XeID
        WHERE x.LoaiXe = @LoaiXe
    );
END;
GO

IF OBJECT_ID('fn_TaoMoTaXe', 'FN') IS NOT NULL
    DROP FUNCTION fn_TaoMoTaXe;
GO
CREATE FUNCTION fn_TaoMoTaXe
(
    @TenXe NVARCHAR(100),
    @HangXe NVARCHAR(50),
    @LoaiXe NVARCHAR(50),
    @MauSac NVARCHAR(50),
    @NamSX INT,
    @DungTich FLOAT,
    @GiaBan DECIMAL(18,2)
)
RETURNS NVARCHAR(500)
AS
BEGIN
    DECLARE @MoTa NVARCHAR(500);

    SET @MoTa = 
        N'Xe ' + @HangXe + N' ' + @TenXe + 
        N' (' + @LoaiXe + N') ' +
        N'màu ' + @MauSac +
        N', sản xuất năm ' + CAST(@NamSX AS NVARCHAR) +
        N', dung tích ' + CAST(@DungTich AS NVARCHAR) + N' cc' +
        N', giá bán: ' + FORMAT(@GiaBan, 'N0');

    RETURN @MoTa;
END;
GO

--2. Hàm trả về bảng--

IF OBJECT_ID('fn_GetTenNhaCungCap', 'IF') IS NOT NULL
    DROP FUNCTION fn_GetTenNhaCungCap;
GO

CREATE FUNCTION fn_GetTenNhaCungCap()
RETURNS TABLE
AS
RETURN
(
    SELECT TenNCC
    FROM NhaCungCap
);
GO


IF OBJECT_ID('fn_GetTenKhachHang', 'IF') IS NOT NULL
    DROP FUNCTION fn_GetTenKhachHang;
GO

CREATE FUNCTION fn_GetTenKhachHang()
RETURNS TABLE
AS
RETURN
(
    SELECT HoTen
    FROM KhachHang
);
GO

IF OBJECT_ID('fn_GetSoDTNhaCungCap', 'IF') IS NOT NULL
    DROP FUNCTION fn_GetSoDTNhaCungCap;
GO

CREATE FUNCTION fn_GetSoDTNhaCungCap()
RETURNS TABLE
AS
RETURN
(
    SELECT SoDienThoai
    FROM NhaCungCap
);
GO

IF OBJECT_ID('fn_GetSoDTKhachHang', 'IF') IS NOT NULL
    DROP FUNCTION fn_GetSoDTKhachHang;
GO

CREATE FUNCTION fn_GetSoDTKhachHang()
RETURNS TABLE
AS
RETURN
(
    SELECT SoDienThoai
    FROM KhachHang
);
GO

	
CREATE FUNCTION fn_GetAllXeTonKho()
RETURNS TABLE
AS
RETURN
(
    SELECT 
        xct.XeCTID,
        x.TenXe,
        x.HangXe,
        x.LoaiXe,
        x.MauSac,
        x.NamSX,
        x.DungTich,
        xct.SoKhung,
        xct.SoMay,
        xct.TinhTrang,
        x.GiaNhap as DonGia,
        x.HinhAnh
    FROM XeChiTiet xct
    INNER JOIN Xe x ON xct.XeID = x.XeID
    WHERE xct.TrangThai = N'TonKho'
);
GO

IF OBJECT_ID('fn_GetDataForPieChart', 'IF') IS NOT NULL
    DROP FUNCTION fn_GetDataForPieChart;
GO

CREATE FUNCTION fn_GetDataForPieChart(@Criteria NVARCHAR(50))
RETURNS TABLE
AS
RETURN
(
    -- Tình trạng
    SELECT TinhTrang AS Category, COUNT(*) AS SoLuong
    FROM XeChiTiet
    WHERE @Criteria = N'Tình trạng'
    GROUP BY TinhTrang

    UNION ALL

    -- Trạng thái
    SELECT TrangThai AS Category, COUNT(*) AS SoLuong
    FROM XeChiTiet
    WHERE @Criteria = N'Trạng thái'
    GROUP BY TrangThai

    UNION ALL

    -- Nguồn nhập
    SELECT 
        CASE WHEN NCCID IS NOT NULL THEN N'NCC' ELSE N'Khách hàng' END AS Category,
        COUNT(*) AS SoLuong
    FROM PhieuNhap
    WHERE @Criteria = N'Nguồn nhập'
    GROUP BY CASE WHEN NCCID IS NOT NULL THEN N'NCC' ELSE N'Khách hàng' END
);
GO

IF OBJECT_ID('fn_GetDanhSachXeNhap', 'IF') IS NOT NULL
    DROP FUNCTION fn_GetDanhSachXeNhap;
GO

CREATE FUNCTION fn_GetDanhSachXeNhap(@PhieuNhapID INT)
RETURNS TABLE
AS
RETURN
(
    SELECT 
        ctn.PhieuNhapID,
        x.TenXe,
        x.HangXe,
        x.LoaiXe,
        x.MauSac,
        x.NamSX,
        x.DungTich,
        xct.SoKhung,
        xct.SoMay,
        xct.TinhTrang,
        xct.XeCTID,
        ctn.DonGia,
        x.HinhAnh,
        x.MoTa
    FROM ChiTietNhap ctn
    INNER JOIN XeChiTiet xct ON ctn.XeCTID = xct.XeCTID
    INNER JOIN Xe x ON xct.XeID = x.XeID
    WHERE ctn.PhieuNhapID = @PhieuNhapID
);
GO

IF OBJECT_ID('fn_GetThongTinChungPhieuNhap', 'IF') IS NOT NULL
    DROP FUNCTION fn_GetThongTinChungPhieuNhap;
GO

CREATE FUNCTION fn_GetThongTinChungPhieuNhap(@PhieuNhapID INT)
RETURNS TABLE
AS
RETURN
(
    SELECT 
        pn.PhieuNhapID,
        pn.NgayNhap,
        pn.LoaiNhap,
        ncc.TenNCC,
        ncc.SoDienThoai AS SoDienThoaiNCC,
        ncc.DiaChi AS DiaChiNCC,
        kh.HoTen AS TenKhachHang,
        kh.SoDienThoai AS SoDienThoaiKH,
        kh.DiaChi AS DiaChiKH
    FROM PhieuNhap pn
    LEFT JOIN NhaCungCap ncc ON pn.NCCID = ncc.NCCID
    LEFT JOIN KhachHang kh ON pn.KhachHangID = kh.KhachHangID
    WHERE pn.PhieuNhapID = @PhieuNhapID
);
GO


IF OBJECT_ID('sp_InsertPhieuNhapWithXe', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertPhieuNhapWithXe;
GO

CREATE PROCEDURE sp_InsertPhieuNhapWithXe
   @NgayNhap DATE,
   @LoaiNhap NVARCHAR(20),
   @Ten NVARCHAR(100),
   @SoDT VARCHAR(15),
   @DiaChi NVARCHAR(200),
   @XeList XeTempType READONLY   -- bảng tạm truyền danh sách xe
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @PhieuNhapID INT;
    DECLARE @NCCID INT = NULL;
    DECLARE @KhachHangID INT = NULL;

    BEGIN TRANSACTION;
    BEGIN TRY
        -- Xử lý NCC hoặc KH
        IF @LoaiNhap = N'NCC'
        BEGIN
            SELECT @NCCID = NCCID FROM NhaCungCap WHERE SoDienThoai = @SoDT;
            IF @NCCID IS NULL
            BEGIN
                INSERT INTO NhaCungCap (TenNCC, DiaChi, SoDienThoai)
                VALUES (@Ten, @DiaChi, @SoDT);
                SET @NCCID = SCOPE_IDENTITY();
            END
        END
        ELSE IF @LoaiNhap = N'KhachHang'
        BEGIN
            SELECT @KhachHangID = KhachHangID FROM KhachHang WHERE SoDienThoai = @SoDT;
            IF @KhachHangID IS NULL
            BEGIN
                INSERT INTO KhachHang (HoTen, DiaChi, SoDienThoai)
                VALUES (@Ten, @DiaChi, @SoDT);
                SET @KhachHangID = SCOPE_IDENTITY();
            END
        END

        -- Thêm phiếu nhập
        INSERT INTO PhieuNhap (NgayNhap, LoaiNhap, NCCID, KhachHangID)
        VALUES (@NgayNhap, @LoaiNhap, @NCCID, @KhachHangID);
        SET @PhieuNhapID = SCOPE_IDENTITY();

        -- Thêm Xe và XeChiTiet
        MERGE Xe AS target
        USING (
            SELECT DISTINCT TenXe, HangXe, LoaiXe, MauSac, NamSX, DungTich, DonGia, HinhAnh, MoTa
            FROM @XeList
        ) AS source
        ON target.TenXe = source.TenXe AND target.HangXe = source.HangXe 
           AND target.LoaiXe = source.LoaiXe AND target.MauSac = source.MauSac 
           AND target.NamSX = source.NamSX AND target.DungTich = source.DungTich
        WHEN NOT MATCHED THEN
            INSERT (TenXe, HangXe, LoaiXe, MauSac, NamSX, GiaNhap, GiaBan, HinhAnh, MoTa, DungTich)
            VALUES (source.TenXe, source.HangXe, source.LoaiXe, source.MauSac, source.NamSX,
                    source.DonGia, source.DonGia * 1.1, source.HinhAnh, source.MoTa, source.DungTich);

        INSERT INTO XeChiTiet (XeID, SoKhung, SoMay, TinhTrang)
        SELECT x.XeID, t.SoKhung, t.SoMay, t.TinhTrang
        FROM @XeList t
        CROSS APPLY (
            SELECT TOP 1 XeID FROM Xe x
            WHERE x.TenXe = t.TenXe AND x.HangXe = t.HangXe AND x.LoaiXe = t.LoaiXe
              AND x.MauSac = t.MauSac AND x.NamSX = t.NamSX AND x.DungTich = t.DungTich
            ORDER BY XeID DESC
        ) x
        WHERE NOT EXISTS (SELECT 1 FROM XeChiTiet xct WHERE xct.SoKhung = t.SoKhung)
          AND NOT EXISTS (SELECT 1 FROM XeChiTiet xct WHERE xct.SoMay = t.SoMay);

        INSERT INTO ChiTietNhap (PhieuNhapID, XeCTID, DonGia)
        SELECT @PhieuNhapID, xct.XeCTID, t.DonGia
        FROM @XeList t
        CROSS APPLY (
            SELECT XeCTID FROM XeChiTiet xct WHERE xct.SoKhung = t.SoKhung
        ) xct;

        COMMIT TRANSACTION;
        SELECT @PhieuNhapID AS PhieuNhapID;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

IF OBJECT_ID('sp_InsertDanhMuc', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertDanhMuc;
GO
CREATE PROCEDURE sp_InsertDanhMuc
    @LoaiDanhMuc NVARCHAR(50),
    @GiaTri NVARCHAR(100)
AS
BEGIN
    -- BỎ SET NOCOUNT ON để ExecuteNonQuery trả về số dòng
    IF EXISTS (SELECT 1 FROM DanhMuc WHERE LoaiDanhMuc = @LoaiDanhMuc AND GiaTri = @GiaTri)
    BEGIN
        RAISERROR('Giá trị đã tồn tại trong loại danh mục này', 16, 1);
        RETURN;
    END

    INSERT INTO DanhMuc (LoaiDanhMuc, GiaTri)
    VALUES (@LoaiDanhMuc, @GiaTri);

    SELECT SCOPE_IDENTITY(); -- Trả về ID vừa thêm
END;
GO

IF OBJECT_ID('sp_InsertNhaCungCap', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertNhaCungCap;
GO

CREATE PROCEDURE sp_InsertNhaCungCap
    @TenNCC NVARCHAR(100),
    @DiaChi NVARCHAR(200),
    @SoDienThoai VARCHAR(15)
AS
BEGIN
    INSERT INTO NhaCungCap (TenNCC, DiaChi, SoDienThoai)
    VALUES (@TenNCC, @DiaChi, @SoDienThoai);
END;
GO



--2. UPDATE--

IF OBJECT_ID('sp_UpdateChiTietXeNhap', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateChiTietXeNhap;
GO

CREATE PROCEDURE sp_UpdateChiTietXeNhap
    @PhieuNhapID INT,
    @SoKhung NVARCHAR(50),
    @TenXe NVARCHAR(100),
    @HangXe NVARCHAR(50),
    @LoaiXe NVARCHAR(50),
    @MauSac NVARCHAR(50),
	@DungTich FLOAT,
    @NamSX INT,
    @SoMay NVARCHAR(50),
    @DonGia DECIMAL(18,2),
    @HinhAnh NVARCHAR(200),
    @MoTa NVARCHAR(MAX),
    @TinhTrang NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Cập nhật thông tin Xe
        UPDATE xe
        SET TenXe=@TenXe, HangXe=@HangXe, LoaiXe=@LoaiXe, MauSac=@MauSac,
            NamSX=@NamSX, MoTa=@MoTa, HinhAnh=@HinhAnh, GiaNhap=@DonGia, GiaBan=@DonGia*1.1, DungTich=@DungTich
        FROM Xe xe
        INNER JOIN XeChiTiet xct ON xe.XeID=xct.XeID
        INNER JOIN ChiTietNhap ctn ON xct.XeCTID=ctn.XeCTID
        WHERE ctn.PhieuNhapID=@PhieuNhapID AND xct.SoKhung=@SoKhung;

        UPDATE XeChiTiet
        SET SoMay=@SoMay, TinhTrang=@TinhTrang
        FROM XeChiTiet xct
        INNER JOIN ChiTietNhap ctn ON ctn.XeCTID=xct.XeCTID
        WHERE ctn.PhieuNhapID=@PhieuNhapID AND xct.SoKhung=@SoKhung;

        UPDATE ChiTietNhap
        SET DonGia=@DonGia
        FROM ChiTietNhap ctn
        INNER JOIN XeChiTiet xct ON ctn.XeCTID=xct.XeCTID
        WHERE ctn.PhieuNhapID=@PhieuNhapID AND xct.SoKhung=@SoKhung;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

IF OBJECT_ID('sp_UpdatePhieuNhap', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdatePhieuNhap;
GO

CREATE PROCEDURE sp_UpdatePhieuNhap
    @PhieuNhapID INT,
    @NgayNhap DATE,
    @LoaiNhap NVARCHAR(20),
    @Ten NVARCHAR(100),
    @SoDT VARCHAR(15),
    @DiaChi NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE PhieuNhap
        SET NgayNhap=@NgayNhap, LoaiNhap=@LoaiNhap
        WHERE PhieuNhapID=@PhieuNhapID;

        IF @LoaiNhap=N'NCC'
            UPDATE NhaCungCap
            SET TenNCC=@Ten, SoDienThoai=@SoDT, DiaChi=@DiaChi
            WHERE NCCID=(SELECT NCCID FROM PhieuNhap WHERE PhieuNhapID=@PhieuNhapID);
        ELSE IF @LoaiNhap=N'KhachHang'
            UPDATE KhachHang
            SET HoTen=@Ten, SoDienThoai=@SoDT, DiaChi=@DiaChi
            WHERE KhachHangID=(SELECT KhachHangID FROM PhieuNhap WHERE PhieuNhapID=@PhieuNhapID);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

IF OBJECT_ID('sp_UpdateDanhMuc', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateDanhMuc;
GO

CREATE PROCEDURE sp_UpdateDanhMuc
    @DanhMucID INT,
    @GiaTri NVARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM DanhMuc WHERE GiaTri = @GiaTri AND DanhMucID <> @DanhMucID)
    BEGIN
        RAISERROR('Giá trị đã tồn tại trong loại danh mục này', 16, 1);
        RETURN;
    END

    UPDATE DanhMuc
    SET GiaTri = @GiaTri
    WHERE DanhMucID = @DanhMucID;
END;
GO

IF OBJECT_ID('sp_UpdateNhaCungCap', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateNhaCungCap;
GO

CREATE PROCEDURE sp_UpdateNhaCungCap
    @NCCID INT,
    @TenNCC NVARCHAR(100),
    @DiaChi NVARCHAR(200),
    @SoDienThoai VARCHAR(15)
AS
BEGIN
    UPDATE NhaCungCap
    SET TenNCC = @TenNCC,
        DiaChi = @DiaChi,
        SoDienThoai = @SoDienThoai
    WHERE NCCID = @NCCID;
END;
GO


--3. DELETE--

IF OBJECT_ID('sp_DeletePhieuNhap','P') IS NOT NULL
    DROP PROCEDURE sp_DeletePhieuNhap;
GO
CREATE PROCEDURE sp_DeletePhieuNhap
    @PhieuNhapID INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM PhieuNhap WHERE PhieuNhapID=@PhieuNhapID)
            THROW 50001,'Phiếu nhập không tồn tại.',1;

        DELETE FROM ChiTietNhap WHERE PhieuNhapID=@PhieuNhapID;

        DELETE FROM XeChiTiet WHERE XeCTID NOT IN (SELECT XeCTID FROM ChiTietNhap);

        DELETE FROM PhieuNhap WHERE PhieuNhapID=@PhieuNhapID;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO


IF OBJECT_ID('sp_DeleteXeFromPhieuNhap','P') IS NOT NULL
    DROP PROCEDURE sp_DeleteXeFromPhieuNhap;
GO
CREATE PROCEDURE sp_DeleteXeFromPhieuNhap
    @PhieuNhapID INT,
    @XeCTID INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM PhieuNhap WHERE PhieuNhapID=@PhieuNhapID)
            THROW 50001,'Phiếu nhập không tồn tại.',1;

        IF NOT EXISTS (SELECT 1 FROM XeChiTiet WHERE XeCTID=@XeCTID)
            THROW 50002,'Xe chi tiết không tồn tại.',1;

        IF NOT EXISTS (SELECT 1 FROM ChiTietNhap WHERE PhieuNhapID=@PhieuNhapID AND XeCTID=@XeCTID)
            THROW 50003,'Xe chi tiết không thuộc phiếu nhập này.',1;

        DELETE FROM ChiTietNhap WHERE PhieuNhapID=@PhieuNhapID AND XeCTID=@XeCTID;

        IF NOT EXISTS (SELECT 1 FROM ChiTietNhap WHERE XeCTID=@XeCTID)
            DELETE FROM XeChiTiet WHERE XeCTID=@XeCTID;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

IF OBJECT_ID('sp_DeleteDanhMuc', 'P') IS NOT NULL
    DROP PROCEDURE sp_DeleteDanhMuc;
GO

CREATE PROCEDURE sp_DeleteDanhMuc
    @DanhMucID INT
AS
BEGIN
    DELETE FROM DanhMuc
    WHERE DanhMucID = @DanhMucID;
END;
GO

IF OBJECT_ID('sp_DeleteNhaCungCap', 'P') IS NOT NULL
    DROP PROCEDURE sp_DeleteNhaCungCap;
GO

CREATE PROCEDURE sp_DeleteNhaCungCap
    @NCCID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- 1 = không tồn tại
    IF NOT EXISTS (SELECT 1 FROM NhaCungCap WHERE NCCID = @NCCID)
    BEGIN
        RETURN 1;
    END

    -- 2 = đang được tham chiếu
    IF EXISTS (SELECT 1 FROM PhieuNhap WHERE NCCID = @NCCID)
    BEGIN
        RETURN 2;
    END

    -- 0 = xóa thành công
    DELETE FROM NhaCungCap WHERE NCCID = @NCCID;
    IF @@ROWCOUNT = 0
        RETURN 3; -- 3 = lỗi không xác định

    RETURN 0;
END
GO

--4. SELECT--

IF OBJECT_ID('sp_GetDanhSachPhieuNhap', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetDanhSachPhieuNhap;
GO

CREATE PROCEDURE sp_GetDanhSachPhieuNhap
AS
BEGIN
    SELECT 
        pn.PhieuNhapID AS MaPhieu,
        pn.NgayNhap,
        pn.LoaiNhap,
        ISNULL(SUM(ctn.DonGia), 0) AS TongTien
    FROM PhieuNhap pn
    LEFT JOIN ChiTietNhap ctn ON pn.PhieuNhapID = ctn.PhieuNhapID
    GROUP BY pn.PhieuNhapID, pn.NgayNhap, pn.LoaiNhap
    ORDER BY pn.NgayNhap DESC;
END;
GO

IF OBJECT_ID('sp_TimKiemPhieuNhap', 'P') IS NOT NULL
    DROP PROCEDURE sp_TimKiemPhieuNhap;
GO

CREATE PROCEDURE sp_TimKiemPhieuNhap
    @TuNgay DATE = NULL,
    @DenNgay DATE = NULL,
    @LoaiNhap NVARCHAR(20) = NULL,
    @NCCID INT = NULL,
    @KhachHangID INT = NULL
AS
BEGIN
    SELECT 
        pn.PhieuNhapID AS MaPhieu,
        pn.NgayNhap,
        pn.LoaiNhap,
        ISNULL(SUM(ctn.DonGia), 0) AS TongTien
    FROM PhieuNhap pn
    LEFT JOIN ChiTietNhap ctn ON pn.PhieuNhapID = ctn.PhieuNhapID
    WHERE 
        (@TuNgay IS NULL OR pn.NgayNhap >= @TuNgay) AND
        (@DenNgay IS NULL OR pn.NgayNhap <= @DenNgay) AND
        (@LoaiNhap IS NULL OR @LoaiNhap = 'All' OR pn.LoaiNhap = @LoaiNhap) AND
        (@NCCID IS NULL OR pn.NCCID = @NCCID) AND
        (@KhachHangID IS NULL OR pn.KhachHangID = @KhachHangID)
    GROUP BY pn.PhieuNhapID, pn.NgayNhap, pn.LoaiNhap
    ORDER BY pn.NgayNhap DESC;
END;
GO

IF OBJECT_ID('sp_GetDanhMucByLoai', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetDanhMucByLoai;
GO

CREATE PROCEDURE sp_GetDanhMucByLoai
    @LoaiDanhMuc NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT DanhMucID, LoaiDanhMuc, GiaTri
    FROM DanhMuc
    WHERE LoaiDanhMuc = @LoaiDanhMuc
    ORDER BY GiaTri;
END;
GO

IF OBJECT_ID('sp_GetAllNhaCungCap', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetAllNhaCungCap;
GO

CREATE PROCEDURE sp_GetAllNhaCungCap
AS
BEGIN
    SELECT * FROM NhaCungCap;
END;
GO

IF OBJECT_ID('sp_GetKhachHang', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetKhachHang;
GO

CREATE PROCEDURE sp_GetKhachHang
AS
BEGIN
    SELECT KhachHangID, HoTen, DiaChi, SoDienThoai
    FROM KhachHang;
END;
GO

IF OBJECT_ID('sp_GetThongTinBySoDT', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetThongTinBySoDT;
GO

CREATE PROCEDURE sp_GetThongTinBySoDT
    @SoDT VARCHAR(15),
    @LoaiNhap NVARCHAR(20)
AS
BEGIN
    IF @LoaiNhap = N'NCC'
    BEGIN
        SELECT NCCID AS ID, TenNCC AS Ten, SoDienThoai, DiaChi
        FROM NhaCungCap
        WHERE SoDienThoai = @SoDT;
    END
    ELSE IF @LoaiNhap = N'KhachHang'
    BEGIN
        SELECT KhachHangID AS ID, HoTen AS Ten, SoDienThoai, DiaChi
        FROM KhachHang
        WHERE SoDienThoai = @SoDT;
    END
END;
GO

IF OBJECT_ID('sp_GetDataForBarChart', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetDataForBarChart;
GO

CREATE PROCEDURE sp_GetDataForBarChart
    @Criteria NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Criteria = N'Hãng xe'
    BEGIN
        SELECT HangXe AS Category, COUNT(*) AS SoLuong
        FROM XeChiTiet xct
        INNER JOIN Xe x ON xct.XeID = x.XeID
        WHERE xct.TrangThai = 'TonKho'
        GROUP BY HangXe;
    END

    ELSE IF @Criteria = N'Loại xe'
    BEGIN
        SELECT LoaiXe AS Category, COUNT(*) AS SoLuong
        FROM XeChiTiet xct
        INNER JOIN Xe x ON xct.XeID = x.XeID
        WHERE xct.TrangThai = 'TonKho'
        GROUP BY LoaiXe;
    END

    ELSE IF @Criteria = N'Màu sắc'
    BEGIN
        SELECT MauSac AS Category, COUNT(*) AS SoLuong
        FROM XeChiTiet xct
        INNER JOIN Xe x ON xct.XeID = x.XeID
        WHERE xct.TrangThai = 'TonKho'
        GROUP BY MauSac;
    END

    ELSE IF @Criteria = N'Năm sản xuất'
    BEGIN
        SELECT NamSX AS Category, COUNT(*) AS SoLuong
        FROM XeChiTiet xct
        INNER JOIN Xe x ON xct.XeID = x.XeID
        WHERE xct.TrangThai = 'TonKho'
        GROUP BY NamSX;
    END
END
GO


--5. -Tính toán, kiểm tra-

IF OBJECT_ID('sp_ThongKeNhap_Filter', 'P') IS NOT NULL
    DROP PROCEDURE sp_ThongKeNhap_Filter;
GO

CREATE PROCEDURE sp_ThongKeNhap_Filter
    @TuNgay DATE = NULL,
    @DenNgay DATE = NULL,
    @LoaiNhap NVARCHAR(20) = NULL,
    @NCCID INT = NULL,
    @KhachHangID INT = NULL
AS
BEGIN
    SELECT 
        COUNT(DISTINCT pn.PhieuNhapID) AS TongSoPhieu,
        COUNT(ctn.ChiTietNhapID) AS TongSoLuongXeNhap,
        ISNULL(SUM(ctn.DonGia), 0) AS TongGiaTriNhap
    FROM PhieuNhap pn
    LEFT JOIN ChiTietNhap ctn ON pn.PhieuNhapID = ctn.PhieuNhapID
    WHERE 
        (@TuNgay IS NULL OR pn.NgayNhap >= @TuNgay) AND
        (@DenNgay IS NULL OR pn.NgayNhap <= @DenNgay) AND
        (@LoaiNhap IS NULL OR @LoaiNhap = 'All' OR pn.LoaiNhap = @LoaiNhap) AND
        (@NCCID IS NULL OR pn.NCCID = @NCCID) AND
        (@KhachHangID IS NULL OR pn.KhachHangID = @KhachHangID);
END;
GO

IF OBJECT_ID('sp_CheckXeExists', 'P') IS NOT NULL
    DROP PROCEDURE sp_CheckXeExists;
GO

CREATE PROCEDURE sp_CheckXeExists
    @SoKhung NVARCHAR(50),
    @SoMay NVARCHAR(50)
AS
BEGIN
    SELECT 
        CASE 
            WHEN EXISTS (SELECT 1 FROM XeChiTiet WHERE SoKhung = @SoKhung) THEN 1
            WHEN EXISTS (SELECT 1 FROM XeChiTiet WHERE SoMay = @SoMay) THEN 2
            ELSE 0
        END AS KetQua;
END;
GO





CREATE FUNCTION fn_DSKH()
RETURNS TABLE
AS
RETURN (
	SELECT kh.KhachHangID, kh.HoTen, kh.SoDienThoai, kh.DiaChi, kh.NgayTao, pl.TenLoaiKH
	FROM KhachHang kh
	INNER JOIN PhanLoaiKH pl ON kh.MaLoaiKH = pl.MaLoaiKH
);
GO


CREATE PROCEDURE sp_InsertKhachHang
    @HoTen NVARCHAR(100),
	@TenLoaiKH NVARCHAR(20),
    @DiaChi NVARCHAR(200),
    @SoDienThoai VARCHAR(15)
AS
BEGIN
    SET NOCOUNT OFF;

    DECLARE @MaLoaiKH INT;

    SELECT @MaLoaiKH = MaLoaiKH
    FROM PhanLoaiKH
    WHERE TenLoaiKH = @TenLoaiKH;

    INSERT INTO KhachHang (HoTen, SoDienThoai, DiaChi, MaLoaiKH, NgayTao)
    VALUES (@HoTen, @SoDienThoai, @DiaChi, @MaLoaiKH, GETDATE());
	
END;
GO

CREATE PROCEDURE sp_UpdateKhachHang
	@MaKH NVARCHAR(20),
	@HoTen NVARCHAR(100),
	@TenLoaiKH NVARCHAR(20),
    @DiaChi NVARCHAR(200),
    @SoDienThoai VARCHAR(15)
AS
BEGIN
    DECLARE @MaLoaiKH INT;
    SELECT @MaLoaiKH = MaLoaiKH FROM PhanLoaiKH WHERE TenLoaiKH = @TenLoaiKH;
     
    UPDATE KhachHang
    SET HoTen = @HoTen,
        MaLoaiKH = @MaLoaiKH,
		SoDienThoai = @SoDienThoai,
		DiaChi = @DiaChi
    WHERE KhachHangID = @MaKH;
END;
GO


CREATE PROCEDURE sp_DeleteKhachHang
    @MaKH NVARCHAR(20)
AS
BEGIN
    DELETE FROM KhachHang
    WHERE KhachHangID = @MaKH;
END;
GO

CREATE FUNCTION fn_FindKhachHangByHoTen(@HoTen NVARCHAR(100))
RETURNS TABLE
AS
RETURN
(
    SELECT k.KhachHangID, k.HoTen, k.SoDienThoai, k.DiaChi,  k.NgayTao, p.TenLoaiKH
    FROM KhachHang k 
	JOIN PhanLoaiKH p ON k.MaLoaiKH = p.MaLoaiKH
    WHERE k.HoTen LIKE '%' + @HoTen + '%'
);
GO

CREATE FUNCTION fn_FindKhachHangBySDT(@SDT NVARCHAR(20))
RETURNS TABLE
AS
RETURN
(
    SELECT k.KhachHangID, k.HoTen, k.SoDienThoai, k.DiaChi,  k.NgayTao, p.TenLoaiKH
    FROM KhachHang k 
	JOIN PhanLoaiKH p ON k.MaLoaiKH = p.MaLoaiKH
    WHERE REPLACE(k.SoDienThoai, ' ', '') LIKE '%' + REPLACE(@SDT, ' ', '') + '%'
);
GO

CREATE FUNCTION fn_FindKhachHangByLoaiKH(@TenLoaiKH NVARCHAR(20))
RETURNS TABLE
AS
RETURN
(
    SELECT k.KhachHangID, k.HoTen, k.SoDienThoai, k.DiaChi,  k.NgayTao, p.TenLoaiKH
    FROM KhachHang k 
	JOIN PhanLoaiKH p ON k.MaLoaiKH = p.MaLoaiKH
    WHERE p.TenLoaiKH = @TenLoaiKH
);
GO


CREATE FUNCTION fn_XeCuaKhach(@KhachHangID INT)
RETURNS TABLE
AS
RETURN
(
    SELECT dh.KhachHangID,
           ctdh.XeCTID,
           xct.SoKhung
    FROM DonHang AS dh
    INNER JOIN ChiTietDonHang AS ctdh ON dh.MaDonHang = ctdh.MaDonHang
    INNER JOIN XeChiTiet AS xct ON ctdh.XeCTID = xct.XeCTID
    WHERE dh.KhachHangID = @KhachHangID AND xct.TrangThai = 'Daban'
);
GO


CREATE PROCEDURE sp_DatLichHen
    @XeCTID INT,
    @NgayHen DATE,
    @CaLamViec NVARCHAR(20),
    @LyDoHen NVARCHAR(500) = NULL,
    @GhiChu NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        
        IF NOT EXISTS (SELECT 1 FROM XeChiTiet WHERE XeCTID = @XeCTID)
        BEGIN
            THROW 50003, N'Xe không tồn tại!', 1;
        END;

		IF NOT EXISTS (SELECT 1 FROM XeChiTiet WHERE XeCTID = @XeCTID AND TrangThai = 'DaBan')
		BEGIN
			THROW 50005, N'Xe chưa được bán, không thể đặt lịch hẹn!', 1;
		END;

        IF @NgayHen < CAST(GETDATE() AS DATE)
        BEGIN
            THROW 50004, N'Ngày hẹn không thể là ngày trong quá khứ!', 1;
        END;

        BEGIN TRANSACTION; 

        DECLARE @SoLichHen INT;
        SELECT @SoLichHen = COUNT(*)
        FROM LichHenBaoTri
        WHERE NgayHen = @NgayHen
          AND CaLamViec = @CaLamViec
          AND TrangThaiHen NOT IN (N'Hủy', N'Hoàn thành');

        IF @SoLichHen >= 10
        BEGIN
            THROW 50005, N'Ca làm việc này đã đầy! Vui lòng chọn ca khác.', 1;
        END;


        INSERT INTO dbo.LichHenBaoTri (
            XeCTID, NgayHen, CaLamViec,
            LyDoHen, GhiChu, TrangThaiHen, NgayTao
        )
        VALUES (
            @XeCTID, @NgayHen, @CaLamViec,
            @LyDoHen, @GhiChu, N'Đã xác nhận', GETDATE()
        );

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO



CREATE FUNCTION fn_LichHenBaoTri_KhachHang()
RETURNS @KetQua TABLE
(
    MaLichHen INT,
    HoTen NVARCHAR(100),
    SDT VARCHAR(15),
    NgayHen DATE,
    CaLamViec NVARCHAR(50)
)
AS
BEGIN
    INSERT INTO @KetQua
    SELECT
        lhbt.MaLichHen,
        kh.HoTen,
        kh.SoDienThoai,
        lhbt.NgayHen,
        lhbt.CaLamViec
    FROM LichHenBaoTri AS lhbt
        INNER JOIN XeChiTiet AS x ON lhbt.XeCTID = x.XeCTID
        INNER JOIN ChiTietNhap AS ctn ON ctn.XeCTID = x.XeCTID
        INNER JOIN PhieuNhap AS pn ON pn.PhieuNhapID = ctn.PhieuNhapID
        INNER JOIN KhachHang AS kh ON kh.KhachHangID = pn.KhachHangID;

    RETURN;
END;
GO

GO

 

CREATE FUNCTION fn_GetLichHen(@MaLichHen INT)
RETURNS TABLE
AS
RETURN(
	SELECT XeCTID, NgayHen, NgayTao, CaLamViec, LyDoHen, GhiChu, TrangThaiHen 
	FROM LichHenBaoTri 
	WHERE MaLichHen = @MaLichHen
);
GO


CREATE FUNCTION fn_FindLichHen_NgayHen(@NgayHen DATE)
RETURNS TABLE
AS 
RETURN(
	SELECT * FROM dbo.fn_LichHenBaoTri_KhachHang()
	WHERE NgayHen = @NgayHen
);
GO


 CREATE PROCEDURE sp_UpdateLichHen
    @MaLichHen INT,                 -- Khóa chính của lịch hẹn
    @NgayHen DATE,
    @CaLamViec NVARCHAR(20),
    @LyDoHen NVARCHAR(500) = NULL,
    @GhiChu NVARCHAR(500) = NULL,
    @TrangThaiHen NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Kiểm tra lịch hẹn có tồn tại không
        IF NOT EXISTS (SELECT 1 FROM dbo.LichHenBaoTri WHERE MaLichHen = @MaLichHen)
        BEGIN
            THROW 50010, N'Lịch hẹn không tồn tại!', 1;
        END;

        -- Kiểm tra ngày hẹn
        IF @NgayHen < CAST(GETDATE() AS DATE)
        BEGIN
            THROW 50011, N'Ngày hẹn không thể là ngày trong quá khứ!', 1;
        END;

        BEGIN TRANSACTION;

        -- Kiểm tra số lượng lịch hẹn trong ca (không tính chính nó)
        DECLARE @SoLichHen INT;
        SELECT @SoLichHen = COUNT(*)
        FROM dbo.LichHenBaoTri
        WHERE NgayHen = @NgayHen
          AND CaLamViec = @CaLamViec
          AND TrangThaiHen NOT IN (N'Hủy', N'Hoàn thành')
          AND MaLichHen <> @MaLichHen;

        IF @SoLichHen >= 10
        BEGIN
            THROW 50012, N'Ca làm việc này đã đầy! Vui lòng chọn ca khác.', 1;
        END;

        -- Cập nhật lịch hẹn
        UPDATE dbo.LichHenBaoTri
        SET NgayHen = @NgayHen,
            CaLamViec = @CaLamViec,
            LyDoHen = @LyDoHen,
            GhiChu = @GhiChu,
            TrangThaiHen = @TrangThaiHen
        WHERE MaLichHen = @MaLichHen;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO


CREATE PROCEDURE sp_HuyLichHen
	@MaLichHen INT
AS
BEGIN
	UPDATE LichHenBaoTri
	SET TrangThaiHen = N'Hủy'
	WHERE MaLichHen = @MaLichHen 
END;
go



CREATE FUNCTION LoadXe()
RETURNS TABLE
AS
RETURN 
(
    SELECT 
        xe.XeID,
        xe.TenXe,
        xe.GiaBan,
		xe.MoTa,
        xe.HinhAnh,
        SUM(CASE 
                WHEN chitiet.trangthai = N'TonKho' 
                THEN 1 
                ELSE 0 
            END) AS soluong  
    FROM 
        Xe xe  
    LEFT JOIN 
        XeChiTiet chitiet ON xe.XeID = chitiet.XeCTID
    GROUP BY 
         xe.XeID, xe.TenXe, xe.GiaBan,xe.MoTa, xe.HinhAnh
);
GO


create FUNCTION BaoCaoDoanhThu()
RETURNS @DoanhThu TABLE
(
    Nam INT,
    Thang INT,
    TongDoanhThu DECIMAL(18, 0)
)
AS
BEGIN
    DECLARE @NamKetThuc INT = YEAR(GETDATE());
    DECLARE @NamBatDau INT = @NamKetThuc - 1;

    -- 1. Tạo bảng 24 cặp (Nam, Thang)
    WITH Months AS (
        SELECT 1 AS Thang UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL 
        SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL
        SELECT 7 UNION ALL SELECT 8 UNION ALL SELECT 9 UNION ALL
        SELECT 10 UNION ALL SELECT 11 UNION ALL SELECT 12
    ),
    Years AS (
        SELECT @NamBatDau AS Nam UNION ALL SELECT @NamKetThuc AS Nam
    ),
    AllPeriods AS (
        SELECT Y.Nam, M.Thang
        FROM Years Y CROSS JOIN Months M
    ),
    -- 2. Tính tổng doanh thu thực tế
    ActualSales AS (
        SELECT
            YEAR(DH.NgayGiaoDich) AS Nam,
            MONTH(DH.NgayGiaoDich) AS Thang,
            SUM(DH.TongTien) AS TongTien
        FROM
            DonHang DH
        WHERE
            YEAR(DH.NgayGiaoDich) BETWEEN @NamBatDau AND @NamKetThuc
			AND DH.TrangThai = N'Hoàn thành'
        GROUP BY
            YEAR(DH.NgayGiaoDich), MONTH(DH.NgayGiaoDich)
    )
    
    -- 3. LEFT JOIN để gộp 24 kỳ với dữ liệu bán hàng (tháng không có đơn sẽ có TongTien = 0)
    INSERT INTO @DoanhThu (Nam, Thang, TongDoanhThu)
    SELECT
        AP.Nam,
        AP.Thang,
        ISNULL(ASL.TongTien, 0) AS TongDoanhThu
    FROM
        AllPeriods AP
    LEFT JOIN
        ActualSales ASL ON AP.Nam = ASL.Nam AND AP.Thang = ASL.Thang
    ORDER BY
        AP.Nam, AP.Thang;

    RETURN
END
GO

CREATE PROCEDURE sp_BaoCaoSoDonHangTheoThang (
    @Nam INT
)
AS
BEGIN
    SET NOCOUNT ON;

    WITH Months AS (
        SELECT 1 AS Thang
        UNION ALL SELECT 2
        UNION ALL SELECT 3
        UNION ALL SELECT 4
        UNION ALL SELECT 5
        UNION ALL SELECT 6
        UNION ALL SELECT 7
        UNION ALL SELECT 8
        UNION ALL SELECT 9
        UNION ALL SELECT 10
        UNION ALL SELECT 11
        UNION ALL SELECT 12
    )
    SELECT
        m.Thang,
        -- Chỉ lấy số lượng đơn hàng, thay thế NULL bằng 0
        ISNULL(COUNT(dh.MaDonHang), 0) AS SoLuongDonHang
    FROM Months m
    LEFT JOIN DonHang dh ON m.Thang = MONTH(dh.NgayGiaoDich)
                         AND YEAR(dh.NgayGiaoDich) = @Nam
    GROUP BY m.Thang
    ORDER BY m.Thang;

END
GO

CREATE FUNCTION dbo.fn_DemTongSoDonHangTheoNam (
    @Nam INT
)
RETURNS INT
AS
BEGIN
    DECLARE @TongSoDon INT;

    SELECT @TongSoDon = COUNT(MaDonHang)
    FROM DonHang
    WHERE YEAR(NgayGiaoDich) = @Nam;

    RETURN @TongSoDon;
END
GO

Create FUNCTION ThongKeBanChay
(
    @NgayBatDau DATE,
    @NgayKetThuc DATE,
    @LoaiXe NVARCHAR(20) = NULL   -- Thêm tham số lọc loại xe
)
RETURNS @KetQua TABLE
(
    STT INT,
    TenSanPham NVARCHAR(200),
    TongSoLuongBan INT
)
AS
BEGIN
    -- Bảng tạm chứa dữ liệu tổng hợp
    DECLARE @DuLieuTam TABLE
    (
        TenSanPham NVARCHAR(200),
        TongSoLuongBan INT
    );

    -- Tổng hợp Top 20 sản phẩm bán chạy theo loại xe (nếu có)
    INSERT INTO @DuLieuTam (TenSanPham, TongSoLuongBan)
    SELECT TOP 20
        CONCAT(X.HangXe, N' ', X.TenXe, N' (', X.MauSac, N')') AS TenSanPham,
        SUM(CTDH.SoLuong) AS TongSoLuongBan
    FROM DonHang DH
    INNER JOIN ChiTietDonHang CTDH ON DH.MaDonHang = CTDH.MaDonHang
    INNER JOIN XeChiTiet XCT ON CTDH.XeCTID = XCT.XeCTID
    INNER JOIN Xe X ON XCT.XeID = X.XeID
    WHERE DH.NgayGiaoDich BETWEEN @NgayBatDau AND @NgayKetThuc
          AND (@LoaiXe IS NULL OR X.LoaiXe = @LoaiXe)  -- nếu @LoaiXe NULL → lấy tất cả
    GROUP BY X.HangXe, X.TenXe, X.MauSac
    ORDER BY SUM(CTDH.SoLuong) DESC;

    -- Thêm cột STT vào kết quả trả về
    INSERT INTO @KetQua (STT, TenSanPham, TongSoLuongBan)
    SELECT 
        ROW_NUMBER() OVER (ORDER BY TongSoLuongBan DESC) AS STT,
        TenSanPham,
        TongSoLuongBan
    FROM @DuLieuTam;

    RETURN;
END;
GO

CREATE FUNCTION ThongKeBanCham
(
    @LoaiXe NVARCHAR(20) = NULL  -- TayGa / XeSo / ConTay / Dien (chỉ áp dụng cho Xe máy)
)
RETURNS @KetQua TABLE
(
    STT INT,
    TenSanPham NVARCHAR(200),
    TongSoLuongBan INT
)
AS
BEGIN
    -- Thời gian: 1 tháng gần nhất
    DECLARE @NgayKetThuc DATE = GETDATE();
    DECLARE @NgayBatDau DATE = DATEADD(MONTH, -1, GETDATE());

    DECLARE @DuLieuTam TABLE (
        TenMatHang NVARCHAR(200),
        TongSoLuong INT
    );

    -- 1️⃣ Xe máy (lọc thêm theo loại xe)
    BEGIN
        WITH AllXe AS (
            SELECT DISTINCT 
                CONCAT(X.HangXe, N' ', X.TenXe, N' (', X.MauSac, N')') AS ConfigName
            FROM Xe X
            WHERE (@LoaiXe IS NULL OR X.LoaiXe = @LoaiXe)
        ),
        SalesData AS (
            SELECT 
                CONCAT(X.HangXe, N' ', X.TenXe, N' (', X.MauSac, N')') AS ConfigName,
                SUM(CTDH.SoLuong) AS TongSoLuong
            FROM DonHang DH
            INNER JOIN ChiTietDonHang CTDH ON DH.MaDonHang = CTDH.MaDonHang
            INNER JOIN XeChiTiet XCT ON CTDH.XeCTID = XCT.XeCTID
            INNER JOIN Xe X ON XCT.XeID = X.XeID
            WHERE DH.NgayGiaoDich BETWEEN @NgayBatDau AND @NgayKetThuc
              AND (@LoaiXe IS NULL OR X.LoaiXe = @LoaiXe)
            GROUP BY X.HangXe, X.TenXe, X.MauSac
        )
        INSERT INTO @DuLieuTam (TenMatHang, TongSoLuong)
        SELECT 
            A.ConfigName,
            ISNULL(S.TongSoLuong, 0)
        FROM AllXe A
        LEFT JOIN SalesData S ON A.ConfigName = S.ConfigName;
    END

   

    -- 3️⃣ Trả về TOP 20 sản phẩm bán chậm nhất
    INSERT INTO @KetQua (STT, TenSanPham, TongSoLuongBan)
    SELECT TOP 20
        ROW_NUMBER() OVER (ORDER BY TongSoLuong ASC) AS STT,
        TenMatHang,
        TongSoLuong
    FROM @DuLieuTam
    ORDER BY TongSoLuong ASC;

    RETURN;
END;
GO

 
create PROCEDURE SP_UuDai_GetAllActive
AS
BEGIN
    SET NOCOUNT ON;

    -- Lấy ngày hiện tại mà không có giờ, chỉ tính 1 lần cho hiệu quả
    DECLARE @CurrentDate DATE = GETDATE();

    -- Trả về cả Mã và Tên để dễ hiển thị trên giao diện
    SELECT
        MaUuDai,
        TenUuDai
    FROM
        UuDai
    WHERE
        -- Sử dụng BETWEEN để kiểm tra xem ngày hiện tại có nằm trong khoảng hiệu lực hay không
        @CurrentDate BETWEEN NgayBatDau AND NgayKetThuc
    ORDER BY
        MaUuDai;
END
GO

CREATE TYPE ChiTietDonHangType AS TABLE
(
    SoKhung VARCHAR(50) NOT NULL,
    SoMay VARCHAR(50) NOT NULL,
    SoLuong INT NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL
);
GO
Create PROCEDURE InsertChiTietDonHang
    @MaDonHang INT,
    @DanhSachXe AS ChiTietDonHangType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    -- Biến bảng tạm để lưu trữ thông tin đã được truy vấn XeCTID
    DECLARE @ChiTietDaXuLy TABLE (
        XeCTID INT NOT NULL,
        SoLuong INT NOT NULL,
        DonGia DECIMAL(18,2) NOT NULL
    );

    -- Truy vấn để lấy XeCTID từ SoKhung/SoMay và kiểm tra xe có tồn tại và sẵn sàng để bán không
    INSERT INTO @ChiTietDaXuLy (XeCTID, SoLuong, DonGia)
    SELECT
        xct.XeCTID,
        dsx.SoLuong,
        dsx.DonGia
    FROM @DanhSachXe AS dsx
    JOIN XeChiTiet AS xct ON dsx.SoKhung = xct.SoKhung AND dsx.SoMay = xct.SoMay
    WHERE xct.TrangThai = N'ChuaBan'; -- Chỉ thêm những xe có trạng thái là 'Chưa bán'

    -- Kiểm tra xem có xe nào trong danh sách không hợp lệ không (không tìm thấy hoặc đã bán)
    IF (SELECT COUNT(*) FROM @DanhSachXe) <> (SELECT COUNT(*) FROM @ChiTietDaXuLy)
    BEGIN
        -- Ném lỗi và thông báo xe nào không hợp lệ
        RAISERROR(N'Một hoặc nhiều xe trong danh sách không tồn tại, đã bán, hoặc có số khung/số máy sai. Giao dịch đã bị hủy.', 16, 1);
        RETURN;
    END

    -- Các bước tiếp theo thực hiện trên biến bảng @ChiTietDaXuLy thay vì @DanhSachXe
    DECLARE @TongTienThem DECIMAL(18,2);
    DECLARE @TongSoLuongThem INT;

    SELECT
        @TongTienThem = SUM(SoLuong * DonGia),
        @TongSoLuongThem = SUM(SoLuong)
    FROM @ChiTietDaXuLy;

    -- 1. Insert vào ChiTietDonHang
    INSERT INTO ChiTietDonHang (MaDonHang, XeCTID, SoLuong, DonGia)
    SELECT @MaDonHang, XeCTID, SoLuong, DonGia
    FROM @ChiTietDaXuLy;

    -- 2. Cập nhật DonHang
    IF @TongTienThem IS NOT NULL
    BEGIN
        UPDATE DonHang
        SET TongTien = ISNULL(TongTien, 0) + @TongTienThem,
            SoLuong = ISNULL(SoLuong, 0) + @TongSoLuongThem
        WHERE MaDonHang = @MaDonHang;
    END

    -- 3. Cập nhật trạng thái xe
    UPDATE XeChiTiet
    SET TrangThai = N'DaBan'
    WHERE XeCTID IN (SELECT XeCTID FROM @ChiTietDaXuLy);

    -- Trả về tổng tiền
    SELECT TongTien
    FROM DonHang
    WHERE MaDonHang = @MaDonHang;
END;
GO

-- Nên chạy lệnh DROP trước nếu procedure đã tồn tại
-- DROP PROCEDURE TaoDonHangVaChiTiet;
-- GO
create PROCEDURE TaoDonHangVaChiTiet
    @KhachHangID INT,
    @DanhSachXe AS ChiTietDonHangType READONLY,
    @MaNV INT = NULL,
    @MaUuDai VARCHAR(20) = NULL,
    @NgayGiaoDich DATE = NULL,
    @GhiChu NVARCHAR(255) = NULL,
    @PhuongThucThanhToan NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaDonHangMoi INT;
    DECLARE @TongTienTruocGiam DECIMAL(18,2);
    DECLARE @TongTienSauGiam DECIMAL(18,2);
    DECLARE @SoTienGiam DECIMAL(18,2) = 0;
    DECLARE @TongSoLuong INT;
    DECLARE @LoaiUuDai VARCHAR(20);
    DECLARE @GiaTriGiam DECIMAL(18,2);
    DECLARE @ChiTietDaXuLy TABLE (
        XeCTID INT NOT NULL, SoLuong INT NOT NULL, DonGia DECIMAL(18,2) NOT NULL
    );

    BEGIN TRANSACTION;
    BEGIN TRY
        -- 1. XÁC THỰC XE VÀ TÍNH TỔNG TIỀN GỐC
        INSERT INTO @ChiTietDaXuLy (XeCTID, SoLuong, DonGia)
        SELECT xct.XeCTID, dsx.SoLuong, dsx.DonGia
        FROM @DanhSachXe AS dsx
        JOIN XeChiTiet AS xct ON dsx.SoKhung = xct.SoKhung AND dsx.SoMay = xct.SoMay
        WHERE xct.TrangThai = N'TonKho'; -- Giả sử trạng thái là 'TonKho'

        IF (SELECT COUNT(*) FROM @DanhSachXe) <> (SELECT COUNT(*) FROM @ChiTietDaXuLy) BEGIN
            RAISERROR(N'Một hoặc nhiều xe trong danh sách không tồn tại hoặc đã được bán.', 16, 1); RETURN; END
        IF NOT EXISTS (SELECT 1 FROM @ChiTietDaXuLy) BEGIN
            RAISERROR(N'Đơn hàng không có chi tiết nào.', 16, 1); RETURN; END

        SELECT @TongTienTruocGiam = SUM(SoLuong * DonGia), @TongSoLuong = SUM(SoLuong)
        FROM @ChiTietDaXuLy;

        -- 2. KIỂM TRA VÀ TÍNH TOÁN ƯU ĐÃI
        IF @MaUuDai IS NOT NULL AND LTRIM(RTRIM(@MaUuDai)) <> ''
        BEGIN
            SELECT @LoaiUuDai = ud.LoaiUuDai, @GiaTriGiam = ud.GiaTriGiam
            FROM UuDai ud
            WHERE ud.MaUuDai = @MaUuDai
             AND CAST(GETDATE() AS DATE) BETWEEN ud.NgayBatDau AND ud.NgayKetThuc
              AND @TongTienTruocGiam >= ud.DieuKienToiThieu;

            IF @LoaiUuDai IS NOT NULL
            BEGIN
                IF @LoaiUuDai = 'PHANTRAM'
                    SET @SoTienGiam = (@TongTienTruocGiam * @GiaTriGiam) / 100;
                ELSE IF @LoaiUuDai = 'TRUCTIEP'
                    SET @SoTienGiam = @GiaTriGiam;
            END
            ELSE
            BEGIN
                RAISERROR(N'Mã ưu đãi không hợp lệ, đã hết hạn, hoặc đơn hàng không đủ điều kiện.', 16, 1); RETURN;
            END
        END

        -- 3. TÍNH TOÁN VÀ TẠO ĐƠN HÀNG
        SET @TongTienSauGiam = @TongTienTruocGiam - @SoTienGiam;
        IF @TongTienSauGiam < 0 SET @TongTienSauGiam = 0;
        IF @NgayGiaoDich IS NULL SET @NgayGiaoDich = GETDATE();

        -- DÒNG CODE ĐƯỢC CẬP NHẬT: Thêm cột và giá trị 'TrangThai'
        INSERT INTO DonHang (KhachHangID, MaNV, MaUuDai, NgayGiaoDich, GhiChu, TongTien, SoLuong, TrangThai)
        VALUES (@KhachHangID, @MaNV, @MaUuDai, @NgayGiaoDich, @GhiChu, @TongTienSauGiam, @TongSoLuong, N'Hoàn thành');
        
        SET @MaDonHangMoi = SCOPE_IDENTITY();

        -- 4. CÁC BƯỚC CÒN LẠI GIỮ NGUYÊN
        IF @TongTienSauGiam > 0
        BEGIN
            INSERT INTO ThanhToan (MaDonHang, SoTien, PhuongThuc)
            VALUES (@MaDonHangMoi, @TongTienSauGiam, @PhuongThucThanhToan);
        END

        INSERT INTO ChiTietDonHang (MaDonHang, XeCTID, SoLuong, DonGia)
        SELECT @MaDonHangMoi, XeCTID, SoLuong, DonGia FROM @ChiTietDaXuLy;

        UPDATE XeChiTiet SET TrangThai = N'DaBan'
        WHERE XeCTID IN (SELECT XeCTID FROM @ChiTietDaXuLy);

        COMMIT TRANSACTION;
        SELECT @MaDonHangMoi AS MaDonHangMoi;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
CREATE PROCEDURE TimKhachHangBangSDT
    @SoDienThoai VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Kiểm tra NULL hoặc Trống
    IF @SoDienThoai IS NULL OR LTRIM(RTRIM(@SoDienThoai)) = ''
    BEGIN
        RAISERROR(N'Số điện thoại tìm kiếm không được để trống.', 16, 1);
        RETURN;
    END

    -- Chuẩn hóa số điện thoại đầu vào để kiểm tra
    DECLARE @SDT_ChuanHoa VARCHAR(15);
    SET @SDT_ChuanHoa = LTRIM(RTRIM(@SoDienThoai));

    -- 2. Kiểm tra Độ Dài (Giả sử 10 ký tự như ràng buộc đã đặt)
    IF LEN(@SDT_ChuanHoa) <> 10
    BEGIN
        RAISERROR(N'Số điện thoại phải có đúng 10 ký tự.', 16, 1);
        RETURN;
    END

    -- 3. Kiểm tra Ký tự Số (Chỉ chấp nhận 0-9)
    -- PATINDEX trả về 0 nếu KHÔNG tìm thấy ký tự nào ngoài 0-9
    IF PATINDEX('%[^0-9]%', @SDT_ChuanHoa) <> 0
    BEGIN
        RAISERROR(N'Số điện thoại chỉ được chứa ký tự số (0-9).', 16, 1);
        RETURN;
    END

    -- Thực hiện tìm kiếm khách hàng nếu các điều kiện kiểm tra hợp lệ
    SELECT
        KhachHangID,
        HoTen,
        DiaChi,
        SoDienThoai,
        MaLoaiKH
    FROM
        KhachHang
    WHERE
        SoDienThoai = @SDT_ChuanHoa; -- Sử dụng giá trị đã chuẩn hóa để so sánh chính xác

    -- Kiểm tra nếu không tìm thấy khách hàng nào
    IF @@ROWCOUNT = 0
    BEGIN
        -- Trả về 0 để báo hiệu không tìm thấy
        RETURN 0;
    END
    
END;
GO

CREATE PROCEDURE sp_InsertKhachHangMoi
    @HoTen NVARCHAR(100),
	@TenLoaiKH NVARCHAR(20),
    @DiaChi NVARCHAR(200),
    @SoDienThoai VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON; -- Nên dùng SET NOCOUNT ON trong SP để tối ưu hiệu suất

    DECLARE @MaLoaiKH INT;
    DECLARE @MaKhachHangMoi INT; -- Biến để lưu ID mới

    -- 1. Lấy MaLoaiKH từ TenLoaiKH
    SELECT @MaLoaiKH = MaLoaiKH
    FROM PhanLoaiKH
    WHERE TenLoaiKH = @TenLoaiKH;
    
    -- Kiểm tra nếu không tìm thấy loại khách hàng
    IF @MaLoaiKH IS NULL
    BEGIN
        -- Bạn có thể ném lỗi hoặc đặt MaLoaiKH về NULL (tùy thuộc vào logic kinh doanh)
        -- Trong ví dụ này, tôi sẽ ném lỗi:
        RAISERROR(N'Không tìm thấy Mã Loại Khách Hàng tương ứng với Tên Loại Khách Hàng đã cung cấp.', 16, 1);
        RETURN -1; -- Trả về giá trị lỗi
    END

    -- 2. Thêm vào KhachHang
    INSERT INTO KhachHang (HoTen, SoDienThoai, DiaChi, MaLoaiKH, NgayTao)
    VALUES (@HoTen, @SoDienThoai, @DiaChi, @MaLoaiKH, GETDATE());

    -- 3. Lấy MaKhachHang mới tạo
    SET @MaKhachHangMoi = SCOPE_IDENTITY();
    
    -- 4. Trả về MaKhachHang mới tạo
    SELECT @MaKhachHangMoi AS MaKhachHangMoi;
END;
GO

create PROCEDURE SP_UuDai_Insert
    @MaUuDai VARCHAR(20),
    @TenUuDai NVARCHAR(100),
    @LoaiUuDai VARCHAR(20),
    @GiaTriGiam DECIMAL(18, 2),
    -- THÊM MỚI: Tham số cho cột mới, mặc định là 0
    @DieuKienToiThieu DECIMAL(18,2) = 0,
    @NgayBatDau DATE,
    @NgayKetThuc DATE,
    @MoTa NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- KIỂM TRA LOGIC (giữ nguyên)
    IF @NgayKetThuc < @NgayBatDau BEGIN
        RAISERROR (N'Ngày kết thúc phải lớn hơn hoặc bằng Ngày bắt đầu.', 16, 1); RETURN; END
    IF @GiaTriGiam < 0 BEGIN
        RAISERROR (N'Giá trị giảm không được là số âm.', 16, 1); RETURN; END
    IF UPPER(@LoaiUuDai) = 'PHANTRAM' AND @GiaTriGiam > 100 BEGIN
        RAISERROR (N'Giảm theo phần trăm không được vượt quá 100.', 16, 1); RETURN; END
    IF UPPER(@LoaiUuDai) NOT IN ('PHANTRAM', 'TRUCTIEP') BEGIN
        RAISERROR (N'Loại ưu đãi không hợp lệ.', 16, 1); RETURN; END
    -- THÊM MỚI: Kiểm tra cho điều kiện tối thiểu
    IF @DieuKienToiThieu < 0 BEGIN
        RAISERROR (N'Điều kiện tối thiểu không được là số âm.', 16, 1); RETURN; END

    -- CẬP NHẬT: Thêm cột mới vào câu lệnh INSERT
    INSERT INTO UuDai (MaUuDai, TenUuDai, LoaiUuDai, GiaTriGiam, DieuKienToiThieu, NgayBatDau, NgayKetThuc, MoTa)
    VALUES (@MaUuDai, @TenUuDai, @LoaiUuDai, @GiaTriGiam, @DieuKienToiThieu, @NgayBatDau, @NgayKetThuc, @MoTa);
END
GO


create PROCEDURE SP_UuDai_Update
    @MaUuDai VARCHAR(20),
    @TenUuDai NVARCHAR(100),
    @LoaiUuDai VARCHAR(20),
    @GiaTriGiam DECIMAL(18, 2),
    -- THÊM MỚI: Tham số cho cột mới
    @DieuKienToiThieu DECIMAL(18,2),
    @NgayBatDau DATE,
    @NgayKetThuc DATE,
    @MoTa NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- KIỂM TRA LOGIC (giữ nguyên và bổ sung)
    IF @NgayKetThuc < @NgayBatDau BEGIN
        RAISERROR (N'Ngày kết thúc phải lớn hơn hoặc bằng Ngày bắt đầu.', 16, 1); RETURN; END
    IF UPPER(@LoaiUuDai) = 'PHANTRAM' AND @GiaTriGiam > 100 BEGIN
        RAISERROR (N'Giảm theo phần trăm không được vượt quá 100.', 16, 1); RETURN; END
    -- THÊM MỚI: Kiểm tra cho điều kiện tối thiểu
    IF @DieuKienToiThieu < 0 BEGIN
        RAISERROR (N'Điều kiện tối thiểu không được là số âm.', 16, 1); RETURN; END

    -- CẬP NHẬT: Thêm cột mới vào câu lệnh UPDATE
    UPDATE UuDai
    SET
        TenUuDai = @TenUuDai,
        LoaiUuDai = UPPER(@LoaiUuDai),
        GiaTriGiam = @GiaTriGiam,
        DieuKienToiThieu = @DieuKienToiThieu, -- << Cập nhật ở đây
        NgayBatDau = @NgayBatDau,
        NgayKetThuc = @NgayKetThuc,
        MoTa = @MoTa
    WHERE MaUuDai = @MaUuDai;

    IF @@ROWCOUNT = 0 BEGIN
        RAISERROR(N'Không tìm thấy Mã Ưu Đãi để cập nhật.', 16, 1); RETURN; END
END
GO


create PROCEDURE SP_UuDai_SearchCombined
    @SearchTerm NVARCHAR(100),
    @Status VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CurrentDate DATE = GETDATE();

    SELECT
        MaUuDai,
        TenUuDai,
        LoaiUuDai,
        GiaTriGiam,
        DieuKienToiThieu, -- << THÊM MỚI: Thêm cột vào kết quả trả về
        NgayBatDau,
        NgayKetThuc,
        MoTa,
        CASE
            WHEN NgayKetThuc >= @CurrentDate THEN N'Kích hoạt'
            ELSE N'Ngừng kích hoạt'
        END AS TrangThaiHieuLuc
    FROM
        UuDai
    WHERE
        (
            @Status = 'TatCa'
            OR (@Status = 'KichHoat' AND NgayKetThuc >= @CurrentDate)
            OR (@Status = 'NgungKichHoat' AND NgayKetThuc < @CurrentDate)
        )
        AND
        (
            -- Điều kiện tìm kiếm theo Tên/Mã giữ nguyên
            @SearchTerm IS NULL OR @SearchTerm = ''
            OR MaUuDai LIKE N'%' + @SearchTerm + N'%'
            OR TenUuDai LIKE N'%' + @SearchTerm + N'%'
        );
END
GO


create PROCEDURE SP_UuDai_GetByMa
    @MaUuDai VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- CẬP NHẬT: Thêm cột mới vào kết quả trả về
    SELECT
        MaUuDai,
        TenUuDai,
        LoaiUuDai,
        GiaTriGiam,
        DieuKienToiThieu, -- << THÊM MỚI
        NgayBatDau,
        NgayKetThuc,
        MoTa
    FROM
        UuDai
    WHERE
        MaUuDai = @MaUuDai;
END
GO

ALTER TABLE Luong
ADD CONSTRAINT DF_Luong_NgayThanhToan DEFAULT GETDATE() FOR NgayThanhToan;
GO
ALTER TABLE Luong
ADD CONSTRAINT DF_Luong_TongGioThang DEFAULT 0 FOR TongGioThang;
GO

-- SP mới cho Luong
IF OBJECT_ID('sp_LuuBangLuong', 'P') IS NOT NULL
    DROP PROCEDURE sp_LuuBangLuong;
GO
CREATE PROCEDURE sp_LuuBangLuong
    @MaNV INT,
    @ThangLuong DATE,
    @LuongThucNhan DECIMAL(18,2),
    @KhoanThuong DECIMAL(18,2),
    @KhoanKhauTru DECIMAL(18,2),
    @TongGioThang DECIMAL(10,2) = 0,
    @GioLamThem DECIMAL(10,2) = 0
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Luong WHERE MaNV = @MaNV AND MONTH(ThangLuong) = MONTH(@ThangLuong) AND YEAR(ThangLuong) = YEAR(@ThangLuong))
    BEGIN
        UPDATE Luong
        SET LgThucNhan = @LuongThucNhan, TienThuong = @KhoanThuong, KhauTru = @KhoanKhauTru,
            TongGioThang = @TongGioThang, GioLamThem = @GioLamThem, NgayThanhToan = GETDATE()
        WHERE MaNV = @MaNV AND MONTH(ThangLuong) = MONTH(@ThangLuong) AND YEAR(ThangLuong) = YEAR(@ThangLuong);
    END
    ELSE
    BEGIN
        INSERT INTO Luong (MaNV, ThangLuong, LgThucNhan, TienThuong, KhauTru, TongGioThang, GioLamThem, NgayThanhToan)
        VALUES (@MaNV, @ThangLuong, @LuongThucNhan, @KhoanThuong, @KhoanKhauTru, @TongGioThang, @GioLamThem, GETDATE());
    END
END
GO

-- Functions đã sửa drop
IF OBJECT_ID('fn_GetNhanVienExperienceDistribution', 'TF') IS NOT NULL
    DROP FUNCTION fn_GetNhanVienExperienceDistribution;
GO
CREATE FUNCTION fn_GetNhanVienExperienceDistribution()
RETURNS TABLE
AS
RETURN
(
    SELECT
        CASE
            WHEN DATEDIFF(YEAR, NgayNhanViec, GETDATE()) < 1 THEN N'Dưới 1 năm'
            WHEN DATEDIFF(YEAR, NgayNhanViec, GETDATE()) BETWEEN 1 AND 3 THEN N'Từ 1 - 3 năm'
            WHEN DATEDIFF(YEAR, NgayNhanViec, GETDATE()) BETWEEN 3 AND 5 THEN N'Từ 3 - 5 năm'
            ELSE N'Trên 5 năm'
        END AS KinhNghiem,
        COUNT(MaNV) AS SoLuong
    FROM
        NhanVien
    GROUP BY
        CASE
            WHEN DATEDIFF(YEAR, NgayNhanViec, GETDATE()) < 1 THEN N'Dưới 1 năm'
            WHEN DATEDIFF(YEAR, NgayNhanViec, GETDATE()) BETWEEN 1 AND 3 THEN N'Từ 1 - 3 năm'
            WHEN DATEDIFF(YEAR, NgayNhanViec, GETDATE()) BETWEEN 3 AND 5 THEN N'Từ 3 - 5 năm'
            ELSE N'Trên 5 năm'
        END
);
GO

-- Stored Procedure: sp_InsertNhanVien
IF OBJECT_ID('sp_InsertNhanVien', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertNhanVien;
GO
CREATE PROCEDURE sp_InsertNhanVien
    @HoTenNV NVARCHAR(255),
    @NgaySinh DATE,
    @GioiTinh NVARCHAR(10),
    @SoDT VARCHAR(15),
    @Email VARCHAR(100),
    @DiaChi NVARCHAR(255),
    @CCCD VARCHAR(20),
    @NgayNhanViec DATE,
    @LuongCB DECIMAL(18,2),
    @HinhAnh NVARCHAR(255),
    @TenTK VARCHAR(50) = NULL,
    @MatKhau VARCHAR(255) = NULL,
    @VaiTro NVARCHAR(100) = NULL
AS
BEGIN
    DECLARE @MaNV INT;

    INSERT INTO NhanVien (HoTenNV, NgaySinh, GioiTinh, SoDT, Email, DiaChi, CCCD, NgayNhanViec, LuongCB, HinhAnh)
    VALUES (@HoTenNV, @NgaySinh, @GioiTinh, @SoDT, @Email, @DiaChi, @CCCD, @NgayNhanViec, @LuongCB, @HinhAnh);

    SET @MaNV = SCOPE_IDENTITY();

    IF @TenTK IS NOT NULL AND @MatKhau IS NOT NULL AND @VaiTro IS NOT NULL
    BEGIN
        INSERT INTO TaiKhoan (MaNV, TenTK, MatKhau, VaiTro)
        VALUES (@MaNV, @TenTK, @MatKhau, @VaiTro);
    END

    SELECT @MaNV AS MaNV;
END
GO

-- Stored Procedure: sp_UpdateNhanVien
IF OBJECT_ID('sp_UpdateNhanVien', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateNhanVien;
GO
CREATE PROCEDURE sp_UpdateNhanVien
    @MaNV INT,
    @HoTenNV NVARCHAR(255),
    @NgaySinh DATE,
    @GioiTinh NVARCHAR(10),
    @SoDT VARCHAR(15),
    @Email VARCHAR(100),
    @DiaChi NVARCHAR(255),
    @CCCD VARCHAR(20),
    @NgayNhanViec DATE,
    @LuongCB DECIMAL(18,2),
    @HinhAnh NVARCHAR(255)
AS
BEGIN
    UPDATE NhanVien
    SET HoTenNV = @HoTenNV,
        NgaySinh = @NgaySinh,
        GioiTinh = @GioiTinh,
        SoDT = @SoDT,
        Email = @Email,
        DiaChi = @DiaChi,
        CCCD = @CCCD,
        NgayNhanViec = @NgayNhanViec,
        LuongCB = @LuongCB,
        HinhAnh = @HinhAnh
    WHERE MaNV = @MaNV;
END
GO

-- Stored Procedure: sp_DeleteNhanVien
IF OBJECT_ID('sp_DeleteNhanVien', 'P') IS NOT NULL
    DROP PROCEDURE sp_DeleteNhanVien;
GO
CREATE PROCEDURE sp_DeleteNhanVien
    @MaNV INT
AS
BEGIN
    -- Xóa các bản ghi liên quan trước
    DELETE FROM TaiKhoan WHERE MaNV = @MaNV;
    DELETE FROM ChamCong WHERE MaNV = @MaNV;
    DELETE FROM Luong WHERE MaNV = @MaNV;
    DELETE FROM ThongBao WHERE MaNV = @MaNV;
    
    -- Cuối cùng, xóa nhân viên
    DELETE FROM NhanVien WHERE MaNV = @MaNV;
END
GO

-- Stored Procedure: sp_InsertChamCong
IF OBJECT_ID('sp_InsertChamCong', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertChamCong;
GO
CREATE PROCEDURE sp_InsertChamCong
    @MaNV INT,
    @NgayLamViec DATE,
    @ThoiGianVaoLam TIME,
    @ThoiGianTanCa TIME,
    @TrangThai NVARCHAR(50),
    @LyDoNghiPhep NVARCHAR(255) = NULL
AS
BEGIN
    INSERT INTO ChamCong (MaNV, NgayLamViec, TgVaoLam, TgTanCa, TrangThai, LyDoNghiPhep)
    VALUES (@MaNV, @NgayLamViec, @ThoiGianVaoLam, @ThoiGianTanCa, @TrangThai, @LyDoNghiPhep);
END
GO

-- Stored Procedure: sp_UpdateChamCong
IF OBJECT_ID('sp_UpdateChamCong', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateChamCong;
GO
CREATE PROCEDURE sp_UpdateChamCong
    @MaCC INT,
    @MaNV INT,
    @NgayLamViec DATE,
    @ThoiGianVaoLam TIME,
    @ThoiGianTanCa TIME,
    @TrangThai NVARCHAR(50),
    @LyDoNghiPhep NVARCHAR(255) = NULL
AS
BEGIN
    UPDATE ChamCong
    SET MaNV = @MaNV,
        NgayLamViec = @NgayLamViec,
        TgVaoLam = @ThoiGianVaoLam,
        TgTanCa = @ThoiGianTanCa,
        TrangThai = @TrangThai,
        LyDoNghiPhep = @LyDoNghiPhep
    WHERE MaChamCong = @MaCC;
END
GO

-- Stored Procedure: sp_DeleteChamCong
IF OBJECT_ID('sp_DeleteChamCong', 'P') IS NOT NULL
    DROP PROCEDURE sp_DeleteChamCong;
GO
CREATE PROCEDURE sp_DeleteChamCong
    @MaCC INT
AS
BEGIN
    DELETE FROM ChamCong WHERE MaChamCong = @MaCC;
END
GO

-- Stored Procedure: sp_InsertThongBao
IF OBJECT_ID('sp_InsertThongBao', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertThongBao;
GO
CREATE PROCEDURE sp_InsertThongBao
    @TieuDe NVARCHAR(255),
    @LoaiThongBao NVARCHAR(100),
    @NoiDung NVARCHAR(MAX),
    @MaNV INT = NULL
AS
BEGIN
    INSERT INTO ThongBao (TieuDe, LoaiThongBao, NoiDung, MaNV)
    VALUES (@TieuDe, @LoaiThongBao, @NoiDung, @MaNV);
END
GO

-- Stored Procedure: sp_DeleteThongBao
IF OBJECT_ID('sp_DeleteThongBao', 'P') IS NOT NULL
    DROP PROCEDURE sp_DeleteThongBao;
GO
CREATE PROCEDURE sp_DeleteThongBao
    @MaTB INT
AS
BEGIN
    DELETE FROM ThongBao WHERE MaThongBao = @MaTB;
END
GO

-- Function: fn_GetTongNhanVien
IF OBJECT_ID('fn_GetTongNhanVien', 'FN') IS NOT NULL
    DROP FUNCTION fn_GetTongNhanVien;
GO
CREATE FUNCTION fn_GetTongNhanVien()
RETURNS INT
AS
BEGIN
    RETURN (SELECT COUNT(*) FROM NhanVien);
END
GO

-- Function: fn_GetTongLuongThang
IF OBJECT_ID('fn_GetTongLuongThang', 'FN') IS NOT NULL
    DROP FUNCTION fn_GetTongLuongThang;
GO
CREATE FUNCTION fn_GetTongLuongThang(@Thang INT, @Nam INT)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @TongLuong DECIMAL(18,2);
    SELECT @TongLuong = SUM(LgThucNhan)
    FROM Luong
    WHERE MONTH(ThangLuong) = @Thang AND YEAR(ThangLuong) = @Nam;
    RETURN ISNULL(@TongLuong, 0);
END
GO

USE QuanLyXeMuaBanXeMay;
GO

CREATE PROCEDURE sp_TimThongTinTheoSDT
    @Loai NVARCHAR(50),       -- 'KhachHang' hoặc 'NhaCungCap'
    @SoDienThoai NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Loai = N'KhachHang'
    BEGIN
        SELECT HoTen AS Ten, DiaChi
        FROM KhachHang
        WHERE SoDienThoai = @SoDienThoai;
    END
    ELSE IF @Loai = N'NCC'
    BEGIN
        SELECT TenNCC AS Ten, DiaChi
        FROM NhaCungCap
        WHERE SoDienThoai = @SoDienThoai;
    END
END
GO


