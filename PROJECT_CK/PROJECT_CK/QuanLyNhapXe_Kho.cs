using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static PROJECT_CK.Main;


namespace QuanLyMuaBanXeMay
{
    public class QuanLyNhapXe_Kho
    {
        private readonly string connectionString = "Data Source=.;Initial Catalog=QuanLyXeMuaBanXeMay;Integrated Security=True;";

        //public QuanLyNhapXe_Kho(string connStr)
        //{
        //    connectionString = connStr;
        //}
        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public DataTable GetDanhSachPhieuNhap()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetDanhSachPhieuNhap", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable TimKiemPhieuNhap(DateTime? tuNgay, DateTime? denNgay, string loaiNhap, int? nccID, int? khachHangID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_TimKiemPhieuNhap", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TuNgay", (object)tuNgay ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DenNgay", (object)denNgay ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LoaiNhap", string.IsNullOrEmpty(loaiNhap) ? (object)DBNull.Value : loaiNhap);
                    cmd.Parameters.AddWithValue("@NCCID", (object)nccID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@KhachHangID", (object)khachHangID ?? DBNull.Value);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable GetThongKePhieuNhap(DateTime? tuNgay, DateTime? denNgay, string loaiNhap, int? nccID, int? khachHangID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ThongKeNhap_Filter", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TuNgay", (object)tuNgay ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DenNgay", (object)denNgay ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LoaiNhap", string.IsNullOrEmpty(loaiNhap) ? "All" : loaiNhap);
                    cmd.Parameters.AddWithValue("@NCCID", (object)nccID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@KhachHangID", (object)khachHangID ?? DBNull.Value);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        // Trong class DatabaseHelper.cs

        public DataTable GetThongTinChungPhieuNhap(int phieuNhapID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM dbo.fn_GetThongTinChungPhieuNhap(@PhieuNhapID)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text; // Gọi hàm bằng SELECT
                    cmd.Parameters.AddWithValue("@PhieuNhapID", phieuNhapID);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }


        public DataTable GetDanhSachXeNhap(int phieuNhapID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM dbo.fn_GetDanhSachXeNhap(@PhieuNhapID)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text; // Vì gọi hàm qua SELECT
                    cmd.Parameters.AddWithValue("@PhieuNhapID", phieuNhapID);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }


        public bool UpdatePhieuNhap(int phieuNhapID, DateTime ngayNhap, string loaiNhap,
               string ten, string soDT, string diaChi)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_UpdatePhieuNhap", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@PhieuNhapID", phieuNhapID);
                        cmd.Parameters.AddWithValue("@NgayNhap", ngayNhap);
                        cmd.Parameters.AddWithValue("@LoaiNhap", loaiNhap);
                        cmd.Parameters.AddWithValue("@Ten", ten);
                        cmd.Parameters.AddWithValue("@SoDT", soDT);
                        cmd.Parameters.AddWithValue("@DiaChi", diaChi);

                        cmd.ExecuteNonQuery();
                        return true; // luôn true nếu không có lỗi
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi SQL: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khác: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }




        // Lấy danh sách NCC
        public DataTable GetAllNhaCungCap()
        {
            using (SqlConnection conn = GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter("sp_GetAllNhaCungCap", conn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // Thêm NCC
        public bool InsertNhaCungCap(string ten, string diaChi, string soDT)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertNhaCungCap", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TenNCC", ten);
                        cmd.Parameters.AddWithValue("@DiaChi", diaChi);
                        cmd.Parameters.AddWithValue("@SoDienThoai", soDT);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi SQL: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khác: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool UpdateNhaCungCap(int id, string ten, string diaChi, string soDT)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateNhaCungCap", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NCCID", id);
                        cmd.Parameters.AddWithValue("@TenNCC", ten);
                        cmd.Parameters.AddWithValue("@DiaChi", diaChi);
                        cmd.Parameters.AddWithValue("@SoDienThoai", soDT);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi SQL khi cập nhật NCC:\n" + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống khi cập nhật NCC:\n" + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }



