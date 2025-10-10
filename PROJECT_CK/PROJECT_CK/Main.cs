using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace PROJECT_CK
{
    public partial class Main : Form
    {
        public List<Product> Xemay = new List<Product>();
        public static System.Globalization.CultureInfo cultureInfoVN = new System.Globalization.CultureInfo("vi-VN");
        private readonly Panel_Click panelClickHandler;
        public Main()
        {
            InitializeComponent();
            panelClickHandler = new Panel_Click(this);
        }

        private void Main_Load(object sender, EventArgs e)
        {
           
        }
        public class Panel_Click
        {
            public readonly Form formBanHang;

            public Panel_Click(Form formBanHang)
            {
                this.formBanHang = formBanHang;
            }

            public void XuLySuKienClick(object sender, EventArgs e)
            {
                //if (sender is Control control && control.Tag is Product product)
                //{
                //    frmXemay chiTietForm = new frmXemay(product);
                //    chiTietForm.ShowDialog();
                //}
                //else
                //{
                //    MessageBox.Show("Không tìm thấy thông tin sản phẩm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}
            }
        }
        public void LoadProductsToTab(string panelName, List<Product> list)
        {
            Control[] foundControls = this.Controls.Find(panelName, true);
            Guna.UI2.WinForms.Guna2Panel tabPanel = foundControls
                                               .OfType<Guna.UI2.WinForms.Guna2Panel>()
                                               .FirstOrDefault();
            if (tabPanel == null)
            {
                MessageBox.Show($"Lỗi: Không tìm thấy Guna2Panel có tên '{panelName}'.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            tabPanel.Controls.Clear();
            int x = 20, y = 20;
            int col = 0;
            foreach (var sp in list)
            {
                // Tạo panel sản phẩm
                Guna2Panel panelSp = ProductPanel.CreateProductPanel(sp, x, y, panelClickHandler.XuLySuKienClick);
                tabPanel.Controls.Add(panelSp);
                // Sắp xếp dạng lưới 2 cột
                col++;
                if (col % 7 == 0)
                {
                    x = 20;
                    y += panelSp.Height + 20;
                }
                else
                {
                    x += panelSp.Width + 20;
                }
            }
        }
        public void LoadXemay()
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

        private void tabControlMain_Selected(object sender, TabControlEventArgs e)
        {
            if(tabControlMain.TabPages[e.TabPageIndex].Name == "tabPageDSX")
            {
                Xemay.Clear();
                LoadXemay();
                LoadProductsToTab("panelDanhMucXe", Xemay);
            }
        }
    }
}
