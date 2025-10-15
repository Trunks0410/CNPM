using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using System.Net.Mail;

namespace PROJECT_CK
{
    internal class QuanLyNhanVien
    {
        private string connectionString = "Data Source=.;Initial Catalog=QuanLyXeMuaBanXeMay;Integrated Security=True";

        public int InsertNhanVien(string hoTen, DateTime ngaySinh, string gioiTinh, string soDT, string email, string diaChi, string cccd,
                                  string chucVu, DateTime ngayNhanViec, decimal luongCB, string hinhAnh,
                                  string tenTK = null, string matKhau = null, string vaiTro = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_InsertNhanVien", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@HoTenNV", hoTen);
                    cmd.Parameters.AddWithValue("@NgaySinh", ngaySinh);
                    cmd.Parameters.AddWithValue("@GioiTinh", gioiTinh);
                    cmd.Parameters.AddWithValue("@SoDT", soDT);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@DiaChi", diaChi);
                    cmd.Parameters.AddWithValue("@CCCD", cccd);
                    cmd.Parameters.AddWithValue("@ChucVu", chucVu);
                    cmd.Parameters.AddWithValue("@NgayNhanViec", ngayNhanViec);
                    cmd.Parameters.AddWithValue("@LuongCB", luongCB);
                    cmd.Parameters.AddWithValue("@HinhAnh", hinhAnh);
                    if (tenTK != null) cmd.Parameters.AddWithValue("@TenTK", tenTK);
                    if (matKhau != null) cmd.Parameters.AddWithValue("@MatKhau", matKhau);
                    if (vaiTro != null) cmd.Parameters.AddWithValue("@VaiTro", vaiTro);

                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        public void UpdateNhanVien(int maNV, string hoTen, DateTime ngaySinh, string gioiTinh, string soDT, string email,
                                  string diaChi, string cccd, string chucVu, DateTime ngayNhanViec, decimal luongCB, string hinhAnh)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_UpdateNhanVien", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaNV", maNV);
                    cmd.Parameters.AddWithValue("@HoTenNV", hoTen);
                    cmd.Parameters.AddWithValue("@NgaySinh", ngaySinh);
                    cmd.Parameters.AddWithValue("@GioiTinh", gioiTinh);
                    cmd.Parameters.AddWithValue("@SoDT", soDT);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@DiaChi", diaChi);
                    cmd.Parameters.AddWithValue("@CCCD", cccd);
                    cmd.Parameters.AddWithValue("@ChucVu", chucVu);
                    cmd.Parameters.AddWithValue("@NgayNhanViec", ngayNhanViec);
                    cmd.Parameters.AddWithValue("@LuongCB", luongCB);
                    cmd.Parameters.AddWithValue("@HinhAnh", hinhAnh);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteNhanVien(int maNV)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_DeleteNhanVien", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaNV", maNV);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertChamCong(int maNV, DateTime ngayLamViec, TimeSpan thoiGianVaoLam, TimeSpan thoiGianTanCa, string trangThai, string lyDoNghiPhep = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_InsertChamCong", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaNV", maNV);
                    cmd.Parameters.AddWithValue("@NgayLamViec", ngayLamViec.Date);
                    cmd.Parameters.AddWithValue("@ThoiGianVaoLam", thoiGianVaoLam);
                    cmd.Parameters.AddWithValue("@ThoiGianTanCa", thoiGianTanCa);
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                    if (!string.IsNullOrEmpty(lyDoNghiPhep))
                        cmd.Parameters.AddWithValue("@LyDoNghiPhep", lyDoNghiPhep);
                    else
                        cmd.Parameters.AddWithValue("@LyDoNghiPhep", DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateChamCong(int maCC, int maNV, DateTime ngayLamViec, TimeSpan thoiGianVaoLam, TimeSpan thoiGianTanCa, string trangThai, string lyDoNghiPhep = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_UpdateChamCong", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaCC", maCC);
                    cmd.Parameters.AddWithValue("@MaNV", maNV);
                    cmd.Parameters.AddWithValue("@NgayLamViec", ngayLamViec.Date);
                    cmd.Parameters.AddWithValue("@ThoiGianVaoLam", thoiGianVaoLam);
                    cmd.Parameters.AddWithValue("@ThoiGianTanCa", thoiGianTanCa);
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                    if (!string.IsNullOrEmpty(lyDoNghiPhep))
                        cmd.Parameters.AddWithValue("@LyDoNghiPhep", lyDoNghiPhep);
                    else
                        cmd.Parameters.AddWithValue("@LyDoNghiPhep", DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteChamCong(int maCC)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_DeleteChamCong", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaCC", maCC);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertThongBao(string tieuDe, string loaiThongBao, string noiDung, int? maNV = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_InsertThongBao", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TieuDe", tieuDe);
                    cmd.Parameters.AddWithValue("@LoaiThongBao", loaiThongBao);
                    cmd.Parameters.AddWithValue("@NoiDung", noiDung);
                    if (maNV.HasValue) cmd.Parameters.AddWithValue("@MaNV", maNV.Value);
                    else cmd.Parameters.AddWithValue("@MaNV", DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteThongBao(int maTB)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_DeleteThongBao", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaTB", maTB);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int GetTongNhanVien()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT dbo.fn_GetTongNhanVien()", conn))
                {
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        public decimal GetTrungBinhKPI(int thang, int nam)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT dbo.fn_GetTrungBinhKPI(@Thang, @Nam)", conn))
                {
                    cmd.Parameters.AddWithValue("@Thang", thang);
                    cmd.Parameters.AddWithValue("@Nam", nam);
                    return (decimal)cmd.ExecuteScalar();
                }
            }
        }

        public int GetTongNhanVienMoi()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM NhanVien WHERE NgayNhanViec >= DATEADD(day, -30, GETDATE())", conn))
                {
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        public decimal GetTongLuongThang(int thang, int nam)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT dbo.fn_GetTongLuongThang(@Thang, @Nam)", conn))
                {
                    cmd.Parameters.AddWithValue("@Thang", thang);
                    cmd.Parameters.AddWithValue("@Nam", nam);
                    return (decimal)cmd.ExecuteScalar();
                }
            }
        }

        public DataTable GetDataForPieChartNhanVien(string criteria)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.fn_GetDataForPieChartNhanVien(@Criteria)", conn))
                {
                    cmd.Parameters.AddWithValue("@Criteria", criteria);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable GetDataForBarChartKPI(int nam)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.fn_GetDataForBarChartKPI(@Nam)", conn))
                {
                    cmd.Parameters.AddWithValue("@Nam", nam);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable GetDataForLineChartLuong()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
              SELECT FORMAT(ThangLuong, 'yyyy-MM') AS Thang, AVG(LgThucNhan) AS LuongTrungBinh 
              FROM Luong 
              GROUP BY FORMAT(ThangLuong, 'yyyy-MM')
              ORDER BY FORMAT(ThangLuong, 'yyyy-MM')", conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable GetDataForBarChartLuongTong()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
              SELECT FORMAT(ThangLuong, 'yyyy-MM') AS Thang, SUM(LgThucNhan) AS TongLuong 
              FROM Luong 
              GROUP BY FORMAT(ThangLuong, 'yyyy-MM')
              ORDER BY FORMAT(ThangLuong, 'yyyy-MM')", conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable GetDataForBarChartChamCong()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT TrangThai, COUNT(*) AS SoLuong FROM ChamCong GROUP BY TrangThai", conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable GetNhanVienList()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT MaNV, HoTenNV, NgaySinh, GioiTinh, SoDT, Email, ChucVu, LuongCB FROM NhanVien", conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public DataRow GetNhanVienById(int maNV)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM NhanVien WHERE MaNV = @MaNV", conn))
                {
                    cmd.Parameters.AddWithValue("@MaNV", maNV);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public DataRow GetChamCongById(int maCC)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM ChamCong WHERE MaChamCong = @MaCC", conn))
                {
                    cmd.Parameters.AddWithValue("@MaCC", maCC);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public TimeSpan GetTongThoiGianLamViec(TimeSpan timeIn, TimeSpan timeOut)
        {
            if (timeOut >= timeIn)
            {
                return timeOut - timeIn;
            }
            else
            {
                TimeSpan endOfDay = new TimeSpan(24, 0, 0);
                TimeSpan duration = (endOfDay - timeIn) + timeOut;
                return duration;
            }
        }

        public DataTable GetChamCongList()
        {
            DataTable dt = new DataTable();
            string query = @"
            SELECT  
                CC.MaChamCong, CC.MaNV, NV.HoTenNV, CC.NgayLamViec, 
                CC.TgVaoLam, CC.TgTanCa, CC.TrangThai, CC.LyDoNghiPhep
            FROM 
                ChamCong CC JOIN NhanVien NV ON CC.MaNV = NV.MaNV
            ORDER BY 
                CC.NgayLamViec DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }

            dt.Columns.Add("TongThoiGianLamViec", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                if (row["TrangThai"].ToString() == "Có mặt" && row["TgVaoLam"] != DBNull.Value && row["TgTanCa"] != DBNull.Value)
                {
                    TimeSpan timeIn = (TimeSpan)row["TgVaoLam"];
                    TimeSpan timeOut = (TimeSpan)row["TgTanCa"];
                    TimeSpan duration = this.GetTongThoiGianLamViec(timeIn, timeOut);
                    row["TongThoiGianLamViec"] = $"{duration.Hours:D2}:{duration.Minutes:D2}";
                }
                else
                {
                    row["TongThoiGianLamViec"] = "00:00";
                }
            }
            return dt;
        }

        public DataTable GetThongBaoList()
        {
            DataTable dt = new DataTable();
            string query = "SELECT MaThongBao, TieuDe, LoaiThongBao, NoiDung, NgayTao FROM ThongBao ORDER BY NgayTao DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable SearchNhanVien(string searchText)
        {
            DataTable dt = new DataTable();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                return GetNhanVienList();
            }

            string query = @"
                SELECT MaNV, HoTenNV, NgaySinh, GioiTinh, SoDT, Email, ChucVu, LuongCB 
                FROM NhanVien 
                WHERE (@MaNV IS NOT NULL AND MaNV = @MaNV) 
                   OR HoTenNV LIKE '%' + @searchText + '%'";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (int.TryParse(searchText, out int maNV))
                        cmd.Parameters.AddWithValue("@MaNV", maNV);
                    else
                        cmd.Parameters.AddWithValue("@MaNV", DBNull.Value);
                    cmd.Parameters.AddWithValue("@searchText", searchText);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable SearchChamCong(DateTime? ngayLamViec, string maNVText)
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT  
                    CC.MaChamCong, CC.MaNV, NV.HoTenNV, CC.NgayLamViec,  
                    CC.TgVaoLam, CC.TgTanCa, CC.TrangThai, CC.LyDoNghiPhep
                FROM 
                    ChamCong CC JOIN NhanVien NV ON CC.MaNV = NV.MaNV
                WHERE 
                    (@NgayLamViec IS NULL OR CAST(CC.NgayLamViec AS DATE) = @NgayLamViec)
                    AND (@MaNV IS NULL OR CC.MaNV = @MaNV)
                ORDER BY  
                    CC.NgayLamViec DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (ngayLamViec.HasValue)
                        cmd.Parameters.AddWithValue("@NgayLamViec", ngayLamViec.Value.Date);
                    else
                        cmd.Parameters.AddWithValue("@NgayLamViec", DBNull.Value);

                    if (!string.IsNullOrWhiteSpace(maNVText) && int.TryParse(maNVText, out int maNV))
                        cmd.Parameters.AddWithValue("@MaNV", maNV);
                    else
                        cmd.Parameters.AddWithValue("@MaNV", DBNull.Value);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }

            dt.Columns.Add("TongThoiGianLamViec", typeof(string));
            foreach (DataRow row in dt.Rows)
            {
                if (row["TgVaoLam"] != DBNull.Value && row["TgTanCa"] != DBNull.Value && row["TrangThai"].ToString() == "Có mặt")
                {
                    TimeSpan timeIn = (TimeSpan)row["TgVaoLam"];
                    TimeSpan timeOut = (TimeSpan)row["TgTanCa"];
                    TimeSpan duration = GetTongThoiGianLamViec(timeIn, timeOut);
                    row["TongThoiGianLamViec"] = $"{duration.Hours:D2}:{duration.Minutes:D2}";
                }
                else
                {
                    row["TongThoiGianLamViec"] = "00:00";
                }
            }
            return dt;
        }

        public decimal GetLuongCoBanByMaNV(int maNV)
        {
            decimal luongCB = 0;
            string query = "SELECT LuongCB FROM NhanVien WHERE MaNV = @MaNV";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNV", maNV);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        luongCB = Convert.ToDecimal(result);
                    }
                }
            }
            return luongCB;
        }

        public DataTable GetBangLuongList(DateTime monthYear)
        {
            DataTable dt = new DataTable();

            DateTime firstDay = new DateTime(monthYear.Year, monthYear.Month, 1);
            DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);

            string query = @"
          SELECT 
              NV.MaNV, 
              NV.HoTenNV, 
              NV.ChucVu, 
              NV.LuongCB,
              SUM(CASE 
                  WHEN CC.TrangThai = N'Có mặt' THEN DATEDIFF(MINUTE, CC.TgVaoLam, CC.TgTanCa)
                  ELSE 0 
              END) AS TongSoPhutLam,
              CAST(MONTH(@monthYear) AS NVARCHAR) + '/' + CAST(YEAR(@monthYear) AS NVARCHAR) AS ThangNamHienThi,
              ISNULL(L.LgThucNhan, 0) AS LuongThucNhan,
              ISNULL(L.TienThuong, 0) AS KhoanThuong,
              ISNULL(L.KhauTru, 0) AS KhoanKhauTru
          FROM NhanVien NV
          LEFT JOIN ChamCong CC ON NV.MaNV = CC.MaNV 
              AND CC.NgayLamViec BETWEEN @FirstDay AND @LastDay
          LEFT JOIN Luong L ON NV.MaNV = L.MaNV 
              AND YEAR(L.ThangLuong) = YEAR(@monthYear) AND MONTH(L.ThangLuong) = MONTH(@monthYear)
          GROUP BY 
              NV.MaNV, NV.HoTenNV, NV.ChucVu, NV.LuongCB, L.LgThucNhan, L.TienThuong, L.KhauTru
          ORDER BY 
              NV.MaNV";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@monthYear", monthYear);
                    cmd.Parameters.AddWithValue("@FirstDay", firstDay);
                    cmd.Parameters.AddWithValue("@LastDay", lastDay);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }

            dt.Columns.Add("MaLuong", typeof(int));
            dt.Columns.Add("TongGioLam", typeof(string));
            dt.Columns.Add("LuongThucNhanTamTinh", typeof(decimal));

            int counter = 1;

            foreach (DataRow row in dt.Rows)
            {
                int totalMinutes = row["TongSoPhutLam"] != DBNull.Value ? Convert.ToInt32(row["TongSoPhutLam"]) : 0;

                int hours = totalMinutes / 60;
                int minutes = totalMinutes % 60;

                row["MaLuong"] = counter++;
                row["TongGioLam"] = $"{hours} giờ {minutes} phút";

                decimal luongThucNhan = Convert.ToDecimal(row["LuongThucNhan"]);
                if (luongThucNhan == 0)
                {
                    decimal luongCoBan = Convert.ToDecimal(row["LuongCB"]);
                    decimal luongThucNhanTamTinh = (luongCoBan / 176) * (totalMinutes / 60m);
                    row["LuongThucNhanTamTinh"] = Math.Round(luongThucNhanTamTinh, 0);
                }
                else
                {
                    row["LuongThucNhanTamTinh"] = luongThucNhan;
                }
            }

            if (dt.Columns.Contains("TongSoPhutLam")) dt.Columns.Remove("TongSoPhutLam");

            return dt;
        }

        public bool LuuBangLuong(int maNV, DateTime thangNam, decimal luongThucNhan, decimal khoanThuong, decimal khoanKhauTru, decimal tongGioThang = 0, decimal gioLamThem = 0)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_LuuBangLuong", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaNV", maNV);
                    cmd.Parameters.AddWithValue("@ThangLuong", new DateTime(thangNam.Year, thangNam.Month, 1));
                    cmd.Parameters.AddWithValue("@LuongThucNhan", luongThucNhan);
                    cmd.Parameters.AddWithValue("@KhoanThuong", khoanThuong);
                    cmd.Parameters.AddWithValue("@KhoanKhauTru", khoanKhauTru);
                    cmd.Parameters.AddWithValue("@TongGioThang", tongGioThang);
                    cmd.Parameters.AddWithValue("@GioLamThem", gioLamThem);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public DataTable GetKPIList()
        {
            DataTable dt = new DataTable();
            string query = @"
        SELECT 
            NV.MaNV, 
            NV.HoTenNV, 
            NV.ChucVu, 
            ISNULL(K.DiemKPI, 0) AS DiemKPI
        FROM 
            NhanVien NV
        LEFT JOIN 
            KPI_Thang K ON NV.MaNV = K.MaNV 
                        AND K.Thang = MONTH(GETDATE()) 
                        AND K.Nam = YEAR(GETDATE())
        ORDER BY 
            NV.MaNV;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }
        public DataRow GetTaiKhoanByMaNV(int maNV)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TenTK, MatKhau, VaiTro FROM TaiKhoan WHERE MaNV = @MaNV";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNV", maNV);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }
        public void GuiEmailMatKhau(string emailNguoiNhan, string tenNhanVien, string tenTaiKhoan, string matKhau)
        {
            string fromMail = "thanhtrung050410@gmail.com"; 
            string fromPassword = "rhjv ibjj myyt qfoh"; 

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = "Thông tin tài khoản nhân viên mới";
            message.To.Add(new MailAddress(emailNguoiNhan));

            // Nội dung email
            message.Body = $@"
        <html>
        <body>
            <p>Chào {tenNhanVien},</p>
            <p>Tài khoản của bạn đã được tạo thành công trên hệ thống của chúng tôi.</p>
            <p>Dưới đây là thông tin đăng nhập của bạn:</p>
            <ul>
                <li><strong>Tên tài khoản:</strong> {tenTaiKhoan}</li>
                <li><strong>Mật khẩu:</strong> {matKhau}</li>
            </ul>
            <p>Vui lòng đổi mật khẩu sau khi đăng nhập lần đầu tiên để đảm bảo an toàn.</p>
            <p>Trân trọng,<br>Cửa hàng UTE BIKE</p>
        </body>
        </html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            smtpClient.Send(message);
        }
        public void UpdateTaiKhoan(int maNV, string matKhau, string vaiTro)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE TaiKhoan SET MatKhau = @MatKhau, VaiTro = @VaiTro WHERE MaNV = @MaNV";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNV", maNV);
                    cmd.Parameters.AddWithValue("@MatKhau", matKhau);
                    cmd.Parameters.AddWithValue("@VaiTro", vaiTro);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private DataTable GetDataFromFunction(string query, SqlParameter parameter = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameter != null) cmd.Parameters.Add(parameter);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }
    }
}