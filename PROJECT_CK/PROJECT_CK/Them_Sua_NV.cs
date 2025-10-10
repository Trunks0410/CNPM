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
            // Gọi hàm điền dữ liệu cho ComboBox
            PopulateComboBoxes();
            LoadInitialData();
            btnLuuThongTin.Click += btnLuuThongTin_Click;

            txtMatKhau.UseSystemPasswordChar = true;
            cbAnHienMatKhau.Checked = false; 
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
            if (maNhanVienHienTai > 0)
            {
                this.Text = "Sửa thông tin nhân viên";
                btnLuuThongTin.Text = "CẬP NHẬT";
                LoadNhanVienData(maNhanVienHienTai);
            }
            else
            {
                this.Text = "Thêm thông tin nhân viên";
                btnLuuThongTin.Text = "LƯU THÔNG TIN";
                // Khi thêm mới, cho phép nhập thông tin tài khoản
                txtTenTaiKhoan.ReadOnly = false;
                txtMatKhau.ReadOnly = false;
                cbVaiTro.Enabled = true;
            }
        }

        private void LoadNhanVienData(int maNV)
        {
            DataRow dr = qlnv.GetNhanVienById(maNV);
            if (dr != null)
            {
                // Thông tin cá nhân
                txtHoTen.Text = dr["HoTenNV"].ToString();
                dtpNgaySinh.Value = Convert.ToDateTime(dr["NgaySinh"]);
                if (dr["GioiTinh"].ToString() == "Nam") rdoNam.Checked = true;
                else rdoNu.Checked = true;
                txtDiaChi.Text = dr["DiaChi"].ToString();
                txtCCCD.Text = dr["CCCD"].ToString();

                // Thông tin liên hệ
                txtSoDT.Text = dr["SoDT"].ToString();
                txtEmail.Text = dr["Email"].ToString();

                // Thông tin công việc
                // Gán giá trị Chức vụ đã đọc từ DB vào ComboBox
                cbChucVu.Text = dr["ChucVu"].ToString();
                dtpNgayNhanViec.Value = Convert.ToDateTime(dr["NgayNhanViec"]);
                txtLuongCoBan.Text = dr["LuongCB"].ToString();

                txtTenTaiKhoan.ReadOnly = true;
                txtMatKhau.ReadOnly = true;
                cbVaiTro.Enabled = false;
            }
        }

        private void btnLuuThongTin_Click(object sender, EventArgs e)
        {
            // 1. Lấy dữ liệu và kiểm tra nhập liệu cơ bản
            string hoTen = txtHoTen.Text;
            DateTime ngaySinh = dtpNgaySinh.Value;
            string gioiTinh = rdoNam.Checked ? "Nam" : "Nữ";
            string diaChi = txtDiaChi.Text;
            string cccd = txtCCCD.Text;
            string soDT = txtSoDT.Text;
            string email = txtEmail.Text;
            string chucVu = cbChucVu.Text;
            DateTime ngayNhanViec = dtpNgayNhanViec.Value;
            decimal luongCB;

            // Kiểm tra Lương
            if (!decimal.TryParse(txtLuongCoBan.Text, out luongCB))
            {
                MessageBox.Show("Lương cơ bản không hợp lệ.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string hinhAnh = ""; // Giả định

            // 2. Xử lý Tài khoản (chỉ khi Thêm mới)
            string tenTK = maNhanVienHienTai == 0 ? txtTenTaiKhoan.Text : null;
            string matKhau = maNhanVienHienTai == 0 ? txtMatKhau.Text : null;
            string vaiTro = maNhanVienHienTai == 0 ? cbVaiTro.Text : null;

            // Kiểm tra tài khoản khi Thêm mới
            if (maNhanVienHienTai == 0 && (string.IsNullOrEmpty(tenTK) || string.IsNullOrEmpty(matKhau) || string.IsNullOrEmpty(vaiTro)))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên tài khoản, Mật khẩu và Vai trò khi thêm nhân viên mới.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (maNhanVienHienTai == 0) // Thêm mới
                {
                    qlnv.InsertNhanVien(hoTen, ngaySinh, gioiTinh, soDT, email, diaChi, cccd,
                                        chucVu, ngayNhanViec, luongCB, hinhAnh,
                                        tenTK, matKhau, vaiTro);
                    MessageBox.Show("Thêm nhân viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else // Cập nhật
                {
                    qlnv.UpdateNhanVien(maNhanVienHienTai, hoTen, ngaySinh, gioiTinh, soDT, email, diaChi, cccd,
                                        chucVu, ngayNhanViec, luongCB, hinhAnh);
                    MessageBox.Show("Cập nhật nhân viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi Lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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