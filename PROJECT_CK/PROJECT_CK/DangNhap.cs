using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PROJECT_CK
{
    public partial class DangNhap : Form
    {
        // Chuỗi kết nối database (thay bằng chuỗi thực tế của bạn)
        private string connectionString = @"Server=localhost;Database=QuanLyXeMuaBanXeMay;Integrated Security=True;";

        public DangNhap()
        {
            InitializeComponent();
            // Thêm event handlers (nếu chưa có trong Designer)
            guna2GradientButton1.Click += guna2GradientButton1_Click; // Nút đăng nhập
            cbAnHienMatKhau.CheckedChanged += cbAnHienMatKhau_CheckedChanged; // Checkbox ẩn/hiện mật khẩu
            linkLabel1.LinkClicked += linkLabel1_LinkClicked; // Link quên mật khẩu
        }

        // Event click nút đăng nhập
        private void guna2GradientButton1_Click(object sender, EventArgs e)
        {
            string tenTK = guna2TextBox1.Text.Trim(); // Tên tài khoản
            string matKhau = guna2TextBox2.Text.Trim(); // Mật khẩu

            if (string.IsNullOrEmpty(tenTK) || string.IsNullOrEmpty(matKhau))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tài khoản và mật khẩu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kết nối và kiểm tra đăng nhập
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT MaTK, MaNV, VaiTro FROM TaiKhoan WHERE TenTK = @TenTK AND MatKhau = @MatKhau";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TenTK", tenTK);
                        cmd.Parameters.AddWithValue("@MatKhau", matKhau); // Nên hash mật khẩu ở production

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Đăng nhập thành công
                                int maTK = reader.GetInt32(0);
                                int? maNV = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1);
                                string vaiTro = reader.GetString(2);

                                MessageBox.Show($"Đăng nhập thành công! Vai trò: {vaiTro}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Mở form chính, truyền username, role và connection string
                                Main mainForm = new Main(tenTK, vaiTro, connectionString);
                                mainForm.Show();
                                this.Hide(); // Ẩn form đăng nhập
                            }
                            else
                            {
                                MessageBox.Show("Tài khoản hoặc mật khẩu không đúng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối database: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Event checkbox ẩn/hiện mật khẩu
        private void cbAnHienMatKhau_CheckedChanged(object sender, EventArgs e)
        {
            guna2TextBox2.UseSystemPasswordChar = !cbAnHienMatKhau.Checked; // Toggle ẩn/hiện mật khẩu
        }

        // Event link quên mật khẩu
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Chức năng quên mật khẩu đang được phát triển. Vui lòng liên hệ admin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}