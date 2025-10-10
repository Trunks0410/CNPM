using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
       
    }
}
