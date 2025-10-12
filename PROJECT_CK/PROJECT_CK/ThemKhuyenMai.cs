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
            if (update)
            {
                if (cbbLoaiUudai.SelectedIndex == 0)
                {
                    loaiUD = "TRUCTIEP";
                }
                else if (cbbLoaiUudai.SelectedIndex == 1)
                {
                    loaiUD = "PHANTRAM";
                }
                decimal giaTri = decimal.Parse(txtGiatri.Text);
                QuanLyBanXe.UpdateUuDai(txtMaUD.Text, txtTenUD.Text, loaiUD, giaTri, dtTuNgay.Value, dtDenNgay.Value, txtMota.Text);
            }
            else
            {
                decimal giaTri = decimal.Parse(txtGiatri.Text);
                QuanLyBanXe.InsertUuDai(txtMaUD.Text, txtTenUD.Text, loaiUD, giaTri, dtTuNgay.Value, dtDenNgay.Value, txtMota.Text);
            }
            this.DataChanged = true;
            this.Close();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
