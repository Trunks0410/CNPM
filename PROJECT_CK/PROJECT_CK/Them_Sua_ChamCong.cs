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
    public partial class Them_Sua_ChamCong : Form
    {
        private int maChamCongHienTai = 0;
        private QuanLyNhanVien qlnv = new QuanLyNhanVien();
        public Them_Sua_ChamCong(int maCC)
        {
            InitializeComponent();
            maChamCongHienTai = maCC;
            LoadInitialData();
        }
        private void LoadInitialData()
        {
            if (maChamCongHienTai > 0)
            {
                this.Text = "Sửa chấm công";
                btnLuuChamCong.Text = "CẬP NHẬT";
                LoadChamCongData(maChamCongHienTai);
            }
            else
            {
                this.Text = "Thêm chấm công";
                btnLuuChamCong.Text = "LƯU CHẤM CÔNG";
            }
        }

        private void LoadChamCongData(int maCC)
        {
            DataRow dr = qlnv.GetChamCongById(maCC);
            if (dr != null)
            {
                txtMaNV.Text = dr["MaNV"].ToString();
                dtpNgayLamViec.Value = Convert.ToDateTime(dr["NgayLamViec"]);
                dtpThoiGianVaoLam.Value = DateTime.Today.Add((TimeSpan)dr["TgVaoLam"]); 
                dtpThoiGianTanCa.Value = DateTime.Today.Add((TimeSpan)dr["TgTanCa"]); 

                // Xử lý RadioButton Trang thái
                string trangThai = dr["TrangThai"].ToString();
                if (trangThai == "Có mặt") rdoCoMat.Checked = true;
                else if (trangThai == "Vắng") rdoVang.Checked = true;
                else if (trangThai == "Nghỉ phép")
                {
                    rdoNghiPhep.Checked = true;
                    txtLyDoNghiPhep.Text = dr["LyDoNghiPhep"].ToString();
                }
            }
        }

        private void btnLuuChamCong_Click(object sender, EventArgs e)
        {
            // 1. Lấy dữ liệu
            int maNV;
            if (!int.TryParse(txtMaNV.Text, out maNV))
            {
                MessageBox.Show("Mã NV không hợp lệ.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime ngayLamViec = dtpNgayLamViec.Value;
            TimeSpan thoiGianVaoLam = dtpThoiGianVaoLam.Value.TimeOfDay;
            TimeSpan thoiGianTanCa = dtpThoiGianTanCa.Value.TimeOfDay;

            string trangThai = "";
            if (rdoCoMat.Checked) trangThai = "Có mặt";
            else if (rdoVang.Checked) trangThai = "Vắng";
            else if (rdoNghiPhep.Checked) trangThai = "Nghỉ phép";

            string lyDo = rdoNghiPhep.Checked ? txtLyDoNghiPhep.Text : null;

            try
            {
                if (maChamCongHienTai == 0) // Thêm mới
                {
                    qlnv.InsertChamCong(maNV, ngayLamViec, thoiGianVaoLam, thoiGianTanCa, trangThai, lyDo);
                    MessageBox.Show("Thêm chấm công thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else // Cập nhật
                {
                    qlnv.UpdateChamCong(maChamCongHienTai, maNV, ngayLamViec, thoiGianVaoLam, thoiGianTanCa, trangThai, lyDo);
                    MessageBox.Show("Cập nhật chấm công thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi Lưu chấm công: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
