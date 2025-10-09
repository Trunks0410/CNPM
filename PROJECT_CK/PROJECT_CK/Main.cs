using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Guna.UI2.WinForms;
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
        System.Drawing.Image editIcon = Properties.Resources.chinhsua;
        System.Drawing.Image deleteIcon = Properties.Resources.xoa;
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
            LoadPhieuNhap();
            LoadThongKe();
            LoadNhaCungCap();
            LoadDanhMucComboboxes();
            cbbNguonNhapChiTiet.SelectedIndex = 0;
            cbbNguonNhapNX.SelectedIndex = 0;
            cbbHangXeFilter.SelectedIndex = -1; // Giả sử "Tất cả" là index 0
            cbbLoaiXeFilter.SelectedIndex = -1;
            cbbMauSacFilter.SelectedIndex = -1;
            cbbTinhTrangFilter.SelectedIndex = -1;
            cbbDonGiaOperator.SelectedIndex = -1;
            LoadDanhSachXeNhap();
            LoadBarChart();
            LoadPieChart();
            LoadTongQuanTonKho();
            LoadXeTonKho();
            LoadTextBoxAutoComplete();
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
            cbbMauSac.DataSource = dtMauSac;
            cbbMauSac.DisplayMember = "GiaTri";
            cbbMauSac.ValueMember = "DanhMucID";
            cbbMauSacNX.DataSource = dtMauSac;
            cbbMauSacNX.DisplayMember = "GiaTri";
            cbbMauSacNX.ValueMember = "DanhMucID";
            cbbMauSacFilter.DataSource = dtMauSac;
            cbbMauSacFilter.DisplayMember = "GiaTri";
            cbbMauSacFilter.ValueMember = "DanhMucID";

            DataTable dtTinhTrang = quanLyNhapXe_Kho.ExecuteDataTable("sp_GetDanhMucByLoai", CommandType.StoredProcedure,
                new SqlParameter("@LoaiDanhMuc", "TinhTrang"));
            cbbTinhTrangCT.DataSource = dtTinhTrang;
            cbbTinhTrangCT.DisplayMember = "GiaTri";
            cbbTinhTrangCT.ValueMember = "DanhMucID";
            cbbTinhTrangNX.DataSource = dtTinhTrang;
            cbbTinhTrangNX.DisplayMember = "GiaTri";
            cbbTinhTrangNX.ValueMember = "DanhMucID";
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

        private void LoadTextBoxAutoComplete()
        {
            LoadAutoCompleteForTextBox(txtTenNCC_KH, cbbNguonNhapChiTiet.Text, isTen: true);
            LoadAutoCompleteForTextBox(txtSDTNCC_KH, cbbNguonNhapChiTiet.Text, isTen: false);
            LoadAutoCompleteForTextBox(txtTenNCC_KHNX, cbbNguonNhapNX.Text, isTen: true);
            LoadAutoCompleteForTextBox(txtSDTNCC_KHNX, cbbNguonNhapNX.Text, isTen: false);
        }

        private void LoadAutoCompleteForTextBox(Guna2TextBox textBox, string loaiNhap, bool isTen)
        {
            List<string> items = null;
            try
            {
                if (string.IsNullOrEmpty(loaiNhap) || (!loaiNhap.Equals("NCC", StringComparison.OrdinalIgnoreCase) && !loaiNhap.Equals("KhachHang", StringComparison.OrdinalIgnoreCase)))
                {
                    // Nếu không hợp lệ, không làm gì hoặc đặt giá trị mặc định
                    textBox.AutoCompleteMode = AutoCompleteMode.None;
                    textBox.AutoCompleteSource = AutoCompleteSource.None;
                    textBox.AutoCompleteCustomSource = null;
                    return;
                }
                if (loaiNhap == "NCC")
                {
                    items = isTen ? quanLyNhapXe_Kho.GetTenNhaCungCap() : quanLyNhapXe_Kho.GetSoDTNhaCungCap();
                }
                else if (loaiNhap == "KhachHang")
                {
                    items = isTen ? quanLyNhapXe_Kho.GetTenKhachHang() : quanLyNhapXe_Kho.GetSoDTKhachHang();
                }
                AutoCompleteStringCollection autoComplete = new AutoCompleteStringCollection();
                autoComplete.AddRange(items.ToArray());

                textBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                textBox.AutoCompleteCustomSource = autoComplete;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thiết lập AutoComplete: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadThongTinChungPhieuNhap(int phieuNhapID)
        {

            DataTable dtPhieuNhap = quanLyNhapXe_Kho.GetThongTinChungPhieuNhap(phieuNhapID);
            if (dtPhieuNhap.Rows.Count > 0)
            {
                DataRow row = dtPhieuNhap.Rows[0];

                txtMaPhieuNhapChiTiet.Text = row["PhieuNhapID"].ToString();
                dtNgayNhapChiTiet.Value = Convert.ToDateTime(row["NgayNhap"]);
                cbbNguonNhapChiTiet.Text = row["LoaiNhap"].ToString();

                // Xử lý thông tin NCC hoặc Khách hàng
                if (row["LoaiNhap"].ToString() == "NCC")
                {
                    txtTenNCC_KH.Text = row["TenNCC"]?.ToString() ?? "";
                    txtSDTNCC_KH.Text = row["SoDienThoaiNCC"]?.ToString() ?? "";
                    txtDiaChiNCC_KH.Text = row["DiaChiNCC"]?.ToString() ?? "";
                }
                else if (row["LoaiNhap"].ToString() == "KhachHang")
                {
                    txtTenNCC_KH.Text = row["TenKhachHang"]?.ToString() ?? "";
                    txtSDTNCC_KH.Text = row["SoDienThoaiKH"]?.ToString() ?? "";
                    txtDiaChiNCC_KH.Text = row["DiaChiKH"]?.ToString() ?? "";
                }
            }
        }

        private void LoadChiTietXeNhap(int phieuNhapID)
        {
            DataTable dtChiTiet = quanLyNhapXe_Kho.GetDanhSachXeNhap(phieuNhapID);

            // Check if DataTable is null or empty
            if (dtChiTiet == null || dtChiTiet.Columns.Count == 0)
            {
                MessageBox.Show("No data returned for PhieuNhapID: " + phieuNhapID, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dgvDanhSachChiTietXeNhap.DataSource = null;
                return;
            }

            dgvDanhSachChiTietXeNhap.DataSource = dtChiTiet;

            // Hide columns if they exist
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("HinhAnh"))
                dgvDanhSachChiTietXeNhap.Columns["HinhAnh"].Visible = false;
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("MoTa"))
                dgvDanhSachChiTietXeNhap.Columns["MoTa"].Visible = false;
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("PhieuNhapID"))
                dgvDanhSachChiTietXeNhap.Columns["PhieuNhapID"].Visible = false;
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("XeCTID"))
                dgvDanhSachChiTietXeNhap.Columns["XeCTID"].Visible = false;

            // Set header text for columns if they exist
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("TenXe"))
                dgvDanhSachChiTietXeNhap.Columns["TenXe"].HeaderText = "Tên Xe";
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("HangXe"))
                dgvDanhSachChiTietXeNhap.Columns["HangXe"].HeaderText = "Hãng Xe";
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("LoaiXe"))
                dgvDanhSachChiTietXeNhap.Columns["LoaiXe"].HeaderText = "Loại Xe";
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("MauSac"))
                dgvDanhSachChiTietXeNhap.Columns["MauSac"].HeaderText = "Màu Sắc";
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("NamSX"))
                dgvDanhSachChiTietXeNhap.Columns["NamSX"].HeaderText = "Năm Sản Xuất";
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("DungTich"))
                dgvDanhSachChiTietXeNhap.Columns["DungTich"].HeaderText = "Dung Tích";
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("SoKhung"))
                dgvDanhSachChiTietXeNhap.Columns["SoKhung"].HeaderText = "Số Khung";
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("SoMay"))
                dgvDanhSachChiTietXeNhap.Columns["SoMay"].HeaderText = "Số Máy";
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("TinhTrang"))
                dgvDanhSachChiTietXeNhap.Columns["TinhTrang"].HeaderText = "Tình Trạng";
            if (dgvDanhSachChiTietXeNhap.Columns.Contains("DonGia"))
            {
                dgvDanhSachChiTietXeNhap.Columns["DonGia"].HeaderText = "Đơn Giá";
                dgvDanhSachChiTietXeNhap.Columns["DonGia"].DefaultCellStyle.Format = "N0";
            }
            if (!dgvDanhSachChiTietXeNhap.Columns.Contains("LuaChon"))
            {
                DataGridViewButtonColumn thaoTacColumn = new DataGridViewButtonColumn();
                thaoTacColumn.Name = "LuaChon";
                thaoTacColumn.HeaderText = "Thao tác";
                dgvDanhSachChiTietXeNhap.Columns.Add(thaoTacColumn);
            }
        }

        private void UpdateThongKe()
        {
            DateTime? tuNgay = dtTuNgay.Value;
            DateTime? denNgay = dtDenNgay.Value;
            string loaiNhap = cbbNguonNhap.SelectedItem?.ToString() ?? "All";
            int? nccID = string.IsNullOrWhiteSpace(txtMaNCC.Text) ? (int?)null : Convert.ToInt32(txtMaNCC.Text);
            int? khID = string.IsNullOrWhiteSpace(txtMaKH.Text) ? (int?)null : Convert.ToInt32(txtMaKH.Text);

            DataTable dt = quanLyNhapXe_Kho.GetThongKePhieuNhap(tuNgay, denNgay, loaiNhap, nccID, khID);

            if (dt.Rows.Count > 0)
            {
                txtTongSoPhieuNhap.Text = dt.Rows[0]["TongSoPhieu"].ToString();
                txtTongSoLuongXeNhap.Text = dt.Rows[0]["TongSoLuongXeNhap"].ToString();
                txtTongGiaTriNhap.Text = string.Format("{0:N0} VND", dt.Rows[0]["TongGiaTriNhap"]);
            }
        }

        private void btnpTimKiem_Click(object sender, EventArgs e)
        {
            DateTime? tuNgay = dtTuNgay.Value;
            DateTime? denNgay = dtDenNgay.Value;

            string loaiNhap = cbbNguonNhap.Text;
            int? nccID = string.IsNullOrWhiteSpace(txtMaNCC.Text) ? (int?)null : int.Parse(txtMaNCC.Text);
            int? khID = string.IsNullOrWhiteSpace(txtMaKH.Text) ? (int?)null : int.Parse(txtMaKH.Text);

            DataTable dt = quanLyNhapXe_Kho.TimKiemPhieuNhap(tuNgay, denNgay, loaiNhap, nccID, khID);

            dgvDanhSachPhieuNhap.AutoGenerateColumns = false;
            dgvDanhSachPhieuNhap.DataSource = dt;
            UpdateThongKe();
        }

        private void dgvDanhSachPhieuNhap_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvDanhSachPhieuNhap.Columns["LuaChon"].Index)
            {
                e.PaintBackground(e.CellBounds, true);

                int padding = 20;
                int iconSize = 50;

                int totalWidth = (iconSize * 2) + padding;
                int startX = e.CellBounds.X + (e.CellBounds.Width - totalWidth) / 2;
                int startY = e.CellBounds.Y + (e.CellBounds.Height - iconSize) / 2;

                Rectangle Sua = new Rectangle(startX, startY, iconSize, iconSize);
                e.Graphics.DrawImage(editIcon, Sua);

                Rectangle Xoa = new Rectangle(startX + iconSize + padding, startY, iconSize, iconSize);
                e.Graphics.DrawImage(deleteIcon, Xoa);

                e.Handled = true;
            }
        }

        private void dgvDanhSachPhieuNhap_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvDanhSachPhieuNhap.Rows[e.RowIndex].IsNewRow)
                return;

            object cellValue = dgvDanhSachPhieuNhap.Rows[e.RowIndex].Cells["MaPhieu"].Value;

            if (cellValue == null || cellValue == DBNull.Value)
                return;

            int phieuNhapID = Convert.ToInt32(cellValue);
            currentPhieuNhapID = phieuNhapID;

            if (e.ColumnIndex == dgvDanhSachPhieuNhap.Columns["LuaChon"].Index)
            {
                int padding = 20;
                int iconSize = 50;
                int totalWidth = (iconSize * 2) + padding;

                Rectangle cellBounds = dgvDanhSachPhieuNhap.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

                int startX = cellBounds.X + (cellBounds.Width - totalWidth) / 2;
                int startY = cellBounds.Y + (cellBounds.Height - iconSize) / 2;

                Rectangle suaRect = new Rectangle(startX, startY, iconSize, iconSize);
                Rectangle xoaRect = new Rectangle(startX + iconSize + padding, startY, iconSize, iconSize);

                Point clickPoint = System.Windows.Forms.Cursor.Position;
                clickPoint = dgvDanhSachPhieuNhap.PointToClient(clickPoint);

                if (suaRect.Contains(clickPoint))
                {
                    LoadThongTinChungPhieuNhap(phieuNhapID);
                    LoadChiTietXeNhap(phieuNhapID);
                    tabConTrolQLNX.SelectedTab = tabPageCTPN;
                }
                else if (xoaRect.Contains(clickPoint))
                {
                    DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa phiếu nhập này không?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            if (quanLyNhapXe_Kho.DeletePhieuNhap(phieuNhapID))
                            {
                                MessageBox.Show("Xóa phiếu nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadPhieuNhap();
                                LoadThongKe();
                                LoadBarChart();
                                LoadPieChart();
                                LoadTongQuanTonKho();
                                LoadXeTonKho();
                            }
                            else
                            {
                                MessageBox.Show("Không thể xóa phiếu nhập. Vui lòng thử lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.Show($"Lỗi SQL khi xóa phiếu nhập: {ex.Message}\nSố lỗi: {ex.Number}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi khi xóa phiếu nhập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void dgvDanhSachChiTietXeNhap_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvDanhSachChiTietXeNhap.Columns["LuaChon"].Index)
            {
                e.PaintBackground(e.CellBounds, true);

                int iconSize = 30;

                int startX = e.CellBounds.X + (e.CellBounds.Width - iconSize) / 2;
                int startY = e.CellBounds.Y + (e.CellBounds.Height - iconSize) / 2;

                Rectangle rect = new Rectangle(startX, startY, iconSize, iconSize);
                e.Graphics.DrawImage(deleteIcon, rect);

                e.Handled = true;
            }
        }

        private void dgvDanhSachChiTietXeNhap_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Nếu click vào header hoặc ngoài phạm vi -> bỏ qua
                if (e.RowIndex < 0 || e.RowIndex >= dgvDanhSachChiTietXeNhap.Rows.Count)
                    return;

                // Nếu click vào cột "LuaChon" -> xử lý nút xóa
                if (e.ColumnIndex == dgvDanhSachChiTietXeNhap.Columns["LuaChon"].Index)
                {
                    int iconSize = 30;

                    Rectangle cellBounds = dgvDanhSachChiTietXeNhap.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                    int startX = cellBounds.X + (cellBounds.Width - iconSize) / 2;
                    int startY = cellBounds.Y + (cellBounds.Height - iconSize) / 2;

                    Rectangle xoaRect = new Rectangle(startX, startY, iconSize, iconSize);
                    Point clickPoint = System.Windows.Forms.Cursor.Position; // Sửa lại thành System.Windows.Forms.Cursor.Position
                    clickPoint = dgvDanhSachChiTietXeNhap.PointToClient(clickPoint); // Chuyển đổi tọa độ

                    if (xoaRect.Contains(clickPoint))
                    {
                        int xeCTID = Convert.ToInt32(dgvDanhSachChiTietXeNhap.Rows[e.RowIndex].Cells["XeCTID"].Value);

                        DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa xe này không?", "Xác nhận xóa",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            if (quanLyNhapXe_Kho.DeleteXeFromPhieuNhap(currentPhieuNhapID, xeCTID))
                            {
                                MessageBox.Show("Xóa xe thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadChiTietXeNhap(currentPhieuNhapID); // load lại dữ liệu
                                LoadBarChart();
                                LoadPieChart();
                                LoadTongQuanTonKho();
                                LoadXeTonKho();
                            }
                            else
                            {
                                MessageBox.Show("Không thể xóa xe. Vui lòng thử lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        return; // tránh chạy xuống dưới khi vừa xóa
                    }
                }

                // Lấy row hiện tại
                DataGridViewRow row = dgvDanhSachChiTietXeNhap.Rows[e.RowIndex];

                // Đưa dữ liệu lên control
                cboHangXe.Text = row.Cells["HangXe"].Value?.ToString();
                cboLoaiXe.Text = row.Cells["LoaiXe"].Value?.ToString();
                cbbMauSac.Text = row.Cells["MauSac"].Value?.ToString();
                txtNSX.Text = row.Cells["NamSX"].Value?.ToString();
                txtTenXe.Text = row.Cells["TenXe"].Value?.ToString();
                txtMoTa.Text = row.Cells["MoTa"].Value?.ToString();
                txtSoKhungCT.Text = row.Cells["SoKhung"].Value?.ToString();
                txtSoMayCT.Text = row.Cells["SoMay"].Value?.ToString();
                cbbTinhTrangCT.Text = row.Cells["TinhTrang"].Value?.ToString();
                txtDonGia.Text = row.Cells["DonGia"].Value?.ToString();
                txtDungTichCT.Text = row.Cells["DungTich"].Value?.ToString();

                // Load hình ảnh
                string relativePath = row.Cells["HinhAnh"].Value?.ToString();
                if (!string.IsNullOrEmpty(relativePath))
                {
                    string projectDir = Directory.GetParent(Application.StartupPath).Parent.FullName;
                    string fullPath = Path.Combine(projectDir, relativePath);

                    if (File.Exists(fullPath))
                    {
                        // Giải phóng ảnh cũ
                        if (pbHinhAnh.Image != null)
                        {
                            pbHinhAnh.Image.Dispose();
                            pbHinhAnh.Image = null;
                        }

                        using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            pbHinhAnh.Image = System.Drawing.Image.FromStream(fs);
                        }
                        pbHinhAnh.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    else
                    {
                        pbHinhAnh.Image = Properties.Resources.placeholder;
                    }

                    txtHinhAnhSua.Text = relativePath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xử lý dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnpSuaPhieuNhap_Click(object sender, EventArgs e)
        {
            try
            {
                int phieuNhapID = int.Parse(txtMaPhieuNhapChiTiet.Text);
                DateTime ngayNhap = dtNgayNhapChiTiet.Value;
                string loaiNhap = cbbNguonNhapChiTiet.Text.Trim();

                // Lấy thông tin NCC hoặc Khách hàng từ các textbox
                string ten = txtTenNCC_KH.Text;
                string soDT = txtSDTNCC_KH.Text;
                string diaChi = txtDiaChiNCC_KH.Text;

                bool updated = quanLyNhapXe_Kho.UpdatePhieuNhap(phieuNhapID, ngayNhap, loaiNhap, ten, soDT, diaChi);

                if (updated)
                {
                    MessageBox.Show("Cập nhật phiếu nhập thành công!", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPhieuNhap();
                    LoadBarChart();
                    LoadPieChart();
                    LoadTongQuanTonKho();
                    LoadXeTonKho();
                }
                else
                {
                    MessageBox.Show("Không có dữ liệu nào được cập nhật.", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật phiếu nhập: " + ex.Message, "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnpSuaThongTinXe_Click(object sender, EventArgs e)
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
                        img = Properties.Resources.placeholder; // Ảnh mặc định nếu lỗi
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

        private void btnpChonHinhCT_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn hình ảnh";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string sourcePath = ofd.FileName;
                    string fileName = Path.GetFileName(sourcePath);

                    // Thư mục Images trong project
                    string projectDir = Directory.GetParent(Application.StartupPath).Parent.FullName;
                    string destDir = Path.Combine(projectDir, "Images");
                    if (!Directory.Exists(destDir))
                    {
                        Directory.CreateDirectory(destDir);
                    }

                    string destPath = Path.Combine(destDir, fileName);

                    // Giải phóng ảnh cũ trong PictureBox (nếu có) để tránh lock
                    if (pbHinhAnh.Image != null)
                    {
                        pbHinhAnh.Image.Dispose();
                        pbHinhAnh.Image = null;
                    }

                    // Nếu file nguồn KHÁC file đích thì mới copy
                    if (!string.Equals(sourcePath, destPath, StringComparison.OrdinalIgnoreCase))
                    {
                        File.Copy(sourcePath, destPath, true);
                    }

                    // Load ảnh vào PictureBox mà không lock file
                    using (var fs = new FileStream(destPath, FileMode.Open, FileAccess.Read))
                    {
                        pbHinhAnh.Image = System.Drawing.Image.FromStream(fs);
                    }
                    pbHinhAnh.SizeMode = PictureBoxSizeMode.Zoom;

                    // Lưu relative path vào TextBox để ghi DB
                    string relativePath = $"Images/{fileName}";
                    txtHinhAnhSua.Text = relativePath;
                }
            }
        }

        private void btnpChonHinhNX_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn hình ảnh";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string sourcePath = ofd.FileName;
                    string fileName = Path.GetFileName(sourcePath);

                    // Thư mục Images trong project
                    string projectDir = Directory.GetParent(Application.StartupPath).Parent.FullName;
                    string destDir = Path.Combine(projectDir, "Images");
                    if (!Directory.Exists(destDir))
                    {
                        Directory.CreateDirectory(destDir);
                    }

                    string destPath = Path.Combine(destDir, fileName);

                    // Nếu file nguồn KHÁC file đích thì mới copy
                    if (!string.Equals(sourcePath, destPath, StringComparison.OrdinalIgnoreCase))
                    {
                        File.Copy(sourcePath, destPath, true);
                    }

                    // Lưu relative path vào TextBox để ghi DB
                    string relativePath = $"Images/{fileName}";
                    txtHinhAnhNX.Text = relativePath;
                }
            }
        }

        private void TinhTongGiaTri()
        {
            try
            {
                decimal tongGiaTri = 0;
                foreach (var xe in tempXeList)
                {
                    if (decimal.TryParse(xe["DonGia"], out decimal donGia))
                    {
                        tongGiaTri += donGia;
                    }
                }
                // Định dạng số tiền (ví dụ: 1.234.567 VNĐ)
                txtTongGiaTriNX.Text = tongGiaTri.ToString("#,##0") + " VNĐ";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tính tổng giá trị: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTongGiaTriNX.Text = "0 VNĐ";
            }
        }

        private void btnpThemXeNX_Click(object sender, EventArgs e)
        {
            string soKhung = txtSoKhungNX.Text.Trim();
            string soMay = txtSoMayNX.Text.Trim();

            if (string.IsNullOrWhiteSpace(cbbTinhTrangNX.Text))
            {
                MessageBox.Show("Vui lòng chọn tình trạng xe!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbbHangXeNX.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn hãng xe!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbbLoaiXeNX.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn loại xe!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbbMauSacNX.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn màu sắc!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTenXeNX.Text))
            {
                MessageBox.Show("Vui lòng nhập tên xe!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNamSanXuatNX.Text))
            {
                MessageBox.Show("Vui lòng nhập năm sản xuất!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra input cơ bản (bạn có thể thêm nhiều hơn)
            if (string.IsNullOrWhiteSpace(soKhung) || string.IsNullOrWhiteSpace(soMay))
            {
                MessageBox.Show("Số khung và số máy không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Bước 1: Kiểm tra trùng lặp trong bảng tạm (tempXeList)
            bool trungTrongTam = tempXeList.Any(xe =>
                xe["SoKhung"].Equals(soKhung, StringComparison.OrdinalIgnoreCase) ||
                xe["SoMay"].Equals(soMay, StringComparison.OrdinalIgnoreCase));

            if (trungTrongTam)
            {
                MessageBox.Show("Số khung hoặc số máy đã tồn tại trong danh sách xe chờ xác nhận!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Bước 2: Kiểm tra trùng lặp với database
            int ketQuaCheckDB = quanLyNhapXe_Kho.CheckXeExists(soKhung, soMay);
            if (ketQuaCheckDB == 1)
            {
                MessageBox.Show("Số khung đã tồn tại trong hệ thống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (ketQuaCheckDB == 2)
            {
                MessageBox.Show("Số máy đã tồn tại trong hệ thống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                if (!int.TryParse(txtNamSanXuatNX.Text, out int namSX) || namSX < 1900 || namSX > DateTime.Now.Year + 1)
                {
                    MessageBox.Show("Năm sản xuất không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!decimal.TryParse(txtDonGiaNX.Text, out decimal donGia) || donGia <= 0)
                {
                    MessageBox.Show("Đơn giá không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var xe = new Dictionary<string, string>
        {
            { "SoKhung", txtSoKhungNX.Text.Trim() },
            { "SoMay", txtSoMayNX.Text.Trim() },
            { "TenXe", txtTenXeNX.Text.Trim() },
            { "HangXe", cbbHangXeNX.Text.Trim() },
            { "LoaiXe", cbbLoaiXeNX.Text.Trim() },
            { "MauSac", cbbMauSacNX.Text.Trim() },
            { "NamSX", txtNamSanXuatNX.Text.Trim() },
            { "DonGia", txtDonGiaNX.Text.Trim() },
            { "HinhAnh", txtHinhAnhNX.Text.Trim() },
            { "MoTa", txtMoTaNX.Text.Trim() },
            { "TinhTrang", cbbTinhTrangNX.Text.Trim()  },
            { "DungTich", txtDungTichNX.Text.Trim()  }

        };

                tempXeList.Add(xe);

                System.Drawing.Image img = null;
                try
                {
                    string projectDir = Directory.GetParent(Application.StartupPath).Parent.FullName;
                    string fullPath = Path.Combine(projectDir, xe["HinhAnh"]);
                    if (File.Exists(fullPath))
                    {
                        img = System.Drawing.Image.FromFile(fullPath);
                    }
                }
                catch
                {
                    // Nếu không load được ảnh thì để null hoặc icon mặc định
                    img = null;
                }

                dgvDanhSachChiTietNhapXe.Rows.Add(
                    img,
                    xe["TenXe"],
                    xe["HangXe"],
                    xe["LoaiXe"],
                    xe["MauSac"],
                    xe["NamSX"],
                    xe["SoKhung"],
                    xe["SoMay"],
                    xe["TinhTrang"],
                    xe["DonGia"],
                    xe["DungTich"],
                    xe["MoTa"]);

                TinhTongGiaTri();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm xe: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnpXoaXeNX_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có row nào được chọn không
            if (dgvDanhSachChiTietNhapXe.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một xe để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Hiển thị thông báo xác nhận xóa
            DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa xe này không?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Lấy chỉ số của row được chọn
                    int rowIndex = dgvDanhSachChiTietNhapXe.CurrentRow.Index;

                    // Xóa phần tử tương ứng trong tempXeList
                    if (rowIndex < tempXeList.Count)
                    {
                        tempXeList.RemoveAt(rowIndex);
                    }

                    // Xóa row khỏi DataGridView
                    dgvDanhSachChiTietNhapXe.Rows.RemoveAt(rowIndex);

                    MessageBox.Show("Xóa xe thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    TinhTongGiaTri();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa xe: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void resetFormNhapXe()
        {
            tempXeList.Clear();
            txtTongGiaTriNX.Text = "0 VNĐ";
            txtSoKhungNX.Clear();
            txtSoMayNX.Clear();
            txtTenXeNX.Clear();
            txtNamSanXuatNX.Clear();
            txtDonGiaNX.Clear();
            txtHinhAnhNX.Clear();
            txtMoTaNX.Clear();
            cbbTinhTrangNX.SelectedIndex = -1;
            txtDungTichNX.Clear();
            dgvDanhSachChiTietNhapXe.Rows.Clear();
            cbbNguonNhapNX.SelectedIndex = 0;
            txtTenNCC_KHNX.Clear();
            txtSDTNCC_KHNX.Clear();
            txtDiaChiNX.Clear();
            cbbHangXeNX.SelectedIndex = -1;
            cbbLoaiXeNX.SelectedIndex = -1;
            cbbMauSacNX.SelectedIndex = -1;
        }

        private void btnXacNhanNX_Click(object sender, EventArgs e)
        {
            try
            {
                if (tempXeList.Count == 0)
                {
                    MessageBox.Show("Vui lòng thêm ít nhất một xe!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DateTime ngayNhap = dtNgayNhapNX.Value;
                string loaiNhap = cbbNguonNhapNX.Text.Trim();
                string ten = txtTenNCC_KHNX.Text.Trim();
                string soDT = txtSDTNCC_KHNX.Text.Trim();
                string diaChi = txtDiaChiNX.Text.Trim();

                // Kiểm tra thông tin NCC hoặc Khách hàng
                if (loaiNhap == "NCC")
                {
                    if (string.IsNullOrWhiteSpace(ten) || string.IsNullOrWhiteSpace(soDT))
                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ thông tin nhà cung cấp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else if (loaiNhap == "KhachHang")
                {
                    if (string.IsNullOrWhiteSpace(ten) || string.IsNullOrWhiteSpace(soDT))
                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ thông tin khách hàng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Loại nhập không hợp lệ! Vui lòng chọn NCC hoặc KhachHang.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int phieuNhapID = quanLyNhapXe_Kho.InsertPhieuNhapWithXe(ngayNhap, loaiNhap, ten, soDT, diaChi, tempXeList);
                maPhieuCurrent = phieuNhapID;

                if (phieuNhapID > 0)
                {
                    MessageBox.Show($"Tạo phiếu nhập thành công! Mã phiếu: {phieuNhapID}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPhieuNhap();
                    LoadBarChart();
                    LoadPieChart();
                    LoadTongQuanTonKho();
                    LoadXeTonKho();
                    LoadThongKe();
                    resetFormNhapXe();
                    btnXuatPhieuNX.Enabled = true;


                }
                else
                {
                    MessageBox.Show("Không thể tạo phiếu nhập.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xác nhận phiếu nhập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXuatPhieuNX_Click(object sender, EventArgs e)
        {
            GeneratePhieuNhapExcel(maPhieuCurrent);
        }

        private void GeneratePhieuNhapExcel(int maPhieuNhap)
        {
            //try
            //{
            //    // Lấy thông tin phiếu
            //    DataTable dtPhieuNhap = quanLyNhapXe_Kho.GetThongTinChungPhieuNhap(maPhieuNhap);

            //    if (dtPhieuNhap.Rows.Count == 0)
            //    {
            //        MessageBox.Show("Không tìm thấy phiếu nhập.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        return;
            //    }

            //    DataRow rowPhieu = dtPhieuNhap.Rows[0];
            //    string ngayNhap = rowPhieu["NgayNhap"].ToString();
            //    string loaiNhap = rowPhieu["LoaiNhap"].ToString();
            //    string nhaCungCap = rowPhieu["TenNCC"]?.ToString() ?? "N/A";
            //    string soDienThoaiNCC = rowPhieu["SoDienThoaiNCC"]?.ToString() ?? "N/A";
            //    string diaChiNCC = rowPhieu["DiaChiNCC"]?.ToString() ?? "N/A";
            //    string tenKhachHang = rowPhieu["TenKhachHang"]?.ToString() ?? "N/A";
            //    string soDienThoaiKH = rowPhieu["SoDienThoaiKH"]?.ToString() ?? "N/A";
            //    string diaChiKH = rowPhieu["DiaChiKH"]?.ToString() ?? "N/A";

            //    DataTable dtXeNhap = quanLyNhapXe_Kho.GetDanhSachXeNhap(maPhieuNhap);

            //    decimal tongGiaTri = dtXeNhap.AsEnumerable()
            //        .Where(r => r["DonGia"] != DBNull.Value)
            //        .Sum(r => Convert.ToDecimal(r["DonGia"]));
            //    int tongSoLuong = dtXeNhap.Rows.Count;

            //    using (var workbook = new XLWorkbook())
            //    {
            //        var ws = workbook.Worksheets.Add("Phiếu nhập");

            //        // ======================= HEADER ===========================
            //        ws.Cell(1, 1).Value = "CỬA HÀNG MUA BÁN XE MÁY";
            //        ws.Range(1, 1, 1, 6).Merge().Style
            //            .Font.SetBold().Font.SetFontSize(12)
            //            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            //        ws.Cell(2, 1).Value = "PHIẾU NHẬP KHO XE MÁY";
            //        ws.Range(2, 1, 2, 6).Merge().Style
            //            .Font.SetBold().Font.SetFontSize(16)
            //            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            //        // ======================= THÔNG TIN ========================
            //        int row = 4;
            //        ws.Cell(row++, 1).Value = "Mã phiếu: " + maPhieuNhap;
            //        ws.Cell(row++, 1).Value = "Ngày nhập: " + ngayNhap;
            //        ws.Cell(row++, 1).Value = "Loại nhập: " + loaiNhap;

            //        if (loaiNhap == "NCC")
            //        {
            //            ws.Cell(row++, 1).Value = "Nhà cung cấp: " + nhaCungCap;
            //            ws.Cell(row++, 1).Value = "SĐT NCC: " + soDienThoaiNCC;
            //            ws.Cell(row++, 1).Value = "Địa chỉ NCC: " + diaChiNCC;
            //        }
            //        else if (loaiNhap == "KhachHang")
            //        {
            //            ws.Cell(row++, 1).Value = "Khách hàng: " + tenKhachHang;
            //            ws.Cell(row++, 1).Value = "SĐT KH: " + soDienThoaiKH;
            //            ws.Cell(row++, 1).Value = "Địa chỉ KH: " + diaChiKH;
            //        }


            //        // ======================= DANH SÁCH XE ======================
            //        row += 1;
            //        string[] headers = { "Tên xe", "Hãng xe", "Loại xe", "Màu sắc", "Năm SX",
            //                 "Dung tích", "Số khung", "Số máy", "Tình trạng", "Đơn giá" };

            //        for (int i = 0; i < headers.Length; i++)
            //        {
            //            ws.Cell(row, i + 1).Value = headers[i];
            //            ws.Cell(row, i + 1).Style.Font.SetBold();
            //            ws.Cell(row, i + 1).Style.Fill.SetBackgroundColor(XLColor.LightGray);
            //            ws.Cell(row, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            //        }

            //        // Dữ liệu
            //        for (int i = 0; i < dtXeNhap.Rows.Count; i++)
            //        {
            //            DataRow r = dtXeNhap.Rows[i];
            //            for (int j = 0; j < headers.Length; j++)
            //            {
            //                ws.Cell(row + i + 1, j + 1).Value = r[j]?.ToString();
            //                ws.Cell(row + i + 1, j + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            //            }
            //        }

            //        row += dtXeNhap.Rows.Count + 1;
            //        ws.Cell(row, 1).Value = "Tổng số lượng:";
            //        ws.Cell(row, 10).Value = tongSoLuong;

            //        row++;
            //        ws.Cell(row, 1).Value = "Tổng giá trị:";
            //        ws.Cell(row, 10).Value = tongGiaTri.ToString("N0") + " VNĐ";

            //        // ======================= CHỮ KÝ ============================
            //        row += 3;
            //        ws.Cell(row, 2).Value = "Khách hàng";
            //        ws.Cell(row, 8).Value = "Nhân viên";

            //        ws.Columns().AdjustToContents();

            //        // ======================= SAVE ==============================
            //        using (SaveFileDialog sfd = new SaveFileDialog())
            //        {
            //            sfd.Filter = "Excel Files|*.xlsx";
            //            sfd.Title = "Chọn nơi lưu phiếu nhập";
            //            sfd.FileName = $"PhieuNhap_{maPhieuNhap}.xlsx";

            //            if (sfd.ShowDialog() == DialogResult.OK)
            //            {
            //                workbook.SaveAs(sfd.FileName);
            //                MessageBox.Show($"Phiếu nhập đã được xuất thành công tại:\n{sfd.FileName}",
            //                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private void cbbBarCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadBarChart();
        }

        private void cbbPieCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPieChart();
        }

        private void btnpTimKiemTonKho_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtXeTonKho == null || dtXeTonKho.Rows.Count == 0)
                {
                    MessageBox.Show("Dữ liệu xe tồn kho chưa được tải. Vui lòng tải lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string filterExpression = BuildFilterExpression();

                DataView dv = new DataView(dtXeTonKho);
                if (!string.IsNullOrEmpty(filterExpression))
                {
                    dv.RowFilter = filterExpression;
                }
                else
                {
                    dv.RowFilter = "";
                }

                dgvTonKhoChiTiet.DataSource = dv;
                FormatTonKhoGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc: {ex.Message}\nStackTrace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string BuildFilterExpression()
        {
            string filter = "";
            bool hasFilter = false;

            // Lọc TenXe (TextBox)
            if (!string.IsNullOrEmpty(txtTenXeFilter.Text?.Trim()))
            {
                filter += $"TenXe LIKE '%{txtTenXeFilter.Text.Trim().Replace("'", "''")}%'";
                hasFilter = true;
            }

            // Lọc HangXe (dùng Text thay vì SelectedItem.ToString())
            string hangXeValue = cbbHangXeFilter.Text;
            if (!string.IsNullOrEmpty(hangXeValue) && hangXeValue != "Tất cả")
            {
                filter += hasFilter ? " AND " : "";
                filter += $"HangXe = '{hangXeValue.Replace("'", "''")}'";
                hasFilter = true;
            }

            // Lọc LoaiXe
            string loaiXeValue = cbbLoaiXeFilter.Text;
            if (!string.IsNullOrEmpty(loaiXeValue) && loaiXeValue != "Tất cả")
            {
                filter += hasFilter ? " AND " : "";
                filter += $"LoaiXe = '{loaiXeValue.Replace("'", "''")}'";
                hasFilter = true;
            }

            // Lọc MauSac
            string mauSacValue = cbbMauSacFilter.Text;
            if (!string.IsNullOrEmpty(mauSacValue) && mauSacValue != "Tất cả")
            {
                filter += hasFilter ? " AND " : "";
                filter += $"MauSac = '{mauSacValue.Replace("'", "''")}'";
                hasFilter = true;
            }

            // Lọc TinhTrang
            string tinhTrangValue = cbbTinhTrangFilter.Text;
            if (!string.IsNullOrEmpty(tinhTrangValue) && tinhTrangValue != "Tất cả")
            {
                filter += hasFilter ? " AND " : "";
                filter += $"TinhTrang = '{tinhTrangValue.Replace("'", "''")}'";
                hasFilter = true;
            }

            // Lọc NamSX (TextBox)
            if (!string.IsNullOrEmpty(txtNamSXFilter.Text?.Trim()))
            {
                if (int.TryParse(txtNamSXFilter.Text.Trim(), out int namSX))
                {
                    filter += hasFilter ? " AND " : "";
                    filter += $"Convert(NamSX, 'System.Int32') = {namSX}";
                    hasFilter = true;
                }
                else
                {
                    MessageBox.Show("Năm sản xuất phải là số nguyên.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return "";
                }
            }

            // Lọc DungTich (TextBox) - số thực
            if (!string.IsNullOrEmpty(txtDungTichFilter.Text?.Trim()))
            {
                if (decimal.TryParse(txtDungTichFilter.Text.Trim(), out decimal dungTich))
                {
                    filter += hasFilter ? " AND " : "";
                    filter += $"Convert(DungTich, 'System.Decimal') = {dungTich}";
                    hasFilter = true;
                }
                else
                {
                    MessageBox.Show("Dung tích phải là số thực hợp lệ (ví dụ: 50.0, 110.5).", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return "";
                }
            }

            // Lọc DonGia
            if (!string.IsNullOrEmpty(txtDonGiaFilter.Text) && decimal.TryParse(txtDonGiaFilter.Text, out decimal gia))
            {
                filter += hasFilter ? " AND " : "";
                filter += $"Convert(DonGia, 'System.Decimal') {cbbDonGiaOperator.Text} {gia}";
                hasFilter = true;
            }


            return filter;
        }

        private void btnpResetTonKho_Click(object sender, EventArgs e)
        {
            try
            {
                cbbHangXeFilter.SelectedIndex = -1; 
                cbbLoaiXeFilter.SelectedIndex = -1;
                cbbMauSacFilter.SelectedIndex = -1;
                cbbTinhTrangFilter.SelectedIndex = -1;
                cbbDonGiaOperator.SelectedIndex = -1;
                txtDonGiaFilter.Clear();
                txtTenXeFilter.Clear();
                txtNamSXFilter.Clear();
                txtDungTichFilter.Clear();
                txtSoKhungFilter.Clear();
                txtSoMayFilter.Clear();

                dgvTonKhoChiTiet.DataSource = dtXeTonKho; 
                FormatTonKhoGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa lọc: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnpThemNCC_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenNCC.Text) || string.IsNullOrWhiteSpace(txtDiaChiNCC.Text) || string.IsNullOrWhiteSpace(txtSDTNCC.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin nhà cung cấp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (quanLyNhapXe_Kho.InsertNhaCungCap(txtTenNCC.Text, txtDiaChiNCC.Text, txtSDTNCC.Text))
            {
                MessageBox.Show("Thêm NCC thành công!");
                LoadNhaCungCap();
            }
        }

        private void btnpXoaNCC_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtNCCID.Text, out int nccID))
            {
                int result = quanLyNhapXe_Kho.DeleteNhaCungCap(nccID);

                switch (result)
                {
                    case 0:
                        MessageBox.Show("Xóa nhà cung cấp thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadNhaCungCap();
                        break;
                    case 1:
                        MessageBox.Show("Nhà cung cấp không tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    case 2:
                        MessageBox.Show("Nhà cung cấp đang được sử dụng trong phiếu nhập. Vui lòng xóa hoặc cập nhật trước.",
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    default:
                        MessageBox.Show("Không thể xóa nhà cung cấp. Có lỗi xảy ra.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập ID nhà cung cấp hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnpSuaNCC_Click(object sender, EventArgs e)
        {
            int id = int.Parse(txtNCCID.Text);
            if (quanLyNhapXe_Kho.UpdateNhaCungCap(id, txtTenNCC.Text, txtDiaChiNCC.Text, txtSDTNCC.Text))
            {
                MessageBox.Show("Cập nhật NCC thành công!");
                LoadNhaCungCap();
            }
        }

        private void btnResetNCC_Click(object sender, EventArgs e)
        {
            txtNCCID.Clear();
            txtTenNCC.Clear();
            txtDiaChiNCC.Clear();
            txtSDTNCC.Clear();
        }

        private void dgvNCC_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvNCC.Rows[e.RowIndex];

                txtNCCID.Text = row.Cells["NCCID"].Value.ToString();
                txtTenNCC.Text = row.Cells["TenNCC"].Value.ToString();
                txtDiaChiNCC.Text = row.Cells["DiaChi"].Value.ToString();
                txtSDTNCC.Text = row.Cells["SoDienThoai"].Value.ToString();
            }
        }

        private void btnpThemDanhMuc_Click(object sender, EventArgs e)
        {
            string loaiDanhMuc = cbbLoaiDanhMuc.Text.Trim();
            string giaTri = txtGiaTri.Text.Trim();

            if (string.IsNullOrEmpty(loaiDanhMuc))
            {
                MessageBox.Show("Vui lòng chọn loại danh mục!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!KiemTraGiaTriHopLe(giaTri))
                return;

            try
            {
                bool result = quanLyNhapXe_Kho.InsertDanhMuc(loaiDanhMuc, giaTri);
                if (result)
                {
                    MessageBox.Show($"Thêm danh mục '{giaTri}' thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhMuc(loaiDanhMuc);
                    txtGiaTri.Clear();
                }
                else
                {
                    MessageBox.Show($"Giá trị '{giaTri}' đã tồn tại trong danh mục '{loaiDanhMuc}'!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm danh mục: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool KiemTraGiaTriHopLe(string giaTri)
        {
            if (string.IsNullOrWhiteSpace(giaTri))
            {
                MessageBox.Show("Giá trị danh mục không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (giaTri.Length > 100)
            {
                MessageBox.Show("Giá trị danh mục không được dài quá 100 ký tự!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!Regex.IsMatch(giaTri, @"^[a-zA-Z0-9\s]+$"))
            {
                MessageBox.Show("Giá trị danh mục chỉ được chứa chữ cái, số và khoảng trắng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void LoadDanhMuc(string loaiDanhMuc)
        {
            DataTable dt = quanLyNhapXe_Kho.GetDanhMucByLoai(loaiDanhMuc);
            dgvDanhMuc.DataSource = dt;
            if (dgvDanhMuc.Columns["DanhMucID"] != null)
            {
                dgvDanhMuc.Columns["DanhMucID"].Visible = false;
            }
            txtGiaTri.Clear();
            btnpSuaDanhMuc.Enabled = false;
            btnpXoaDanhMuc.Enabled = false;
        }

        private void btnpXoaDanhMuc_Click(object sender, EventArgs e)
        {
            if (dgvDanhMuc.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một danh mục để xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string loaiDanhMuc = cbbLoaiDanhMuc.Text.Trim();
            var row = dgvDanhMuc.SelectedRows[0];

            int danhMucID = Convert.ToInt32(row.Cells["DanhMucID"].Value);
            string giaTri = row.Cells["GiaTri"].Value?.ToString();

            if (MessageBox.Show($"Bạn có chắc muốn xóa danh mục '{giaTri}'?",
                                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    bool result = quanLyNhapXe_Kho.DeleteDanhMuc(danhMucID);
                    if (result)
                    {
                        MessageBox.Show($"Xóa danh mục '{giaTri}' thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDanhMuc(loaiDanhMuc);
                    }
                    else
                    {
                        MessageBox.Show($"Không thể xóa danh mục '{giaTri}' vì nó đang được sử dụng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa danh mục: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnpSuaDanhMuc_Click(object sender, EventArgs e)
        {
            if (dgvDanhMuc.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một danh mục để sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string loaiDanhMuc = cbbLoaiDanhMuc.Text.Trim();
            string giaTri = txtGiaTri.Text.Trim();

            if (!KiemTraGiaTriHopLe(giaTri))
                return;

            var row = dgvDanhMuc.SelectedRows[0];
            int danhMucID = Convert.ToInt32(row.Cells["DanhMucID"].Value);

            try
            {
                bool result = quanLyNhapXe_Kho.UpdateDanhMuc(danhMucID, giaTri);
                if (result)
                {
                    MessageBox.Show($"Sửa danh mục thành '{giaTri}' thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhMuc(loaiDanhMuc);
                    txtGiaTri.Clear();
                }
                else
                {
                    MessageBox.Show($"Giá trị '{giaTri}' đã tồn tại trong danh mục '{loaiDanhMuc}'!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi sửa danh mục: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbbLoaiDanhMuc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbLoaiDanhMuc.SelectedItem != null)
            {
                LoadDanhMuc(cbbLoaiDanhMuc.SelectedItem.ToString());
                txtGiaTri.Clear(); // Xóa trường nhập liệu
                btnpThemDanhMuc.Enabled = true;
                btnpSuaDanhMuc.Enabled = false;
                btnpXoaDanhMuc.Enabled = false;
            }
        }

        private void dgvDanhMuc_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDanhMuc.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvDanhMuc.SelectedRows[0];
                txtGiaTri.Text = row.Cells["GiaTri"].Value.ToString();
                btnpSuaDanhMuc.Enabled = true;
                btnpXoaDanhMuc.Enabled = true;
            }
            else
            {
                txtGiaTri.Clear();
                btnpSuaDanhMuc.Enabled = false;
                btnpXoaDanhMuc.Enabled = false;
            }
        }
    }
}
