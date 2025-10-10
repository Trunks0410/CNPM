using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROJECT_CK
{
    public class Banhang
    {
        public static System.Globalization.CultureInfo cultureInfoVN = new System.Globalization.CultureInfo("vi-VN");
        public static void LoadXemay(ref List<Product> Xemay)
        {
            string connectionString = ConnectDB.Connect();
            string sqlQuery = "SELECT * FROM dbo.LoadXe()";
            DataTable dtXe = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dtXe);
                    }
                }
                foreach (DataRow row in dtXe.Rows)
                {
                    // Thực hiện chuyển đổi từng cột sang kiểu dữ liệu tương ứng trong Sanpham
                    Product sp = new Product
                    (
                        row["XeID"].ToString(),
                        row["TenXe"].ToString(),
                    // Đảm bảo xử lý lỗi chuyển đổi (Convert.ToDecimal/Convert.ToInt32)
                        string.Format(cultureInfoVN, "{0:N0}", Convert.ToDecimal(row["GiaBan"])),
                        Convert.ToInt32(row["soluong"]),
                     
                        row["MoTa"].ToString(),
                        row["HinhAnh"].ToString()

                    );
                    Xemay.Add(sp);
                }
            }
            catch (SqlException ex)
            {
                // Hiển thị lỗi, ví dụ: nếu lỗi thiếu quyền EXECUTE lại xảy ra
                MessageBox.Show($"Lỗi tải dữ liệu xe: {ex.Message}", "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
