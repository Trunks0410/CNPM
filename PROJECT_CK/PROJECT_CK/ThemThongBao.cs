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
    public partial class ThemThongBao : Form
    {
        private QuanLyNhanVien qlnv = new QuanLyNhanVien();
        public ThemThongBao()
        {
            InitializeComponent();
            this.Text = "Thêm thông báo";
            PopulateLoaiThongBao();
        }
        private void PopulateLoaiThongBao()
        {
            cbLoaiThongBao.Items.Clear();

            cbLoaiThongBao.Items.Add("Thông báo chung");
            cbLoaiThongBao.Items.Add("Thông báo quan trọng");
            cbLoaiThongBao.Items.Add("Quy định mới");
            if (cbLoaiThongBao.Items.Count > 0)
            {
                cbLoaiThongBao.SelectedIndex = 0;
            }
        }
        private void btnThemThongBao_Click(object sender, EventArgs e)
        {
            string tieuDe = txtTieuDeThongBao.Text;
            string loaiThongBao = cbLoaiThongBao.SelectedItem?.ToString();
            string noiDung = txtNoiDung.Text;

            int? maNV = null; // Thông báo chung

            if (string.IsNullOrEmpty(tieuDe) || string.IsNullOrEmpty(loaiThongBao) || string.IsNullOrEmpty(noiDung))
            {
                MessageBox.Show("Vui lòng điền đầy đủ các trường bắt buộc.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Gọi hàm InsertThongBao trong QuanLyNhanVien.cs
                qlnv.InsertThongBao(tieuDe, loaiThongBao, noiDung, maNV);
                MessageBox.Show("Thêm thông báo thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi Thêm thông báo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
