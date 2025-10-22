using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PROJECT_CK
{
    public partial class QuenMatKhau : Form
    {
        private string connectionString;
        private Guna.UI2.WinForms.Guna2TextBox txtMatKhauMoi;
        private Guna.UI2.WinForms.Guna2TextBox txtNhapLaiMatKhau;
        private Label lblMatKhauMoi;
        private Label lblNhapLai;
        private Guna.UI2.WinForms.Guna2Button btnCapNhat;
        private Panel panelMatKhauMoi;

        public QuenMatKhau(string connStr)
        {
            InitializeComponent();
            connectionString = connStr;

            // Đổi label và textbox hiện có cho Tên tài khoản
            label8.Text = "Tài khoản";
            guna2TextBox2.PlaceholderText = "Nhập tên tài khoản...";
            btn_TimKiemQLNX.Text = "Kiểm tra";
            label1.Text = "Nếu tài khoản tồn tại, bạn có thể đặt mật khẩu mới.";

            // Thêm panel cho phần mật khẩu mới
            InitializeAdditionalControls();

            // Gắn sự kiện cho button kiểm tra
            btn_TimKiemQLNX.Click += btn_TimKiemQLNX_Click;
        }

        private void InitializeAdditionalControls()
        {
            // Panel chứa phần nhập mật khẩu mới
            panelMatKhauMoi = new Panel();
            panelMatKhauMoi.Location = new Point(12, 230);
            panelMatKhauMoi.Size = new Size(645, 240);
            panelMatKhauMoi.Visible = false;
            this.Controls.Add(panelMatKhauMoi);

            // Label "Mật khẩu mới"
            lblMatKhauMoi = new Label();
            lblMatKhauMoi.Text = "Mật khẩu mới";
            lblMatKhauMoi.Location = new Point(0, 0);
            lblMatKhauMoi.Font = new Font("Segoe UI Black", 14F, FontStyle.Bold);
            lblMatKhauMoi.AutoSize = true;
            panelMatKhauMoi.Controls.Add(lblMatKhauMoi);

            // TextBox mật khẩu mới
            txtMatKhauMoi = new Guna.UI2.WinForms.Guna2TextBox();
            txtMatKhauMoi.Location = new Point(0, 35);
            txtMatKhauMoi.Size = new Size(634, 45);
            txtMatKhauMoi.PlaceholderText = "Nhập mật khẩu mới...";
            txtMatKhauMoi.UseSystemPasswordChar = true;
            panelMatKhauMoi.Controls.Add(txtMatKhauMoi);

            // Label "Nhập lại mật khẩu"
            lblNhapLai = new Label();
            lblNhapLai.Text = "Nhập lại mật khẩu";
            lblNhapLai.Location = new Point(0, 90);
            lblNhapLai.Font = new Font("Segoe UI Black", 14F, FontStyle.Bold);
            lblNhapLai.AutoSize = true;
            panelMatKhauMoi.Controls.Add(lblNhapLai);

            // TextBox nhập lại mật khẩu
            txtNhapLaiMatKhau = new Guna.UI2.WinForms.Guna2TextBox();
            txtNhapLaiMatKhau.Location = new Point(0, 125);
            txtNhapLaiMatKhau.Size = new Size(634, 45);
            txtNhapLaiMatKhau.PlaceholderText = "Nhập lại mật khẩu mới...";
            txtNhapLaiMatKhau.UseSystemPasswordChar = true;
            panelMatKhauMoi.Controls.Add(txtNhapLaiMatKhau);

            // Button cập nhật
            btnCapNhat = new Guna.UI2.WinForms.Guna2Button();
            btnCapNhat.Text = "Cập nhật mật khẩu";
            btnCapNhat.Location = new Point(0, 185);
            btnCapNhat.Size = new Size(280, 48);
            btnCapNhat.FillColor = Color.ForestGreen;
            btnCapNhat.ForeColor = Color.White;
            btnCapNhat.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            btnCapNhat.BorderRadius = 15;
            btnCapNhat.BorderThickness = 2;
            btnCapNhat.BorderColor = Color.Gold;
            btnCapNhat.Click += btnCapNhat_Click;
            panelMatKhauMoi.Controls.Add(btnCapNhat);
        }

        private void btn_TimKiemQLNX_Click(object sender, EventArgs e)
        {
            string tenTK = guna2TextBox2.Text.Trim();

            if (string.IsNullOrEmpty(tenTK))
            {
                MessageBox.Show("Vui lòng nhập tên tài khoản!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM TaiKhoan WHERE TenTK = @TenTK";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TenTK", tenTK);
                        int count = (int)cmd.ExecuteScalar();

                        if (count > 0)
                        {
                            // Hiển thị panel mật khẩu mới
                            panelMatKhauMoi.Visible = true;
                            btn_TimKiemQLNX.Enabled = false;
                            guna2TextBox2.Enabled = false;
                            label1.Text = "Tài khoản tồn tại. Hãy nhập mật khẩu mới.";
                            this.Height = 520; // Mở rộng form để hiển thị panel
                        }
                        else
                        {
                            MessageBox.Show("Tài khoản không tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối database: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            string tenTK = guna2TextBox2.Text.Trim();
            string matKhauMoi = txtMatKhauMoi.Text.Trim();
            string nhapLai = txtNhapLaiMatKhau.Text.Trim();

            if (string.IsNullOrEmpty(matKhauMoi) || string.IsNullOrEmpty(nhapLai))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ mật khẩu mới và xác nhận!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (matKhauMoi != nhapLai)
            {
                MessageBox.Show("Mật khẩu mới và xác nhận không khớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE TaiKhoan SET MatKhau = @MatKhau WHERE TenTK = @TenTK";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatKhau", matKhauMoi);
                        cmd.Parameters.AddWithValue("@TenTK", tenTK);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Cập nhật mật khẩu thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Cập nhật thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối database: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
