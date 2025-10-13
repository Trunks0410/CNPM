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
    public partial class ThemKhuyenMai : Form
    {
        public bool DataChanged { get; set; } = false;
        bool update = false;
        public ThemKhuyenMai()
        {
            InitializeComponent();
        }
        public ThemKhuyenMai(string maUD)
        {
            InitializeComponent();
            txtMaUD.Text = maUD;
            update = true;
        }
        private void ThemKhuyenMai_Load(object sender, EventArgs e)
        {
            if(txtMaUD.Text != "")
            {
                btnKhoitao.Enabled = false;
                btnLuu.Enabled = true;
                DataTable dtUuDai = QuanLyBanXe.GetUuDaiByMa(txtMaUD.Text);
                DataRow row = dtUuDai.Rows[0];

                
                
                txtTenUD.Text = row["TenUuDai"].ToString();
                string loaiUuDai = row["LoaiUuDai"].ToString();
                if (loaiUuDai == "TRUCTIEP")
                {
                    cbbLoaiUudai.SelectedIndex = 0;
                }
                else if (loaiUuDai == "PHANTRAM")
                {
                    cbbLoaiUudai.SelectedIndex = 1;
                }
                txtMota.Text = row["MoTa"] == DBNull.Value ? string.Empty : row["MoTa"].ToString();

                // Biến số (Cần Parse hoặc Convert)
                // DECIMAL trong DB tương ứng với decimal trong C#
                txtGiatri.Text = row["GiaTriGiam"].ToString();

                // Biến Ngày tháng (Cần Convert)
                dtTuNgay.Value = Convert.ToDateTime(row["NgayBatDau"]);
                dtDenNgay.Value = Convert.ToDateTime(row["NgayKetThuc"]);
            }
            else
            {
                btnKhoitao.Enabled = false;
                btnLuu.Enabled = false;
            }
        }
        public bool Checkinput()
        {
            if (txtGiatri.Text == "" || txtTenUD.Text == "" || cbbLoaiUudai.SelectedIndex == -1)
            {
                return false;
            }
            return true;
        }

        private void txtTenUD_TextChanged(object sender, EventArgs e)
        {
            if (update)
                return;
            if (Checkinput())
            {
                btnKhoitao.Enabled = true;
            }
            else
            {
                btnKhoitao.Enabled = false;
            }
        }

        private void cbbLoaiUudai_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (update)
                return;
            if (Checkinput())
            {
                btnKhoitao.Enabled = true;
            }
            else
            {
                btnKhoitao.Enabled = false;
            }
        }

        private void txtGiatri_TextChanged(object sender, EventArgs e)
        {
            if (update)
                return;
            if (Checkinput())
            {
                btnKhoitao.Enabled = true;
            }
            else
            {
                btnKhoitao.Enabled = false;
            }
        }
        string loaiUD = "";
        private void btnKhoitao_Click(object sender, EventArgs e)
        {
            if (cbbLoaiUudai.SelectedIndex == 0)
            {
                loaiUD = "TRUCTIEP";
            }
            else if (cbbLoaiUudai.SelectedIndex == 1)
            {
                loaiUD = "PHANTRAM";
            }
            string maUD = QuanLyBanXe.GetNewMaUuDai(loaiUD);
            txtMaUD.Text = maUD;
            btnLuu.Enabled = true;
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
           try
            {
                // --- 1. VALIDATION: Kiểm tra dữ liệu đầu vào trước khi xử lý ---
                if (cbbLoaiUudai.SelectedIndex < 0)
                {
                    MessageBox.Show("Vui lòng chọn loại ưu đãi.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Dừng lại nếu chưa chọn loại
                }

                if (!decimal.TryParse(txtGiatri.Text, out decimal giaTri))
                {
                    MessageBox.Show("Giá trị giảm không hợp lệ. Vui lòng nhập đúng định dạng số.", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // SỬA: Thêm validation cho trường mới 'Điều kiện tối thiểu'
                if (!decimal.TryParse(txtDK.Text, out decimal dieuKienToiThieu))
                {
                    MessageBox.Show("Điều kiện đơn tối thiểu không hợp lệ. Vui lòng nhập đúng định dạng số.", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // --- 2. THU THẬP DỮ LIỆU ---
                string maUD = txtMaUD.Text;
                string tenUD = txtTenUD.Text;
                string loaiUD = (cbbLoaiUudai.SelectedIndex == 0) ? "TRUCTIEP" : "PHANTRAM"; // Cách viết gọn hơn
                DateTime tuNgay = dtTuNgay.Value;
                DateTime denNgay = dtDenNgay.Value;
                string moTa = txtMota.Text;

                // --- 3. THỰC THI HÀNH ĐỘNG ---
                btnLuu.Enabled = false; // Vô hiệu hóa nút để tránh click nhiều lần
                this.Cursor = Cursors.WaitCursor; // Đổi con trỏ chuột báo hiệu đang xử lý

                if (update) // Chế độ Cập nhật
                {
                    // SỬA: Thêm tham số 'dieuKienToiThieu' vào lời gọi hàm
                    QuanLyBanXe.UpdateUuDai(maUD, tenUD, loaiUD, giaTri, dieuKienToiThieu, tuNgay, denNgay, moTa);
                    MessageBox.Show("Cập nhật ưu đãi thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else // Chế độ Thêm mới
                {
                    // SỬA: Thêm tham số 'dieuKienToiThieu' vào lời gọi hàm
                    QuanLyBanXe.InsertUuDai(maUD, tenUD, loaiUD, giaTri, dieuKienToiThieu, tuNgay, denNgay, moTa);
                    MessageBox.Show("Thêm mới ưu đãi thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DataChanged = true; // Báo hiệu dữ liệu đã thay đổi
                this.Close(); // Đóng form sau khi thành công
            }
            catch (Exception ex)
            {
                // Bắt tất cả các lỗi (bao gồm lỗi từ SQL RAISERROR) và hiển thị cho người dùng
                MessageBox.Show(ex.Message, "Đã xảy ra lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Khối này LUÔN LUÔN được thực thi, dù thành công hay thất bại
                btnLuu.Enabled = true; // Kích hoạt lại nút
                this.Cursor = Cursors.Default; // Trả lại con trỏ chuột bình thường
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
