using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace PROJECT_CK
{
    public class QuanLyBanXe
    {
        
        public System.Globalization.CultureInfo cultureInfoVN = new System.Globalization.CultureInfo("vi-VN");
        public static SqlConnection GetConnection()
        {
            string connectionString = "Data Source=.;Initial Catalog=QuanLyXeMuaBanXeMay;Integrated Security=True;";
            return new SqlConnection(connectionString);
        }
        public static object ExecuteScalar(string sql, Dictionary<string, object> parameters)
        {
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                // ⚠️ SỬA LỖI: PHẢI CHỈ ĐỊNH RẰNG ĐÂY LÀ STORES PROCEDURE
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        // Sử dụng AddWithValue an toàn
                        cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    }
                }

                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
        public static DataTable GetDoanhThuNamHienTai()
        {
            DataTable dt = new DataTable();

            string query = "SELECT * FROM dbo.BaoCaoDoanhThu()";

            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // 1. CommandType là Text
                command.CommandType = CommandType.Text;

                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi lấy dữ liệu doanh thu năm hiện tại: " + ex.Message);
                }
            }
            return dt;
        }
        public static DataTable LayBaoCaoSoDonHangTheoThang(int nam)
        {



            DataTable dtBaoCao = new DataTable();

            using (SqlConnection connection = GetConnection())
            {
                using (SqlCommand command = new SqlCommand("sp_BaoCaoSoDonHangTheoThang", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Nam", nam);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            dtBaoCao.Load(reader);
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine($"Lỗi khi lấy dữ liệu báo cáo: {ex.Message}");
                    }
                }
            }
            return dtBaoCao;
        }
        public static int LayTongSoDonHangTheoNam(int nam)
        {

            int tongDonHang = 0;

            using (SqlConnection connection = GetConnection())
            {

                string sqlQuery = "SELECT dbo.fn_DemTongSoDonHangTheoNam(@Nam)";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {

                    command.Parameters.AddWithValue("@Nam", nam);

                    try
                    {
                        connection.Open();


                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            tongDonHang = Convert.ToInt32(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return tongDonHang;
        }
        public static DataTable GetDanhSachBanChay(DateTime ngayBatDau, DateTime ngayKetThuc, string Loai)
        {
            DataTable dt = new DataTable();
            
            // Câu lệnh SELECT gọi hàm TVF. CHÚ Ý: TVF được gọi như một bảng!
            string query = "SELECT * FROM dbo.ThongKeBanChay(@NgayBatDau, @NgayKetThuc, @LoaiXe)";

            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // 1. CommandType là Text, KHÔNG phải StoredProcedure
                command.CommandType = CommandType.Text;

                
                command.Parameters.AddWithValue("@NgayBatDau", ngayBatDau.Date);
                command.Parameters.AddWithValue("@NgayKetThuc", ngayKetThuc.Date);
                command.Parameters.AddWithValue("@LoaiXe", string.IsNullOrEmpty(Loai) ? (object)DBNull.Value : Loai);

                try
                {
                    connection.Open();

                    // 3. Sử dụng SqlDataAdapter để điền dữ liệu vào DataTable
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi (ví dụ: hiển thị MessageBox)
                    MessageBox.Show("Lỗi khi lấy dữ liệu thống kê: " + ex.Message);
                }
            }
            return dt;
        }
        public static DataTable GetDanhSachBanCham(string loai)
        {
            DataTable dt = new DataTable();
            ;

            string query = "SELECT * FROM dbo.ThongKeBanCham(@LoaiXe)";

            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // 1. CommandType là Text (bắt buộc khi gọi TVF qua SELECT)
                command.CommandType = CommandType.Text;

                // 2. Thêm tham số @Type
                command.Parameters.AddWithValue("@LoaiXe", string.IsNullOrEmpty(loai) ? (object)DBNull.Value : loai);

                try
                {
                    connection.Open();

                    // 3. Sử dụng SqlDataAdapter để điền dữ liệu
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi
                    MessageBox.Show("Lỗi khi lấy dữ liệu thống kê bán chậm: " + ex.Message);
                }
            }
            return dt;
        }
        public static DataTable GetDanhSachUuDaiCombined(string searchTerm, string status)
        {
            DataTable dt = new DataTable();
            // Lệnh gọi Stored Procedure đã được cập nhật
            string query = "SP_UuDai_SearchCombined";

            // Khởi tạo và sử dụng tài nguyên (SqlConnection, SqlCommand)
            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // 1. CommandType là StoredProcedure
                command.CommandType = CommandType.StoredProcedure;

                // 2. Thêm tham số @SearchTerm
                // Nếu chuỗi tìm kiếm rỗng hoặc null, gửi DBNull.Value để SP xử lý
                command.Parameters.AddWithValue("@SearchTerm", string.IsNullOrEmpty(searchTerm) ? (object)DBNull.Value : searchTerm);

                // 3. Thêm tham số @Status
                command.Parameters.AddWithValue("@Status", status);

                try
                {
                    connection.Open();

                    // 4. Sử dụng SqlDataAdapter để điền dữ liệu
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi
                    MessageBox.Show("Lỗi khi tìm kiếm ưu đãi kết hợp: " + ex.Message);
                }
            }
            return dt;
        }
        public static string GetNewMaUuDai(string loaiUuDai)
        {
            string newMa = null;
            // Gọi hàm vô hướng với tham số
            string query = "SELECT dbo.FN_GenerateMaUuDai(@Loai)";

            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.CommandType = CommandType.Text;

                // Thêm tham số @Loai
                command.Parameters.AddWithValue("@Loai", loaiUuDai);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        newMa = result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tạo mã ưu đãi tự động: " + ex.Message);
                }
            }
            return newMa;
        }
        
        public static void InsertUuDai(
            string maUuDai,
            string tenUuDai,
            string loai,
            decimal giaTri,
            decimal dieuKienToiThieu, // <-- THÊM MỚI: Tham số cho điều kiện tối thiểu
            DateTime tuNgay,
            DateTime denNgay,
            string moTa)
            {
                // THÊM MỚI: Kiểm tra logic cho tham số mới ngay trên C#
                if (dieuKienToiThieu < 0)
                {
                    throw new ArgumentException("Điều kiện tối thiểu không được là số âm.");
                }
                if (denNgay.Date < tuNgay.Date)
                {
                    throw new ArgumentException("Ngày kết thúc phải lớn hơn hoặc bằng Ngày bắt đầu.");
                }

                string query = "SP_UuDai_Insert";
                SqlTransaction transaction = null;

                using (SqlConnection connection = GetConnection())
                {
                    try
                    {
                        connection.Open();
                        transaction = connection.BeginTransaction();

                        using (SqlCommand command = new SqlCommand(query, connection, transaction))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            // Thêm tham số
                            command.Parameters.AddWithValue("@MaUuDai", maUuDai);
                            command.Parameters.AddWithValue("@TenUuDai", tenUuDai);
                            command.Parameters.AddWithValue("@LoaiUuDai", loai.ToUpper());
                            command.Parameters.AddWithValue("@GiaTriGiam", giaTri);
                            command.Parameters.AddWithValue("@DieuKienToiThieu", dieuKienToiThieu); // <-- THÊM MỚI
                            command.Parameters.AddWithValue("@NgayBatDau", tuNgay.Date);
                            command.Parameters.AddWithValue("@NgayKetThuc", denNgay.Date);
                            command.Parameters.AddWithValue("@MoTa", string.IsNullOrEmpty(moTa) ? (object)DBNull.Value : moTa);

                            command.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                    catch (SqlException ex)
                    {
                        if (transaction != null) transaction.Rollback();
                        if (ex.Number == 2627)
                        {
                            throw new InvalidOperationException("Mã chương trình (MaUuDai) đã tồn tại.", ex);
                        }
                        // Ném lỗi từ RAISERROR của SP
                        throw new InvalidOperationException("Lỗi Cơ Sở Dữ Liệu: " + ex.Message, ex);
                    }
                    catch (Exception ex)
                    {
                        if (transaction != null) transaction.Rollback();
                        throw;
                    }
                }
           }
        public static void UpdateUuDai(
     string maUuDai,
     string tenUuDai,
     string loai,
     decimal giaTri,
     decimal dieuKienToiThieu, // <-- THÊM MỚI: Tham số cho điều kiện tối thiểu
     DateTime tuNgay,
     DateTime denNgay,
     string moTa)
        {
            // THÊM MỚI: Kiểm tra logic cho tham số mới
            if (dieuKienToiThieu < 0)
            {
                throw new ArgumentException("Điều kiện tối thiểu không được là số âm.");
            }
            if (denNgay.Date < tuNgay.Date)
            {
                throw new ArgumentException("Ngày kết thúc phải lớn hơn hoặc bằng Ngày bắt đầu.");
            }

            string query = "SP_UuDai_Update";
            SqlTransaction transaction = null;

            using (SqlConnection connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    using (SqlCommand command = new SqlCommand(query, connection, transaction))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Thêm tham số
                        command.Parameters.AddWithValue("@MaUuDai", maUuDai);
                        command.Parameters.AddWithValue("@TenUuDai", tenUuDai);
                        command.Parameters.AddWithValue("@LoaiUuDai", loai.ToUpper());
                        command.Parameters.AddWithValue("@GiaTriGiam", giaTri);
                        command.Parameters.AddWithValue("@DieuKienToiThieu", dieuKienToiThieu); // <-- THÊM MỚI
                        command.Parameters.AddWithValue("@NgayBatDau", tuNgay.Date);
                        command.Parameters.AddWithValue("@NgayKetThuc", denNgay.Date);
                        command.Parameters.AddWithValue("@MoTa", string.IsNullOrEmpty(moTa) ? (object)DBNull.Value : moTa);

                        int rowsAffected = command.ExecuteNonQuery();
                        transaction.Commit();

                        if (rowsAffected == 0)
                        {
                            throw new InvalidOperationException($"Không tìm thấy Mã Ưu Đãi ({maUuDai}) để cập nhật.");
                        }
                    }
                }
                catch (SqlException ex)
                {
                    if (transaction != null) transaction.Rollback();
                    throw new InvalidOperationException("Lỗi Cơ Sở Dữ Liệu khi cập nhật: " + ex.Message, ex);
                }
                catch (Exception ex)
                {
                    if (transaction != null) transaction.Rollback();
                    throw;
                }
            }
        }
        public static void DeleteUuDai(string maUuDai)
        {
            string query = "SP_UuDai_Delete"; // Tên Stored Procedure
            SqlTransaction transaction = null;

            using (SqlConnection connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    using (SqlCommand command = new SqlCommand(query, connection, transaction))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MaUuDai", maUuDai);

                        int rowsAffected = command.ExecuteNonQuery();

                        transaction.Commit();

                        if (rowsAffected == 0)
                        {
                            // Ném lỗi nếu không tìm thấy mã để xóa
                            throw new InvalidOperationException($"Không tìm thấy Mã Ưu Đãi ({maUuDai}) để xóa.");
                        }
                        
                    }
                }
                catch (SqlException ex)
                {
                    if (transaction != null) transaction.Rollback(); // Rollback nếu có lỗi

                    // Mã lỗi 547 là lỗi vi phạm Khóa Ngoại (ForeignKey Constraint Violation)
                    if (ex.Number == 547)
                    {
                        throw new InvalidOperationException("Không thể xóa ưu đãi này vì nó đã được sử dụng trong các ĐƠN HÀNG.", ex);
                    }
                    // Ném lỗi SQL chung
                    throw new InvalidOperationException("Lỗi Cơ Sở Dữ Liệu khi xóa: " + ex.Message, ex);
                }
                catch (Exception ex)
                {
                    if (transaction != null) transaction.Rollback();
                    throw;
                }
            }
        }
        public static DataTable GetUuDaiByMa(string maUuDai)
        {
            DataTable dt = new DataTable();
            string query = "SP_UuDai_GetByMa";

            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Thiết lập CommandType là StoredProcedure
                command.CommandType = CommandType.StoredProcedure;

                // Thêm tham số đầu vào @MaUuDai
                command.Parameters.AddWithValue("@MaUuDai", maUuDai);

                try
                {
                    // Mở kết nối và sử dụng SqlDataAdapter để điền dữ liệu
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi (ví dụ: hiển thị thông báo)
                    MessageBox.Show("Lỗi khi lấy chi tiết ưu đãi: " + ex.Message, "Lỗi truy vấn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Trả về DataTable rỗng nếu có lỗi
                    dt = new DataTable();
                }
            }
            return dt;
        }
        public static DataTable GetActiveUuDaiCodes()
        {
            DataTable dt = new DataTable();
            string query = "SP_UuDai_GetAllActive";

            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải danh sách mã ưu đãi: " + ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dt = new DataTable(); // Trả về DataTable rỗng nếu có lỗi
                }
            }
            return dt;
        }
        public static DataTable TimKhachHangBangSDT(string soDienThoai)
        {
            // Tên Stored Procedure
            string storedProcedureName = "TimKhachHangBangSDT";
            DataTable dtKhachHang = new DataTable();

            try
            {
                // Dùng lại mẫu: using (SqlConnection connection = GetConnection())
                using (SqlConnection connection = GetConnection())
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // 1. Thêm tham số đầu vào
                    command.Parameters.Add("@SoDienThoai", SqlDbType.VarChar, 15).Value = soDienThoai;

                    connection.Open();

                    // 2. Sử dụng SqlDataAdapter để điền dữ liệu
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dtKhachHang);
                    }
                }
            }
            catch (SqlException ex)
            {
                // Xử lý lỗi (ví dụ: lỗi kiểm tra độ dài/ký tự số từ RAISERROR trong SP)
                Console.WriteLine($"Lỗi tìm kiếm khách hàng: {ex.Message}");
                throw;
            }

            return dtKhachHang;
        }
        public int TaoDonHangVaChiTiet(int khachHangID,DataTable danhSachXe, int? maNV = null,string maUuDai = null,string ghiChu = null)
        {
            string storedProcedureName = "TaoDonHangVaChiTiet";
            int maDonHangMoi = 0;

            try
            {
                using (SqlConnection connection = GetConnection())
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Thêm các tham số cơ bản
                    command.Parameters.AddWithValue("@KhachHangID", khachHangID);
                    command.Parameters.AddWithValue("@MaNV", maNV ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MaUuDai", maUuDai ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@GhiChu", ghiChu ?? (object)DBNull.Value);
                    // @NgayGiaoDich được bỏ qua vì SP xử lý NULL

                    // Thêm tham số Biến Bảng (TVP)
                    SqlParameter tvpParam = command.Parameters.AddWithValue("@DanhSachXe", danhSachXe);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    // Tên của kiểu dữ liệu bảng SQL
                    tvpParam.TypeName = "ChiTietDonHangType";

                    connection.Open();

                    // Lấy MaDonHang mới (SP sử dụng SELECT)
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        maDonHangMoi = Convert.ToInt32(result);
                        return maDonHangMoi;
                    }
                    else
                    {
                        throw new InvalidOperationException("Giao dịch không thành công.");
                    }
                }
            }
            catch (SqlException ex)
            {
                // Bắt lỗi RAISERROR hoặc Transaction Rollback
                throw new Exception($"Lỗi tạo đơn hàng: {ex.Message}", ex);
            }
        }
        public static async Task<int> TaoDonHangMoiAsync(
        int khachHangID,
        DataTable danhSachXe, // Dùng DataTable rất phù hợp
        string phuongThucThanhToan,
        int? maNV,
        string maUuDai,
        string ghiChu)
        {
            string storedProcedureName = "TaoDonHangVaChiTiet";
            int maDonHangMoi = 0;

            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
            {
                try
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // 1. Thêm các tham số cơ bản
                    command.Parameters.AddWithValue("@KhachHangID", khachHangID);
                    command.Parameters.AddWithValue("@PhuongThucThanhToan", phuongThucThanhToan);
                    command.Parameters.AddWithValue("@MaNV", (object)maNV ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MaUuDai", string.IsNullOrEmpty(maUuDai) ? (object)DBNull.Value : maUuDai);
                    command.Parameters.AddWithValue("@GhiChu", string.IsNullOrEmpty(ghiChu) ? (object)DBNull.Value : ghiChu);
                   

                    // 2. Thêm tham số Biến Bảng (Table-Valued Parameter)
                    SqlParameter tvpParam = command.Parameters.AddWithValue("@DanhSachXe", danhSachXe);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    
                    tvpParam.TypeName = "dbo.ChiTietDonHangType";

                    await connection.OpenAsync();

                    // 3. Lấy MaDonHang mới mà Stored Procedure trả về
                    object result = await command.ExecuteScalarAsync();

                    if (result != null && result != DBNull.Value)
                    {
                        maDonHangMoi = Convert.ToInt32(result);
                        return maDonHangMoi;
                    }
                    else
                    {
                        throw new InvalidOperationException("Không nhận được mã đơn hàng trả về từ cơ sở dữ liệu.");
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception($"Lỗi khi tạo đơn hàng: {ex.Message}", ex);
                }
            }
        }
        public static async Task<object> ExecuteScalarAsync(string procedureName, Dictionary<string, object> parameters)
        {
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    }
                }
                await conn.OpenAsync();
                return await cmd.ExecuteScalarAsync();
            }
        }
    }
}