        public int DeleteNhaCungCap(int nccID)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteNhaCungCap", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NCCID", nccID);

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        cmd.ExecuteNonQuery();
                        return (int)returnParameter.Value; // trả về mã kết quả từ SP
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi SQL khi xóa NCC:\n" + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -99; // mã lỗi đặc biệt
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống khi xóa NCC:\n" + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -100;
            }
        }




        public DataTable GetKhachHang()
        {
            using (SqlConnection conn = GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter("sp_GetKhachHang", conn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public bool InsertKhachHang(string hoTen, string diaChi, string sdt)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertKhachHang", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@HoTen", hoTen);
                        cmd.Parameters.AddWithValue("@DiaChi", diaChi);
                        cmd.Parameters.AddWithValue("@SoDienThoai", sdt);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi SQL: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khác: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool UpdateKhachHang(int id, string hoTen, string diaChi, string sdt)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateKhachHang", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@KhachHangID", id);
                        cmd.Parameters.AddWithValue("@HoTen", hoTen);
                        cmd.Parameters.AddWithValue("@DiaChi", diaChi);
                        cmd.Parameters.AddWithValue("@SoDienThoai", sdt);

                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0; // chỉ trả về true khi có update
                    }
                }
            }
            catch (SqlException ex)
            {
                // Lỗi do SQL Server (ví dụ trigger RAISERROR/THROW, constraint UNIQUE,…)
                MessageBox.Show("Lỗi SQL: " + ex.Message, "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                // Lỗi khác (ví dụ kết nối DB, lỗi tham số…)
                MessageBox.Show("Lỗi khác: " + ex.Message, "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        public int DeleteKhachHang(int khachHangID)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteKhachHang", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@KhachHangID", khachHangID);

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        cmd.ExecuteNonQuery();
                        return (int)returnParameter.Value;
                    }
                }
            }
            catch (SqlException ex)
            {
                // Lỗi từ trigger sẽ được ném vào đây
                MessageBox.Show("Lỗi SQL từ trigger: " + ex.Message, "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khác: " + ex.Message, "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }



        // Lấy danh sách danh mục theo loại
        public DataTable GetDanhMucByLoai(string loaiDanhMuc)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetDanhMucByLoai", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LoaiDanhMuc", loaiDanhMuc);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        // Thêm danh mục
        public bool InsertDanhMuc(string loaiDanhMuc, string giaTri)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_InsertDanhMuc", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LoaiDanhMuc", loaiDanhMuc);
                    cmd.Parameters.AddWithValue("@GiaTri", giaTri);

                    try
                    {
                        object result = cmd.ExecuteScalar();
                        return result != null && Convert.ToInt32(result) > 0;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 50000 || ex.Number == 2627) // Duplicate
                            return false;
                        throw;
                    }
                }
            }
        }

        // Sửa danh mục
        public bool UpdateDanhMuc(int danhMucID, string giaTri)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_UpdateDanhMuc", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DanhMucID", danhMucID);
                    cmd.Parameters.AddWithValue("@GiaTri", giaTri);

                    try
                    {
                        return cmd.ExecuteNonQuery() > 0;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 50000 || ex.Number == 2627)
                            return false;
                        throw;
                    }
                }
            }
        }

        // Xóa danh mục
        public bool DeleteDanhMuc(int danhMucID)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_DeleteDanhMuc", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DanhMucID", danhMucID);

                    try
                    {
                        return cmd.ExecuteNonQuery() > 0;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 547) // Vi phạm khóa ngoại
                            return false;
                        throw;
                    }
                }
            }
        }

        public DataTable ExecuteDataTable(string query, CommandType commandType, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = commandType;
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public void UpdateChiTietXeNhap(
    int phieuNhapID,
    string soKhung,
    string tenXe,
    string hangXe,
    string loaiXe,
    string mauSac,
    int namSX,
    string soMay,
    decimal donGia,
    string hinhAnh,
    string moTa,
    string tinhTrang,
    float dungTich)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_UpdateChiTietXeNhap", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PhieuNhapID", phieuNhapID);
                    cmd.Parameters.AddWithValue("@SoKhung", soKhung);
                    cmd.Parameters.AddWithValue("@TenXe", tenXe);
                    cmd.Parameters.AddWithValue("@HangXe", hangXe);
                    cmd.Parameters.AddWithValue("@LoaiXe", loaiXe);
                    cmd.Parameters.AddWithValue("@MauSac", mauSac);
                    cmd.Parameters.AddWithValue("@NamSX", namSX);
                    cmd.Parameters.AddWithValue("@SoMay", soMay);
                    cmd.Parameters.AddWithValue("@DonGia", donGia);
                    cmd.Parameters.AddWithValue("@HinhAnh", hinhAnh);
                    cmd.Parameters.AddWithValue("@MoTa", moTa);
                    cmd.Parameters.AddWithValue("@TinhTrang", tinhTrang);
                    cmd.Parameters.AddWithValue("@DungTich", dungTich);

                    int rows = cmd.ExecuteNonQuery();
                }
            }
        }

        // Lấy danh sách tên NCC
        public List<string> GetTenNhaCungCap()
        {
            List<string> tenList = new List<string>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT TenNCC FROM dbo.fn_GetTenNhaCungCap() ORDER BY TenNCC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tenList.Add(reader["TenNCC"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy danh sách tên NCC: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return tenList;
        }

        // Lấy danh sách tên Khách Hàng
        public List<string> GetTenKhachHang()
        {
            List<string> tenList = new List<string>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT HoTen FROM dbo.fn_GetTenKhachHang() ORDER BY HoTen";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tenList.Add(reader["HoTen"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy danh sách tên KH: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return tenList;
        }

        // Lấy danh sách số điện thoại NCC
        public List<string> GetSoDTNhaCungCap()
        {
            List<string> sdtList = new List<string>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT SoDienThoai FROM dbo.fn_GetSoDTNhaCungCap() ORDER BY SoDienThoai";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                sdtList.Add(reader["SoDienThoai"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy danh sách số ĐT NCC: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return sdtList;
        }

        // Lấy danh sách số điện thoại Khách Hàng
        public List<string> GetSoDTKhachHang()
        {
            List<string> sdtList = new List<string>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT SoDienThoai FROM dbo.fn_GetSoDTKhachHang() ORDER BY SoDienThoai";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                sdtList.Add(reader["SoDienThoai"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy danh sách số ĐT KH: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return sdtList;
        }



        public DataTable GetThongTinBySoDT(string soDT, string loaiNhap)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_GetThongTinBySoDT", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SoDT", soDT ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@LoaiNhap", loaiNhap ?? (object)DBNull.Value);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            return dt;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu cần, hoặc ném lại nếu muốn xử lý ở cấp cao hơn
                    throw new Exception($"Lỗi khi gọi sp_GetThongTinBySoDT: {ex.Message}", ex);
                }
            }
        }

        public int InsertPhieuNhapWithXe(DateTime ngayNhap, string loaiNhap, string ten, string soDT, string diaChi, List<Dictionary<string, string>> xeList)
        {
            try
            {
                DataTable xeTable = new DataTable();
                xeTable.Columns.Add("SoKhung", typeof(string));
                xeTable.Columns.Add("SoMay", typeof(string));
                xeTable.Columns.Add("TenXe", typeof(string));
                xeTable.Columns.Add("HangXe", typeof(string));
                xeTable.Columns.Add("LoaiXe", typeof(string));
                xeTable.Columns.Add("MauSac", typeof(string));
                xeTable.Columns.Add("NamSX", typeof(int));
                xeTable.Columns.Add("DonGia", typeof(decimal));
                xeTable.Columns.Add("HinhAnh", typeof(string));
                xeTable.Columns.Add("MoTa", typeof(string));
                xeTable.Columns.Add("TinhTrang", typeof(string));
                xeTable.Columns.Add("DungTich", typeof(float));

                foreach (var xe in xeList)
                {
                    xeTable.Rows.Add(
                        xe["SoKhung"],
                        xe["SoMay"],
                        xe["TenXe"],
                        xe["HangXe"],
                        xe["LoaiXe"],
                        xe["MauSac"],
                        int.Parse(xe["NamSX"]),
                        decimal.Parse(xe["DonGia"]),
                        xe["HinhAnh"],
                        xe["MoTa"],
                        xe.ContainsKey("TinhTrang") ? xe["TinhTrang"] : "Mới",
                        float.Parse(xe["DungTich"])
                    );
                }

                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertPhieuNhapWithXe", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@NgayNhap", ngayNhap);
                        cmd.Parameters.AddWithValue("@LoaiNhap", loaiNhap);
                        cmd.Parameters.AddWithValue("@Ten", ten);
                        cmd.Parameters.AddWithValue("@SoDT", soDT);
                        cmd.Parameters.AddWithValue("@DiaChi", diaChi);

                        SqlParameter xeListParam = cmd.Parameters.AddWithValue("@XeList", xeTable);
                        xeListParam.SqlDbType = SqlDbType.Structured;
                        xeListParam.TypeName = "XeTempType";

                        object result = cmd.ExecuteScalar();
                        return result != null && int.TryParse(result.ToString(), out int phieuNhapID) ? phieuNhapID : -1;
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi SQL: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khác: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int CheckXeExists(string soKhung, string soMay)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_CheckXeExists", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SoKhung", soKhung);
                    cmd.Parameters.AddWithValue("@SoMay", soMay);

                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        public bool DeletePhieuNhap(int phieuNhapID)
        {
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand("sp_DeletePhieuNhap", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PhieuNhapID", phieuNhapID);

                // Nhận giá trị RETURN từ procedure
                var returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParam.Direction = ParameterDirection.ReturnValue;

                conn.Open();
                cmd.ExecuteNonQuery();

                int result = (int)returnParam.Value;
                return result == 0; // ✅ TRUE nếu xóa thành công
            }
        }


        public bool DeleteXeFromPhieuNhap(int phieuNhapID, int xeCTID)
        {
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand("sp_DeleteXeFromPhieuNhap", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PhieuNhapID", phieuNhapID);
                cmd.Parameters.AddWithValue("@XeCTID", xeCTID);

                // Thêm parameter để lấy giá trị RETURN từ proc
                var returnParam = new SqlParameter();
                returnParam.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(returnParam);

                conn.Open();
                cmd.ExecuteNonQuery();

                int result = (int)returnParam.Value;
                return result == 0; // Trả về true nếu xóa thành công
            }
        }

        public int GetTongSoXeTonKho()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT dbo.fn_GetTongSoXeTonKho()";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy tổng số xe tồn kho: {ex.Message}");
            }
        }

        public decimal GetTongGiaTriTonKho()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT dbo.fn_GetTongGiaTriTonKho()";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        return (decimal)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy giá trị tồn kho: {ex.Message}");
            }
        }

        public DataTable GetAllXeTonKho()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM dbo.fn_GetAllXeTonKho()";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy dữ liệu xe tồn kho: {ex.Message}");
            }
        }

        public decimal GetTongLoiNhuanXe()
        {
            decimal tongLoiNhuan = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // gọi function SQL
                string query = "SELECT dbo.fn_GetTongLoiNhuanXe()";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        tongLoiNhuan = Convert.ToDecimal(result);
                    }
                }
            }

            return tongLoiNhuan;
        }

        // Hàm gọi Scalar Function: Tổng Giá Trị Nhập
        public decimal GetTongGiaTriNhap()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT dbo.fn_TongGiaTriNhap()", conn))
            {
                conn.Open();
                object result = cmd.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
            }
        }

        // Hàm gọi Scalar Function: Tổng Số Xe Nhập
        public int GetTongSoXeNhap()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT dbo.fn_TongSoXeNhap()", conn))
            {
                conn.Open();
                object result = cmd.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
        }

        public DataTable GetLogPhieuNhap()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT LogID, PhieuNhapID, HanhDong, NgayNhap, LoaiNhap, NCCID, KhachHangID, NguoiThucHien, NgayThucHien FROM LogPhieuNhap ORDER BY LogID DESC";
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public bool IsKhachHangExistsBySDT(string soDT)
        {
            bool exists = false;

            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT dbo.fn_IsKhachHangExistsBySDT(@SoDT)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SoDT", soDT);

                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            exists = Convert.ToBoolean(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi kiểm tra khách hàng: " + ex.Message);
            }

            return exists;
        }

        public DateTime? GetLastNhapByLoaiXe(string loaiXe)
        {
            DateTime? ngay = null;

            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT dbo.fn_GetLastNhapByLoaiXe(@LoaiXe)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoaiXe", loaiXe);

                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            ngay = Convert.ToDateTime(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lấy ngày nhập gần nhất theo LoạiXe: " + ex.Message);
            }

            return ngay;
        }

        public string TaoMoTaXe(string tenXe, string hangXe, string loaiXe, string mauSac, int namSX, double dungTich, decimal giaBan)
        {
            string moTa = string.Empty;

            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT dbo.fn_TaoMoTaXe(@TenXe, @HangXe, @LoaiXe, @MauSac, @NamSX, @DungTich, @GiaBan)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TenXe", tenXe);
                        cmd.Parameters.AddWithValue("@HangXe", hangXe);
                        cmd.Parameters.AddWithValue("@LoaiXe", loaiXe);
                        cmd.Parameters.AddWithValue("@MauSac", mauSac);
                        cmd.Parameters.AddWithValue("@NamSX", namSX);
                        cmd.Parameters.AddWithValue("@DungTich", dungTich);
                        cmd.Parameters.AddWithValue("@GiaBan", giaBan);

                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            moTa = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi tạo mô tả xe: " + ex.Message);
            }

            return moTa;
        }

        public DataTable GetDataForBarChart(string criteria)
        {
            return ExecuteDataTable(
                "sp_GetDataForBarChart",
                CommandType.StoredProcedure,
                new SqlParameter("@Criteria", criteria)
            );
        }

        public DataTable GetDataForPieChart(string criteria)
        {
            string query = "SELECT * FROM dbo.fn_GetDataForPieChart(@Criteria)";

            return ExecuteDataTable(
                query,
                CommandType.Text,
                new SqlParameter("@Criteria", criteria)
            );
        }








    }
}