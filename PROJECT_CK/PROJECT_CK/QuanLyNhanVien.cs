using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

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

        // Update UpdateNhanVien để thêm CCCD
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
            // Cần tạo Stored Procedure 'sp_DeleteNhanVien' trong SQL Server trước khi dùng
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

                    // Xử lý giá trị NULL cho LyDoNghiPhep
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

        // Thêm method cho InsertThongBao
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
                // Sử dụng Stored Procedure
                using (SqlCommand cmd = new SqlCommand("sp_DeleteThongBao", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    // Tên cột MaThongBao đã được đổi thành MaTB trong SP trước
                    cmd.Parameters.AddWithValue("@MaTB", maTB);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Lấy tổng số nhân viên
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

        // Lấy trung bình KPI
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

        // Lấy tổng lương tháng
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

        // Lấy dữ liệu biểu đồ tròn (Pie Chart) cho nhân viên
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

        // Lấy dữ liệu biểu đồ cột (Bar Chart) cho KPI
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

        // Lấy dữ liệu biểu đồ đường (Line Chart) cho Lương (xu hướng lương theo thời gian)
        public DataTable GetDataForLineChartLuong()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT ThangLuong AS Thang, AVG(LgThucNhan) AS LuongTrungBinh FROM Luong GROUP BY ThangLuong ORDER BY ThangLuong", conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        // Lấy dữ liệu biểu đồ cột (Bar Chart) cho Lương
        public DataTable GetDataForBarChartLuongTong()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT ThangLuong AS Thang, SUM(LgThucNhan) AS TongLuong FROM Luong GROUP BY ThangLuong", conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        // Lấy dữ liệu biểu đồ cột (Bar Chart) cho Chấm công
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
            if (dt.Columns.Contains("MaNV")) dt.Columns["MaNV"].ColumnName = "Mã NV";
            if (dt.Columns.Contains("HoTenNV")) dt.Columns["HoTenNV"].ColumnName = "Họ tên NV";
            if (dt.Columns.Contains("NgaySinh")) dt.Columns["NgaySinh"].ColumnName = "Ngày sinh";
            if (dt.Columns.Contains("GioiTinh")) dt.Columns["GioiTinh"].ColumnName = "Giới tính";
            if (dt.Columns.Contains("SoDT")) dt.Columns["SoDT"].ColumnName = "Số ĐT";
            if (dt.Columns.Contains("Email")) dt.Columns["Email"].ColumnName = "Email";
            if (dt.Columns.Contains("ChucVu")) dt.Columns["ChucVu"].ColumnName = "Chức vụ";
            if (dt.Columns.Contains("LuongCB")) dt.Columns["LuongCB"].ColumnName = "Lương cơ bản";

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

            // Thêm cột tính toán Tổng Thời Gian vào DataTable
            dt.Columns.Add("TongThoiGianLamViec", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                TimeSpan timeIn = (TimeSpan)row["TgVaoLam"];
                TimeSpan timeOut = (TimeSpan)row["TgTanCa"];

                if (row["TrangThai"].ToString() == "Có mặt")
                {
                    // Giả định GetTongThoiGianLamViec là một hàm có sẵn trong class này
                    TimeSpan duration = this.GetTongThoiGianLamViec(timeIn, timeOut);
                    row["TongThoiGianLamViec"] = $"{duration.Hours:D2}:{duration.Minutes:D2}";
                }
                else
                {
                    row["TongThoiGianLamViec"] = "00:00";
                }
            }
            if (dt.Columns.Contains("MaChamCong")) dt.Columns["MaChamCong"].ColumnName = "Mã CC";
            if (dt.Columns.Contains("MaNV")) dt.Columns["MaNV"].ColumnName = "Mã NV";
            if (dt.Columns.Contains("HoTenNV")) dt.Columns["HoTenNV"].ColumnName = "Họ tên";
            if (dt.Columns.Contains("NgayLamViec")) dt.Columns["NgayLamViec"].ColumnName = "Ngày làm";
            if (dt.Columns.Contains("TgVaoLam")) dt.Columns["TgVaoLam"].ColumnName = "Giờ vào";
            if (dt.Columns.Contains("TgTanCa")) dt.Columns["TgTanCa"].ColumnName = "Giờ tan";
            if (dt.Columns.Contains("TrangThai")) dt.Columns["TrangThai"].ColumnName = "Trạng thái";
            if (dt.Columns.Contains("LyDoNghiPhep")) dt.Columns["LyDoNghiPhep"].ColumnName = "Lý do nghỉ";
            if (dt.Columns.Contains("TongThoiGianLamViec")) dt.Columns["TongThoiGianLamViec"].ColumnName = "Tổng giờ làm";

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
            if (dt.Columns.Contains("MaThongBao")) dt.Columns["MaThongBao"].ColumnName = "Mã TB";
            if (dt.Columns.Contains("TieuDe")) dt.Columns["TieuDe"].ColumnName = "Tiêu đề";
            if (dt.Columns.Contains("LoaiThongBao")) dt.Columns["LoaiThongBao"].ColumnName = "Loại thông báo";
            if (dt.Columns.Contains("NoiDung")) dt.Columns["NoiDung"].ColumnName = "Nội dung";
            if (dt.Columns.Contains("NgayTao")) dt.Columns["NgayTao"].ColumnName = "Ngày tạo";

            return dt;
        }

        public DataTable SearchNhanVien(string searchText)
        {
            DataTable dt = new DataTable();

            // Nếu chuỗi tìm kiếm rỗng, trả về toàn bộ danh sách
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return GetNhanVienList(); 
            }

            string query = @"
                SELECT MaNV, HoTenNV, NgaySinh, GioiTinh, SoDT, Email, ChucVu, LuongCB 
                FROM NhanVien 
                WHERE MaNV = @searchText 
                   OR HoTenNV LIKE '%' + @searchText + '%'";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
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
                    // Tham số Ngày làm việc
                    if (ngayLamViec.HasValue)
                        cmd.Parameters.AddWithValue("@NgayLamViec", ngayLamViec.Value.Date);
                    else
                        cmd.Parameters.AddWithValue("@NgayLamViec", DBNull.Value);

                    // Tham số Mã NV
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

            // Lấy ngày đầu và ngày cuối của tháng được chọn
            DateTime firstDay = new DateTime(monthYear.Year, monthYear.Month, 1);
            DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);

            // Truy vấn kết hợp thông tin nhân viên và tính tổng giờ làm
            string query = @"
        SELECT 
            NV.MaNV, 
            NV.HoTenNV, 
            NV.ChucVu, 
            NV.LuongCB, -- Giữ lại LuongCB để tính toán sau
            SUM(CASE 
                WHEN CC.TrangThai = N'Có mặt' THEN DATEDIFF(MINUTE, CC.TgVaoLam, CC.TgTanCa)
                ELSE 0 
            END) AS TongSoPhutLam,
            CAST(MONTH(@monthYear) AS NVARCHAR) + '/' + CAST(YEAR(@monthYear) AS NVARCHAR) AS ThangNamHienThi
        FROM NhanVien NV
        LEFT JOIN ChamCong CC ON NV.MaNV = CC.MaNV 
            AND CC.NgayLamViec BETWEEN @FirstDay AND @LastDay
        GROUP BY 
            NV.MaNV, NV.HoTenNV, NV.ChucVu, NV.LuongCB
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

                decimal luongCoBan = Convert.ToDecimal(row["LuongCB"]);
                decimal luongThucNhanTamTinh = (luongCoBan / 176) * (totalMinutes / 60m);
                row["LuongThucNhanTamTinh"] = Math.Round(luongThucNhanTamTinh, 0);
            }

            if (dt.Columns.Contains("TongSoPhutLam")) dt.Columns.Remove("TongSoPhutLam");

            if (dt.Columns.Contains("MaLuong")) dt.Columns["MaLuong"].ColumnName = "Mã lương";
            if (dt.Columns.Contains("MaNV")) dt.Columns["MaNV"].ColumnName = "Mã NV";
            if (dt.Columns.Contains("HoTenNV")) dt.Columns["HoTenNV"].ColumnName = "Họ tên NV";
            if (dt.Columns.Contains("ChucVu")) dt.Columns["ChucVu"].ColumnName = "Chức vụ";
            if (dt.Columns.Contains("ThangNamHienThi")) dt.Columns["ThangNamHienThi"].ColumnName = "Tháng/Năm";
            if (dt.Columns.Contains("TongGioLam")) dt.Columns["TongGioLam"].ColumnName = "Tổng giờ làm";
            if (dt.Columns.Contains("LuongCB")) dt.Columns["LuongCB"].ColumnName = "Lương cơ bản";
            if (dt.Columns.Contains("LuongThucNhanTamTinh")) dt.Columns["LuongThucNhanTamTinh"].ColumnName = "Lương tạm tính";

            return dt;
        }

        public bool LuuBangLuong(int maNV, DateTime thangNam, decimal luongThucNhan, decimal khoanThuong, decimal khoanKhauTru)
        {
            string query = @"
        IF EXISTS (SELECT 1 FROM BangLuong WHERE MaNV = @MaNV AND MONTH(ThangNam) = MONTH(@ThangNam) AND YEAR(ThangNam) = YEAR(@ThangNam))
        BEGIN
            UPDATE BangLuong
            SET LuongThucNhan = @LuongThucNhan, KhoanThuong = @KhoanThuong, KhoanKhauTru = @KhoanKhauTru
            WHERE MaNV = @MaNV AND MONTH(ThangNam) = MONTH(@ThangNam) AND YEAR(ThangNam) = YEAR(@ThangNam);
        END
        ELSE
        BEGIN
            INSERT INTO BangLuong (MaNV, ThangNam, LuongThucNhan, KhoanThuong, KhoanKhauTru)
            VALUES (@MaNV, @ThangNam, @LuongThucNhan, @KhoanThuong, @KhoanKhauTru);
        END";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNV", maNV);
                    cmd.Parameters.AddWithValue("@ThangNam", new DateTime(thangNam.Year, thangNam.Month, 1)); // Chỉ lưu tháng/năm
                    cmd.Parameters.AddWithValue("@LuongThucNhan", luongThucNhan);
                    cmd.Parameters.AddWithValue("@KhoanThuong", khoanThuong);
                    cmd.Parameters.AddWithValue("@KhoanKhauTru", khoanKhauTru);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }


        // Helper method để gọi function trả về DataTable
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

