using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROJECT_CK
{
    public partial class Them_Sua_NV : Form
    {
        private int maNhanVienHienTai = 0;
        private QuanLyNhanVien qlnv = new QuanLyNhanVien();

        public Them_Sua_NV(int maNV)
        {
            InitializeComponent();
            maNhanVienHienTai = maNV;
            PopulateComboBoxes();
            LoadInitialData();
        }

        private void PopulateComboBoxes()
        {
            // 1. ComboBox Chức vụ (cbChucVu)
            cbChucVu.Items.Clear();
            cbChucVu.Items.Add("Chủ cửa hàng");
            cbChucVu.Items.Add("Nhân viên bán hàng");
            cbChucVu.Items.Add("Kỹ thuật viên");
            cbChucVu.Items.Add("Nhân viên kho");
            cbChucVu.Items.Add("Nhân viên chăm sóc khách hàng");
            if (cbChucVu.Items.Count > 0)
            {
                cbChucVu.SelectedIndex = 0;
            }

            // 2. ComboBox Vai trò (cbVaiTro)
            cbVaiTro.Items.Clear();
            cbVaiTro.Items.Add("Quản trị viên");
            cbVaiTro.Items.Add("Nhân viên");
            if (cbVaiTro.Items.Count > 0)
            {
                cbVaiTro.SelectedIndex = 1; // Mặc định chọn Nhân viên
            }
        }

        private void LoadInitialData()
        {
            if (maNhanVienHienTai > 0) // Chế độ Sửa
            {
                this.Text = "Sửa thông tin nhân viên";
                btnLuuThongTin.Text = "CẬP NHẬT";
                LoadNhanVienData(maNhanVienHienTai);

                txtMatKhau.UseSystemPasswordChar = true;
                cbAnHienMatKhau.Checked = false;
                cbAnHienMatKhau.Enabled = true; 
            }
            else // Chế độ Thêm mới
            {
                this.Text = "Thêm thông tin nhân viên";
                btnLuuThongTin.Text = "LƯU THÔNG TIN";

                txtTenTaiKhoan.ReadOnly = false;
                cbVaiTro.Enabled = true;

                string matKhauNgauNhien = TaoMatKhauNgauNhien();
                txtMatKhau.Text = matKhauNgauNhien;
                txtMatKhau.ReadOnly = true;

                cbAnHienMatKhau.Checked = true;  
                cbAnHienMatKhau.Enabled = false; 

                cbAnHienMatKhau_CheckedChanged(null, null);
            }
        }

        // Hàm để tạo một chuỗi mật khẩu ngẫu nhiên
        private string TaoMatKhauNgauNhien(int length = 8)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(validChars[rnd.Next(validChars.Length)]);
            }
            return res.ToString();
        }

        private void LoadNhanVienData(int maNV)
        {
            DataRow drNhanVien = qlnv.GetNhanVienById(maNV);
            if (drNhanVien != null)
            {
                txtHoTen.Text = drNhanVien["HoTenNV"].ToString();
                dtpNgaySinh.Value = Convert.ToDateTime(drNhanVien["NgaySinh"]);
                if (drNhanVien["GioiTinh"].ToString() == "Nam") rdoNam.Checked = true;
                else rdoNu.Checked = true;
                txtDiaChi.Text = drNhanVien["DiaChi"].ToString();
                txtCCCD.Text = drNhanVien["CCCD"].ToString();

                txtSoDT.Text = drNhanVien["SoDT"].ToString();
                txtEmail.Text = drNhanVien["Email"].ToString();

                cbChucVu.Text = drNhanVien["ChucVu"].ToString();
                dtpNgayNhanViec.Value = Convert.ToDateTime(drNhanVien["NgayNhanViec"]);
                txtLuongCoBan.Text = drNhanVien["LuongCB"].ToString();

                DataRow drTaiKhoan = qlnv.GetTaiKhoanByMaNV(maNV);
                if (drTaiKhoan != null)
                {
                    txtTenTaiKhoan.Text = drTaiKhoan["TenTK"].ToString();
                    txtMatKhau.Text = drTaiKhoan["MatKhau"].ToString();
                    cbVaiTro.Text = drTaiKhoan["VaiTro"].ToString();
                }
                else
                {
                    txtTenTaiKhoan.Text = "(chưa có)";
                    txtMatKhau.Text = "";
                    cbVaiTro.SelectedIndex = -1; 
                }

                txtTenTaiKhoan.ReadOnly = true;
                txtMatKhau.ReadOnly = true;
            }
        }

        private void btnLuuThongTin_Click(object sender, EventArgs e)
        {
            string hoTen = txtHoTen.Text.Trim();
            DateTime ngaySinh = dtpNgaySinh.Value;
            string gioiTinh = rdoNam.Checked ? "Nam" : "Nữ";
            string diaChi = txtDiaChi.Text.Trim();
            string cccd = txtCCCD.Text.Trim();
            string soDT = txtSoDT.Text.Trim();
            string email = txtEmail.Text.Trim();
            string chucVu = cbChucVu.Text;
            DateTime ngayNhanViec = dtpNgayNhanViec.Value;
            decimal luongCB;
            string hinhAnh = ""; 

            if (string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(chucVu))
            {
                MessageBox.Show("Vui lòng điền đầy đủ các thông tin bắt buộc.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(txtLuongCoBan.Text, out luongCB))
            {
                MessageBox.Show("Lương cơ bản không hợp lệ.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (maNhanVienHienTai == 0) 
                {
                    string tenTK = txtTenTaiKhoan.Text.Trim();
                    string matKhau = txtMatKhau.Text; 
                    string vaiTro = cbVaiTro.Text;

                    if (string.IsNullOrWhiteSpace(tenTK) || string.IsNullOrWhiteSpace(matKhau) || string.IsNullOrWhiteSpace(vaiTro))
                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ Tên tài khoản, Mật khẩu và Vai trò khi thêm nhân viên mới.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    qlnv.InsertNhanVien(hoTen, ngaySinh, gioiTinh, soDT, email, diaChi, cccd,
                                        chucVu, ngayNhanViec, luongCB, hinhAnh,
                                        tenTK, matKhau, vaiTro);

                    try
                    {
                        qlnv.GuiEmailMatKhau(email, hoTen, tenTK, matKhau);
                        MessageBox.Show("Thêm nhân viên thành công! Mật khẩu đã được gửi đến email của nhân viên.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception exMail)
                    {
                        // Nếu gửi mail thất bại, vẫn thông báo thêm thành công nhưng cảnh báo về việc gửi email
                        MessageBox.Show($"Thêm nhân viên thành công, nhưng không thể gửi email: {exMail.Message}", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else // Trường hợp: Cập nhật nhân viên
                {
                    qlnv.UpdateNhanVien(maNhanVienHienTai, hoTen, ngaySinh, gioiTinh, soDT, email, diaChi, cccd,
                                        chucVu, ngayNhanViec, luongCB, hinhAnh);

                    // Cập nhật thông tin tài khoản (Mật khẩu và Vai trò)
                    string matKhauMoi = txtMatKhau.Text;
                    string vaiTroMoi = cbVaiTro.Text;
                    if (!string.IsNullOrWhiteSpace(matKhauMoi) && !string.IsNullOrWhiteSpace(vaiTroMoi))
                    {
                        qlnv.UpdateTaiKhoan(maNhanVienHienTai, matKhauMoi, vaiTroMoi);
                    }

                    MessageBox.Show("Cập nhật thông tin nhân viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // 4. Đóng form sau khi hoàn tất
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void cbAnHienMatKhau_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAnHienMatKhau.Checked)
            {
                txtMatKhau.UseSystemPasswordChar = false;
            }
            else
            {
                txtMatKhau.UseSystemPasswordChar = true;
            }
        }

    }
}