using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guna.UI2.WinForms;
using System.Windows.Forms;

namespace PROJECT_CK
{
    public class ProductPanel
    {
        // Cấu hình cố định cho panel
        private static readonly Size PanelSize = new Size(250, 260);
        private static readonly Color PanelBackColor = Color.LightBlue;

        // Cấu hình PictureBox
        private static readonly Size PictureBoxSize = new Size(240, 190);
        private static readonly Point PictureBoxDefaultLocation = new Point(5, 40);
        private static readonly PictureBoxSizeMode PictureBoxSizeMode = PictureBoxSizeMode.Zoom;

        // Cấu hình Label Ten
        private static readonly Font LabelTenFont = new Font("Segoe UI", 16, FontStyle.Bold);
        private static readonly Point LabelTenDefaultLocation = new Point(20, 0);
        private static readonly Color LabelTenForeColor = Color.Black;

        // Cấu hình Label Gia
        private static readonly Font LabelGiaFont = new Font("Segoe UI", 13, FontStyle.Italic);
        private static readonly Color LabelGiaForeColor = Color.Red;
        private static readonly Point LabelGiaDefaultLocation = new Point(10, 230);

        private static readonly Font LabelHetHangFont = new Font("Segoe UI", 25, FontStyle.Bold); // TĂNG CỠ CHỮ
        private static readonly Color LabelHetHangForeColor = Color.Red;
        private static readonly Color LabelHetHangBackColor = Color.FromArgb(150, Color.Black);

        public static Guna2Panel CreateProductPanel(Product product, int posx, int posy, EventHandler clickHandler = null)
        {
            // Tạo panel với vị trí động, không viền
            Guna2Panel panel = new Guna2Panel
            {
                Size = PanelSize,
                BackColor = PanelBackColor,
                Location = new Point(posx, posy),
                BorderRadius = 0,
                BorderThickness = 0
            };

            // Tạo PictureBox (hình ảnh làm nền)
            Guna2PictureBox pictureBox = new Guna2PictureBox
            {
                Size = PictureBoxSize,
                Location = PictureBoxDefaultLocation,
                SizeMode = PictureBoxSizeMode
            };
            if (!string.IsNullOrEmpty(product.ImagePath) && System.IO.File.Exists(product.ImagePath))
            {
                pictureBox.Image = Image.FromFile(product.ImagePath);
            }

            // Tạo Label Ten (nằm trên hình)
            Guna2HtmlLabel lblTen = new Guna2HtmlLabel
            {
                Text = product.Ten ?? "Chưa có tên",
                Font = LabelTenFont,
                ForeColor = LabelTenForeColor,
                Location = LabelTenDefaultLocation,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // Tạo Label Gia (nằm trên hình)
            Guna2HtmlLabel lblGia = new Guna2HtmlLabel
            {
                Text = "Giá bán: " + (product.Gia ?? "Chưa có giá"),
                Font = LabelGiaFont,
                ForeColor = LabelGiaForeColor,
                Location = LabelGiaDefaultLocation,
                AutoSize = true,
                BackColor = Color.Transparent
            };


            if (product.SoLuong == 0)
            {
                Guna2HtmlLabel lblHetHang = new Guna2HtmlLabel
                {
                    Text = "HẾT HÀNG",
                    Font = LabelHetHangFont,
                    ForeColor = LabelHetHangForeColor,
                    BackColor = LabelHetHangBackColor, // Nền đen mờ
                    TextAlignment = ContentAlignment.MiddleCenter,  // Canh giữa chữ
                    Dock = DockStyle.Fill, // Lấp đầy PictureBox
                    Location = new Point(0, 0), // Vị trí trong PictureBox
                    Size = PictureBoxSize, // Kích thước bằng PictureBox
                    Visible = true // Đảm bảo hiển thị
                };
                // Đặt Label "HẾT HÀNG" trên PictureBox
                pictureBox.Controls.Add(lblHetHang);
                // Đảm bảo Label luôn nằm trên cùng
                lblHetHang.BringToFront();

                // Vô hiệu hóa panel và các control con để không cho click
                panel.Enabled = false;
            }


            // Gắn sự kiện Click cho panel và các thành phần con
            if (clickHandler != null)
            {
                // Gắn sự kiện Click cho panel
                panel.Click += clickHandler;

                // Gắn sự kiện Click cho các thành phần con
                pictureBox.Click += clickHandler;
                lblTen.Click += clickHandler;
                lblGia.Click += clickHandler;

                // Lưu thông tin sản phẩm vào Tag của panel và các thành phần con
                panel.Tag = product;
                pictureBox.Tag = product;
                lblTen.Tag = product;
                lblGia.Tag = product;
            }

            // Thêm các điều khiển vào panel
            panel.Controls.Add(pictureBox);
            panel.Controls.Add(lblTen);
            panel.Controls.Add(lblGia);



            return panel;
        }
    }
}
