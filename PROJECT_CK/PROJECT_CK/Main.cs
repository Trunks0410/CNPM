using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using QuanLyMuaBanXeMay;

namespace PROJECT_CK
{
    public partial class Main : Form
    {
        private readonly QuanLyNhapXe_Kho quanLyNhapXe_Kho;
        private readonly string _username;
        private readonly string _role;
        private readonly string _connectionString;
        private int currentPhieuNhapID;
        private List<Dictionary<string, string>> tempXeList = new List<Dictionary<string, string>>();
        private DataTable dtXeTonKho;
        private int maPhieuCurrent;
        public Main()
        {
            InitializeComponent();
 
            quanLyNhapXe_Kho = new QuanLyNhapXe_Kho();
            //ApplyRolePermissions();
        }

        private void ApplyRolePermissions()
        {
            if (_role == "RoleAdmin")
            {
            }
            else if (_role == "RoleUser")
            {


            }
            MessageBox.Show($"Chào mừng {_username}! Bạn đang đăng nhập với quyền {_role}.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            //LoadPhieuNhap();
            //LoadThongKe();
            //LoadNhaCungCap();
            //LoadDanhMucComboboxes();
            //cbbNguonNhapChiTiet.SelectedIndex = 0;
            //cbbNguonNhapNX.SelectedIndex = 0;
            //cbbHangXeFilter.SelectedIndex = -1; // Giả sử "Tất cả" là index 0
            //cbbLoaiXeFilter.SelectedIndex = -1;
            //cbbMauSacFilter.SelectedIndex = -1;
            //cbbTinhTrangFilter.SelectedIndex = -1;
            //cbbDonGiaOperator.SelectedIndex = -1;
            //LoadDanhSachXeNhap();
            //LoadBarChart();
            //LoadPieChart();
            //LoadTongQuanTonKho();
            //LoadXeTonKho();
        }

        private void LoadPhieuNhap()
        {
            DataTable dt = quanLyNhapXe_Kho.GetDanhSachPhieuNhap();
            dgvDanhSachPhieuNhap.DataSource = dt;

            dgvDanhSachPhieuNhap.Columns["MaPhieu"].HeaderText = "Mã Phiếu";
            dgvDanhSachPhieuNhap.Columns["NgayNhap"].HeaderText = "Ngày Nhập";
            dgvDanhSachPhieuNhap.Columns["LoaiNhap"].HeaderText = "Loại Nhập";
            dgvDanhSachPhieuNhap.Columns["TongTien"].HeaderText = "Tổng Tiền";

            dgvDanhSachPhieuNhap.Columns["NgayNhap"].DefaultCellStyle.Format = "dd/MM/yyyy";

            if (!dgvDanhSachPhieuNhap.Columns.Contains("LuaChon"))
            {
                DataGridViewButtonColumn thaoTacColumn = new DataGridViewButtonColumn();
                thaoTacColumn.Name = "LuaChon";
                thaoTacColumn.HeaderText = "Thao tác";
                dgvDanhSachPhieuNhap.Columns.Add(thaoTacColumn);
            }
        }

        public void LoadThongKe()
        {
            DataTable dt = quanLyNhapXe_Kho.GetThongKePhieuNhap(null, null, null, null, null);

            if (dt.Rows.Count > 0)
            {
                txtTongSoPhieuNhap.Text = dt.Rows[0]["TongSoPhieu"]?.ToString() ?? "0";
                txtTongSoLuongXeNhap.Text = dt.Rows[0]["TongSoLuongXeNhap"]?.ToString() ?? "0";
                txtTongGiaTriNhap.Text = string.Format("{0:N0} VND", dt.Rows[0]["TongGiaTriNhap"] ?? 0);
            }
            else
            {
                txtTongSoPhieuNhap.Text = "0";
                txtTongSoLuongXeNhap.Text = "0";
                txtTongGiaTriNhap.Text = "0 VND";
            }
        }
        private void LoadNhaCungCap()
        {
            dgvNCC.DataSource = quanLyNhapXe_Kho.GetAllNhaCungCap();
            dgvNCC.Columns["NCCID"].HeaderText = "Nhà cung cấp ID";
            dgvNCC.Columns["TenNCC"].HeaderText = "Tên nhà cung cấp";
            dgvNCC.Columns["DiaChi"].HeaderText = "Địa chỉ";
            dgvNCC.Columns["SoDienThoai"].HeaderText = "Số điện thoại";
        }

        private void LoadDanhMucComboboxes()
        {
            // Hãng xe
            DataTable dtHangXe = quanLyNhapXe_Kho.ExecuteDataTable("sp_GetDanhMucByLoai", CommandType.StoredProcedure,
                new SqlParameter("@LoaiDanhMuc", "HangXe"));
            cboHangXe.DataSource = dtHangXe;
            cboHangXe.DisplayMember = "GiaTri";   // Giá trị hiển thị
            cboHangXe.ValueMember = "DanhMucID";  // Giá trị ẩn ID
            cbbHangXeNX.DataSource = dtHangXe;
            cbbHangXeNX.DisplayMember = "GiaTri";   // Giá trị hiển thị
            cbbHangXeNX.ValueMember = "DanhMucID";
            cbbHangXeFilter.DataSource = dtHangXe;
            cbbHangXeFilter.DisplayMember = "GiaTri";   // Giá trị hiển thị
            cbbHangXeFilter.ValueMember = "DanhMucID";

            // Loại xe
            DataTable dtLoaiXe = quanLyNhapXe_Kho.ExecuteDataTable("sp_GetDanhMucByLoai", CommandType.StoredProcedure,
                new SqlParameter("@LoaiDanhMuc", "LoaiXe"));
            cboLoaiXe.DataSource = dtLoaiXe;
            cboLoaiXe.DisplayMember = "GiaTri";
            cboLoaiXe.ValueMember = "DanhMucID";
            cbbLoaiXeNX.DataSource = dtLoaiXe;
            cbbLoaiXeNX.DisplayMember = "GiaTri";
            cbbLoaiXeNX.ValueMember = "DanhMucID";
            cbbLoaiXeFilter.DataSource = dtLoaiXe;
            cbbLoaiXeFilter.DisplayMember = "GiaTri";
            cbbLoaiXeFilter.ValueMember = "DanhMucID";

            // Màu sắc
            DataTable dtMauSac = quanLyNhapXe_Kho.ExecuteDataTable("sp_GetDanhMucByLoai", CommandType.StoredProcedure,
                new SqlParameter("@LoaiDanhMuc", "MauSac"));
            cbbMauSacCT.DataSource = dtMauSac;
            cbbMauSacCT.DisplayMember = "GiaTri";
            cbbMauSacCT.ValueMember = "DanhMucID";
            cboMauSacNX.DataSource = dtMauSac;
            cboMauSacNX.DisplayMember = "GiaTri";
            cboMauSacNX.ValueMember = "DanhMucID";
            cbbMauSacFilter.DataSource = dtMauSac;
            cbbMauSacFilter.DisplayMember = "GiaTri";
            cbbMauSacFilter.ValueMember = "DanhMucID";

            DataTable dtTinhTrang = quanLyNhapXe_Kho.ExecuteDataTable("sp_GetDanhMucByLoai", CommandType.StoredProcedure,
                new SqlParameter("@LoaiDanhMuc", "TinhTrang"));
            cbbTinhTrangCT.DataSource = dtTinhTrang;
            cbbTinhTrangCT.DisplayMember = "GiaTri";
            cbbTinhTrangCT.ValueMember = "DanhMucID";
            cbbTinhTrangFilter.DataSource = dtTinhTrang;
            cbbTinhTrangFilter.DisplayMember = "GiaTri";
            cbbTinhTrangFilter.ValueMember = "DanhMucID";
        }

        private void LoadDanhSachXeNhap()
        {
            dgvDanhSachChiTietNhapXe.Columns.Clear();
            dgvDanhSachChiTietNhapXe.Columns.Add("HinhAnh", "Hình Ảnh");
            dgvDanhSachChiTietNhapXe.Columns.Add("TenXe", "Tên Xe");
            dgvDanhSachChiTietNhapXe.Columns.Add("HangXe", "Hãng Xe");
            dgvDanhSachChiTietNhapXe.Columns.Add("LoaiXe", "Loại Xe");
            dgvDanhSachChiTietNhapXe.Columns.Add("MauSac", "Màu Sắc");
            dgvDanhSachChiTietNhapXe.Columns.Add("NamSX", "Năm SX");
            dgvDanhSachChiTietNhapXe.Columns.Add("SoKhung", "Số Khung");
            dgvDanhSachChiTietNhapXe.Columns.Add("SoMay", "Số Máy");
            dgvDanhSachChiTietNhapXe.Columns.Add("DungTich", "Dung Tích");
            dgvDanhSachChiTietNhapXe.Columns.Add("TinhTrang", "Tình Trạng");
            dgvDanhSachChiTietNhapXe.Columns.Add("DonGia", "Đơn Giá");
            dgvDanhSachChiTietNhapXe.Columns.Add("MoTa", "Mô Tả");
            // Thiết lập định dạng cho cột Đơn Giá
            if (dgvDanhSachChiTietNhapXe.Columns["DonGia"] != null)
            {
                dgvDanhSachChiTietNhapXe.Columns["DonGia"].DefaultCellStyle.Format = "N0";
            }
            // Thiết lập chiều cao hàng để hiển thị hình ảnh tốt hơn
            dgvDanhSachChiTietNhapXe.RowTemplate.Height = 100;
            // Thiết lập chế độ hiển thị hình ảnh trong cột Hình Ảnh
            DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
            imgCol.Name = "HinhAnh";
            imgCol.HeaderText = "Hình Ảnh";
            imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            int colIndex = dgvDanhSachChiTietNhapXe.Columns["HinhAnh"].Index;
            dgvDanhSachChiTietNhapXe.Columns.RemoveAt(colIndex);
            dgvDanhSachChiTietNhapXe.Columns.Insert(colIndex, imgCol);
        }

        private void LoadBarChart()
        {
            // Xóa dữ liệu cũ
            chartBar.Series.Clear();
            chartBar.ChartAreas.Clear();

            // Tạo ChartArea
            ChartArea chartArea = new ChartArea("ChartArea1");
            chartBar.ChartAreas.Add(chartArea); // Đảm bảo chỉ gọi add trực tiếp

            // Tạo Series cho cột
            Series series = new Series("SoLuong");
            series.ChartType = SeriesChartType.Column;

            // Lấy tiêu chí từ ComboBox
            string criteria = cbbBarCriteria.SelectedItem?.ToString() ?? "Hãng xe";
            DataTable dt = quanLyNhapXe_Kho.GetDataForBarChart(criteria);

            // Thêm dữ liệu vào Series
            foreach (DataRow row in dt.Rows)
            {
                series.Points.AddXY(row["Category"].ToString(), Convert.ToInt32(row["SoLuong"]));
            }

            // Thêm Series vào Chart
            chartBar.Series.Add(series);

            // Tùy chỉnh giao diện
            chartBar.BackColor = Color.White;
        }

        private void LoadPieChart()
        {
            // Xóa dữ liệu cũ
            chartPie.Series.Clear();
            chartPie.ChartAreas.Clear();

            // Tạo ChartArea
            ChartArea chartArea = new ChartArea("ChartArea1");
            chartPie.ChartAreas.Add(chartArea);

            // Tạo Series cho tròn
            Series series = new Series("TiLe");
            series.ChartType = SeriesChartType.Pie;

            // Lấy tiêu chí từ ComboBox
            string criteria = cbbPieCriteria.SelectedItem?.ToString() ?? "Tình trạng";
            DataTable dt = quanLyNhapXe_Kho.GetDataForPieChart(criteria);

            // Thêm dữ liệu vào Series
            foreach (DataRow row in dt.Rows)
            {
                series.Points.AddXY(row["Category"].ToString(), Convert.ToInt32(row["SoLuong"]));
            }

            // Thêm Series vào Chart
            chartPie.Series.Add(series);

            // Định dạng
            series["PieLabelStyle"] = "Outside";
            series["PieLineColor"] = "Black";
            series.Color = Color.FromArgb(255, 99, 132);

            // Tùy chỉnh giao diện
            chartPie.BackColor = Color.White;
        }
        private void LoadTongQuanTonKho()
        {
            try
            {
                int tongSoXeTonKho = quanLyNhapXe_Kho.GetTongSoXeTonKho();
                decimal tongGiaTriTonKho = quanLyNhapXe_Kho.GetTongGiaTriTonKho();
                decimal tongLoiNhuan = quanLyNhapXe_Kho.GetTongLoiNhuanXe();
                decimal tongGiaTriNhap = quanLyNhapXe_Kho.GetTongGiaTriNhap();
                int tongSoXeNhap = quanLyNhapXe_Kho.GetTongSoXeNhap();

                txtTongSoXeTonKho.Text = tongSoXeTonKho.ToString();
                txtTongGiaTriTonKho.Text = string.Format("{0:N0} VNĐ", tongGiaTriTonKho);
                txtTongSoLuong.Text = tongSoXeTonKho.ToString();
                txtTongLoiNhuan.Text = string.Format("{0:N0} VNĐ", tongLoiNhuan);
                txtTong.Text = string.Format("{0:N0} VNĐ", tongGiaTriNhap);
                txtTongXeNhap.Text = tongSoXeNhap.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin tồn kho: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadXeTonKho()
        {
            try
            {
                dtXeTonKho = quanLyNhapXe_Kho.GetAllXeTonKho();

                // Nếu chưa có cột ảnh thì thêm mới
                if (!dtXeTonKho.Columns.Contains("HinhAnhImg"))
                    dtXeTonKho.Columns.Add("HinhAnhImg", typeof(System.Drawing.Image));

                string projectDir = Directory.GetParent(Application.StartupPath).Parent.FullName;

                foreach (DataRow row in dtXeTonKho.Rows)
                {
                    System.Drawing.Image img = null;
                    try
                    {
                        string relativePath = row["HinhAnh"]?.ToString();
                        if (!string.IsNullOrEmpty(relativePath))
                        {
                            string fullPath = Path.Combine(projectDir, relativePath);
                            if (File.Exists(fullPath))
                            {
                                // Dùng stream để tránh lock file
                                using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                                {
                                    img = System.Drawing.Image.FromStream(fs);
                                }
                            }
                        }
                    }
                    catch
                    {
                        img = null; // Nếu có lỗi, giữ nguyên null
                    }

                    row["HinhAnhImg"] = img;
                }

                dgvTonKhoChiTiet.DataSource = dtXeTonKho;
                FormatTonKhoGrid();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải xe tồn kho: {ex.Message}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatTonKhoGrid()
        {
            dgvTonKhoChiTiet.RowTemplate.Height = 100;

            // Ẩn cột không cần
            if (dgvTonKhoChiTiet.Columns["XeCTID"] != null)
                dgvTonKhoChiTiet.Columns["XeCTID"].Visible = false;
            if (dgvTonKhoChiTiet.Columns["HinhAnh"] != null)
                dgvTonKhoChiTiet.Columns["HinhAnh"].Visible = false;

            // Cột ảnh
            if (dgvTonKhoChiTiet.Columns["HinhAnhImg"] is DataGridViewImageColumn imgCol)
            {
                imgCol.HeaderText = "Hình ảnh";
                imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                imgCol.DisplayIndex = 0;
            }

            // Header text
            dgvTonKhoChiTiet.Columns["TenXe"].HeaderText = "Tên xe";
            dgvTonKhoChiTiet.Columns["HangXe"].HeaderText = "Hãng xe";
            dgvTonKhoChiTiet.Columns["LoaiXe"].HeaderText = "Loại xe";
            dgvTonKhoChiTiet.Columns["MauSac"].HeaderText = "Màu sắc";
            dgvTonKhoChiTiet.Columns["NamSX"].HeaderText = "Năm SX";
            dgvTonKhoChiTiet.Columns["DungTich"].HeaderText = "Dung tích";
            dgvTonKhoChiTiet.Columns["SoKhung"].HeaderText = "Số khung";
            dgvTonKhoChiTiet.Columns["SoMay"].HeaderText = "Số máy";
            dgvTonKhoChiTiet.Columns["TinhTrang"].HeaderText = "Tình trạng";
            dgvTonKhoChiTiet.Columns["DonGia"].HeaderText = "Đơn giá";
            dgvTonKhoChiTiet.Columns["DonGia"].DefaultCellStyle.Format = "N0";
        }




    }
}
